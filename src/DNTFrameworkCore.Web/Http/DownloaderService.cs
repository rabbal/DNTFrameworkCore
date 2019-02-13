using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Web.Network;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Web.Http
{
       /// <summary>
    /// DownloaderService Extensions
    /// </summary>
    public static class DownloaderServiceExtensions
    {
        /// <summary>
        /// Adds IDownloaderService to IServiceCollection
        /// </summary>
        public static IServiceCollection AddDownloaderService(this IServiceCollection services)
        {
            services.AddTransient<IDownloaderService, DownloaderService>();
            return services;
        }
    }

    /// <summary>
    /// Downloader Service
    /// </summary>
    public interface IDownloaderService
    {
        /// <summary>
        /// Downloads a file from a given url and then stores it as a local file.
        /// </summary>
        Task<DownloadStatus> DownloadFileAsync(string url, string outputFilePath, AutoRetriesPolicy autoRetries = null);

        /// <summary>
        /// Downloads a file from a given url and then returns it as a byte array.
        /// </summary>
        Task<(byte[] Data, DownloadStatus DownloadStatus)> DownloadDataAsync(string url, AutoRetriesPolicy autoRetries = null);

        /// <summary>
        /// Downloads a file from a given url and then returns it as a text.
        /// </summary>
        Task<(string Data, DownloadStatus DownloadStatus)> DownloadPageAsync(string url, AutoRetriesPolicy autoRetries = null);

        /// <summary>
        /// Gives the current download operation's status.
        /// </summary>
        Action<DownloadStatus> OnDownloadStatusChanged { set; get; }

        /// <summary>
        /// It fires when the download operation is completed.
        /// </summary>
        Action<DownloadStatus> OnDownloadCompleted { set; get; }

        /// <summary>
        /// Cancels the download operation.
        /// </summary>
        void CancelDownload();
    }

    /// <summary>
    /// Auto Retries Policy
    /// </summary>
    public class AutoRetriesPolicy
    {
        /// <summary>
        /// How many time the downloader service should retries if an error has occurred.
        /// </summary>
        public int MaxRequestAutoRetries { set; get; }

        /// <summary>
        /// How much time the downloader service should wait between retries if an error has occurred.
        /// </summary>
        public TimeSpan AutoRetriesDelay { set; get; }
    }

    /// <summary>
    /// Download Status
    /// </summary>
    public class DownloadStatus
    {
        /// <summary>
        /// Estimated Remote FileSize
        /// </summary>
        public long RemoteFileSize { get; set; }

        /// <summary>
        /// Current downloaded bytes.
        /// </summary>
        public long BytesTransferred { get; set; }

        /// <summary>
        /// Download's start time.
        /// </summary>
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// Received remote file name from the server.
        /// </summary>
        public string RemoteFileName { get; set; }

        /// <summary>
        /// Current task's status.
        /// </summary>
        public TaskStatus Status { get; set; }

        /// <summary>
        /// Current percent of download operation.
        /// </summary>
        public decimal PercentComplete { get; set; }

        /// <summary>
        /// Download speed.
        /// </summary>
        public double BytesPerSecond { get; set; }

        /// <summary>
        /// Estimated Completion Time
        /// </summary>
        public TimeSpan EstimatedCompletionTime { get; set; }

        /// <summary>
        /// Elapsed Download Time
        /// </summary>
        public TimeSpan ElapsedDownloadTime { get; set; }
    }

    /// <summary>
    /// Lifetime of this class should be set to `Transient`.
    /// Because setting HttpClient's `DefaultRequestHeaders` is not thread-safe and can't be shared across different threads.
    /// </summary>
    public class DownloaderService : IDownloaderService
    {
        private const int MaxBufferSize = 0x10000; // 64K. The artificial constraint due to win32 api limitations. Increasing the buffer size beyond 64k will not help in any circumstance, as the underlying SMB protocol does not support buffer lengths beyond 64k.
        private readonly HttpClient _client = new HttpClient(new HttpClientHandler
        {
            UseProxy = false,
            Proxy = null,
            AllowAutoRedirect = true,
            CookieContainer = new CookieContainer(),
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
        })
        {
            Timeout = Timeout.InfiniteTimeSpan
        };
        private readonly CancellationTokenSource _cancelSrc = new CancellationTokenSource();
        private readonly DownloadStatus _downloadStatus = new DownloadStatus();

        /// <summary>
        /// Gives the current download operation's status.
        /// </summary>
        public Action<DownloadStatus> OnDownloadStatusChanged { set; get; }

        /// <summary>
        /// It fires when the download operation is completed.
        /// </summary>
        public Action<DownloadStatus> OnDownloadCompleted { set; get; }

        /// <summary>
        /// Downloads a file from a given url and then stores it as a local file.
        /// </summary>
        public Task<DownloadStatus> DownloadFileAsync(string url, string outputFilePath, AutoRetriesPolicy autoRetries = null)
        {
            return downloadAsync(() => doDownloadFileAsync(url, outputFilePath), autoRetries);
        }

        /// <summary>
        /// Downloads a file from a given url and then returns it as a byte array.
        /// </summary>
        public Task<(byte[] Data, DownloadStatus DownloadStatus)> DownloadDataAsync(string url, AutoRetriesPolicy autoRetries = null)
        {
            return downloadAsync(() => doDownloadDataAsync(url), autoRetries);
        }

        /// <summary>
        /// Downloads a file from a given url and then returns it as a text.
        /// </summary>
        public async Task<(string Data, DownloadStatus DownloadStatus)> DownloadPageAsync(string url, AutoRetriesPolicy autoRetries = null)
        {
            var result = await DownloadDataAsync(url, autoRetries);
            return result.Data == null ? (string.Empty, _downloadStatus) : (Encoding.UTF8.GetString(result.Data), _downloadStatus);
        }

        private async Task<T> downloadAsync<T>(Func<Task<T>> task, AutoRetriesPolicy autoRetries)
        {
            if (autoRetries == null)
            {
                autoRetries = new AutoRetriesPolicy
                {
                    MaxRequestAutoRetries = 2,
                    AutoRetriesDelay = TimeSpan.FromSeconds(2)
                };
            }

            var exceptions = new List<Exception>();

            T result = default(T);
            do
            {
                --autoRetries.MaxRequestAutoRetries;
                try
                {
                    result = await task();
                }
                catch (TaskCanceledException ex)
                {
                    _downloadStatus.Status = _cancelSrc.IsCancellationRequested ? TaskStatus.Canceled : TaskStatus.Faulted;
                    exceptions.Add(ex);
                }
                catch (HttpRequestException ex)
                {
                    _downloadStatus.Status = TaskStatus.Faulted;
                    exceptions.Add(ex);
                }
                catch (Exception ex) when (ex.IsNetworkError())
                {
                    _downloadStatus.Status = TaskStatus.Faulted;
                    exceptions.Add(ex);
                }

                if (exceptions.Any())
                {
                    // Wait a bit and try again later
                    await Task.Delay(autoRetries.AutoRetriesDelay, _cancelSrc.Token);
                }
            } while (autoRetries.MaxRequestAutoRetries > 0 &&
                     _downloadStatus.Status != TaskStatus.RanToCompletion &&
                     !_cancelSrc.IsCancellationRequested);

            var uniqueExceptions = exceptions.Distinct(new ExceptionEqualityComparer()).ToList();
            if (uniqueExceptions.Any() &&
               _downloadStatus.Status != TaskStatus.RanToCompletion)
            {
                if (uniqueExceptions.Count() == 1)
                    throw uniqueExceptions.First();
                throw new AggregateException("Could not process the request.", uniqueExceptions);
            }

            return result;
        }

        /// <summary>
        /// Cancels the download operation.
        /// </summary>
        public void CancelDownload()
        {
            _downloadStatus.Status = TaskStatus.Canceled;
            _cancelSrc.Cancel();
            _client.CancelPendingRequests();
        }

        private async Task<DownloadStatus> doDownloadFileAsync(string url, string outputFilePath)
        {
            _downloadStatus.StartTime = DateTimeOffset.UtcNow;
            _downloadStatus.BytesTransferred = 0;

            var fileMode = FileMode.CreateNew;
            var fileInfo = new FileInfo(outputFilePath);
            if (fileInfo.Exists)
            {
                _downloadStatus.BytesTransferred = fileInfo.Length;
                fileMode = FileMode.Append;
            }

            if (_downloadStatus.BytesTransferred > 0)
            {
                _client.DefaultRequestHeaders.Range = new RangeHeaderValue(_downloadStatus.BytesTransferred, null);
            }

            var response = await readResponseHeadersAsync(url);

            if (_downloadStatus.BytesTransferred > 0 &&
                _downloadStatus.RemoteFileSize == _downloadStatus.BytesTransferred)
            {
                reportDownloadCompleted();
                return _downloadStatus;
            }

            using (var inputStream = await response.Content.ReadAsStreamAsync())
            {
                using (var fileStream = new FileStream(outputFilePath, fileMode, FileAccess.Write,
                                                       FileShare.None,
                                                       MaxBufferSize,
                                                       // you have to explicitly open the FileStream as asynchronous
                                                       // or else you're just doing synchronous operations on a background thread.
                                                       useAsync: true
                                                       ))
                {
                    if (response.Headers.AcceptRanges == null && fileStream.Length > 0)
                    {
                        // Resume is not supported. Starting over.
                        fileStream.SetLength(0);
                        fileStream.Flush();
                        _downloadStatus.BytesTransferred = 0;
                    }

                    await readInputStreamAsync(inputStream, fileStream);
                } // The Close() calls the Flush(), so you don't need to call it manually.
            }

            reportDownloadCompleted();
            return _downloadStatus;
        }

        private async Task readInputStreamAsync(Stream inputStream, Stream outputStream)
        {
            var buffer = new byte[MaxBufferSize];
            int read;
            var readCount = 0L;
            while ((read = await inputStream.ReadAsync(buffer, 0, buffer.Length, _cancelSrc.Token)) > 0 &&
                   !_cancelSrc.IsCancellationRequested)
            {
                _downloadStatus.BytesTransferred += read;
                _downloadStatus.Status = TaskStatus.Running;
                readCount++;

                if (readCount % 100 == 0)
                {
                    updateDownloadStatus();
                }

                await outputStream.WriteAsync(buffer, 0, read, _cancelSrc.Token);
            }
        }

        private async Task<HttpResponseMessage> readResponseHeadersAsync(string url)
        {
            _client.DefaultRequestHeaders.ExpectContinue = false;
            _client.DefaultRequestHeaders.Add("Keep-Alive", "false");
            _client.DefaultRequestHeaders.Add("User-Agent", typeof(DownloaderService).Namespace);
            _client.DefaultRequestHeaders.Referrer = new Uri(url);

            var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, _cancelSrc.Token);
            response.EnsureSuccessStatusCode();

            _downloadStatus.RemoteFileSize =
               response.Content.Headers?.ContentRange?.Length ?? response.Content?.Headers?.ContentLength ?? 0;
            _downloadStatus.RemoteFileName =
               response.Content?.Headers?.ContentDisposition?.FileName ?? String.Empty;
            return response;
        }

        private async Task<(byte[] Data, DownloadStatus DownloadStatus)> doDownloadDataAsync(string url)
        {
            _downloadStatus.StartTime = DateTimeOffset.UtcNow;
            _downloadStatus.BytesTransferred = 0;

            var response = await readResponseHeadersAsync(url);

            using (var inputStream = await response.Content.ReadAsStreamAsync())
            {
                using (var outputStream = new MemoryStream())
                {
                    await readInputStreamAsync(inputStream, outputStream);
                    await outputStream.FlushAsync();

                    reportDownloadCompleted();

                    return (outputStream.ToArray(), _downloadStatus);
                }
            }
        }

        private void reportDownloadCompleted()
        {
            _downloadStatus.ElapsedDownloadTime = DateTimeOffset.UtcNow - _downloadStatus.StartTime;
            _downloadStatus.Status = _cancelSrc.IsCancellationRequested ? TaskStatus.Canceled : TaskStatus.RanToCompletion;

            OnDownloadCompleted?.Invoke(_downloadStatus);
        }

        private void updateDownloadStatus()
        {
            if (_downloadStatus.RemoteFileSize <= 0 || _downloadStatus.BytesTransferred <= 0)
            {
                return;
            }

            _downloadStatus.PercentComplete =
               Math.Round((decimal)_downloadStatus.BytesTransferred * 100 / (decimal)_downloadStatus.RemoteFileSize, 2);

            var elapsedTime = DateTimeOffset.UtcNow - _downloadStatus.StartTime;
            _downloadStatus.BytesPerSecond = _downloadStatus.BytesTransferred / elapsedTime.TotalSeconds;

            var rawEta = _downloadStatus.RemoteFileSize / _downloadStatus.BytesPerSecond;
            _downloadStatus.EstimatedCompletionTime = TimeSpan.FromSeconds(rawEta);

            OnDownloadStatusChanged?.Invoke(_downloadStatus);
        }
    }

    /// <summary>
    /// Exception EqualityComparer
    /// </summary>
    public class ExceptionEqualityComparer : IEqualityComparer<Exception>
    {
        /// <summary>
        /// Checks if two exceptions are equal.
        /// </summary>
        public bool Equals(Exception ex1, Exception ex2)
        {
            if (ex2 == null && ex1 == null)
                return true;
            if (ex1 == null | ex2 == null)
                return false;
            if (ex1.GetType().Name.Equals(ex2.GetType().Name) && ex1.Message.Equals(ex2.Message))
                return true;
            return false;
        }

        /// <summary>
        /// Gets the exception's hash.
        /// </summary>
        public int GetHashCode(Exception ex)
        {
            return (ex.GetType().Name + ex.Message).GetHashCode();
        }
    }
}
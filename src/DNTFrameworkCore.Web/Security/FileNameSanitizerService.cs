using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Web.Security
{
    /// <summary>
    /// SafeFile Download Service Extensions
    /// </summary>
    public static class FileNameSanitizerServiceExtensions
    {
        /// <summary>
        /// Adds IFileNameSanitizerService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddFileNameSanitizerService(this IServiceCollection services)
        {
            services.AddTransient<IFileNameSanitizerService, FileNameSanitizerService>();
            return services;
        }
    }

    /// <summary>
    /// SafeFile Download Service
    /// </summary>
    public interface IFileNameSanitizerService
    {
        /// <summary>
        /// Determines whether the requested file is safe to download.
        /// </summary>
        SafeFile IsSafeToDownload(string folderPath, string requestedFileName);
    }

    /// <summary>
    /// Safe File
    /// </summary>
    public class SafeFile
    {
        /// <summary>
        /// Determines whether the requested file is safe to download.
        /// </summary>
        public bool IsSafeToDownload { get; set; }

        /// <summary>
        /// Cleaned requested file's name.
        /// </summary>
        public string SafeFileName { get; set; } = string.Empty;

        /// <summary>
        /// Cleaned requested file's path.
        /// </summary>
        public string SafeFilePath { get; set; } = string.Empty;
    }

    /// <summary>
    /// SafeFile Download Service
    /// </summary>
    public class FileNameSanitizerService : IFileNameSanitizerService
    {
        private readonly ILogger<FileNameSanitizerService> _logger;

        /// <summary>
        /// SafeFile Download Service
        /// </summary>
        public FileNameSanitizerService(ILogger<FileNameSanitizerService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Determines whether the requested file is safe to download.
        /// </summary>
        public SafeFile IsSafeToDownload(string folderPath, string requestedFileName)
        {
            if (string.IsNullOrWhiteSpace(requestedFileName))
            {
                return new SafeFile();
            }

            var fileName = Path.GetFileName(requestedFileName);
            if (fileName != requestedFileName)
            {
                _logger.LogWarning(
                    $"Bad file request. Sanitized file name is different than the actual name. `{fileName}` != `{requestedFileName}`");
                return new SafeFile();
            }

            var filePath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"Requested file not found: `{filePath}`");
                return new SafeFile();
            }

            if (isOutsideOfRootPath(filePath, folderPath))
            {
                _logger.LogWarning(
                    $"Bad file request. The requested file path `{filePath}` is outside of the root path `{folderPath}`.");
                return new SafeFile();
            }

            return new SafeFile {IsSafeToDownload = true, SafeFileName = fileName, SafeFilePath = filePath};
        }

        private static bool isOutsideOfRootPath(string filePath, string folder)
        {
            return !filePath.StartsWith(folder, StringComparison.OrdinalIgnoreCase);
        }
    }
}
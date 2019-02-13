using System;
using System.Security.Cryptography;
using System.Text;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Dependency;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Web.Security
{
    /// <summary>
    /// Protection Provider Service
    /// More info: http://www.dotnettips.info/post/2519
    /// </summary>
    public class ProtectionProvider : IProtectionProvider, ISingletonDependency
    {
        private readonly ILogger<ProtectionProvider> _logger;
        private readonly IDataProtector _dataProtector;

        public ProtectionProvider(
            IDataProtectionProvider dataProtectionProvider,
            ILogger<ProtectionProvider> logger)
        {
            if (dataProtectionProvider == null)
            {
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataProtector = dataProtectionProvider.CreateProtector(typeof(ProtectionProvider).FullName);
        }

        public string Decrypt(string inputText)
        {
            if (inputText == null)
            {
                throw new ArgumentNullException(nameof(inputText));
            }

            try
            {
                var inputBytes = Convert.FromBase64String(inputText);
                var bytes = _dataProtector.Unprotect(inputBytes);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex.Message, "Invalid base 64 string. Fall through.");
            }
            catch (CryptographicException ex)
            {
                _logger.LogError(ex.Message, "Invalid protected payload. Fall through.");
            }

            return null;
        }

        public string Encrypt(string inputText)
        {
            if (inputText == null)
            {
                throw new ArgumentNullException(nameof(inputText));
            }

            var inputBytes = Encoding.UTF8.GetBytes(inputText);
            var bytes = _dataProtector.Protect(inputBytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
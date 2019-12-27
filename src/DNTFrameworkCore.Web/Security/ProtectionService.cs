using System;
using System.Security.Cryptography;
using System.Text;
using DNTFrameworkCore.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Web.Security
{
    internal sealed class ProtectionService : IProtectionService
    {
        private readonly ILogger<ProtectionService> _logger;
        private readonly IDataProtector _dataProtector;

        public ProtectionService(
            IDataProtectionProvider provider,
            ILogger<ProtectionService> logger)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataProtector = provider.CreateProtector(typeof(ProtectionService).FullName);
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
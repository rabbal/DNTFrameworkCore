using System;
using System.Security.Cryptography;
using System.Text;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Cryptography
{
    public interface ISecurityService
    {
        Guid NewSecureGuid();
        string ComputeSha256Hash(string input);
    }

    public class SecurityService : ISecurityService, ISingletonDependency
    {
        private readonly RandomNumberGenerator _rand = RandomNumberGenerator.Create();
        
        public Guid NewSecureGuid()
        {
            var bytes = new byte[16];
            _rand.GetBytes(bytes);
            return new Guid(bytes);
        }

        public string ComputeSha256Hash(string input)
        {
            using (var hashAlgorithm = SHA256.Create())
            {
                var byteValue = Encoding.UTF8.GetBytes(input);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
            }
        }

    }
}
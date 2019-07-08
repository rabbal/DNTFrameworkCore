using System.Collections.Generic;
using System.Security.Cryptography;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Licensing
{
    public class LicenseKey : ValueObject
    {
        private LicenseKey()
        {
        }

        public string PublicKey { get; private set; }
        public string PrivateKey { get; private set; }

        public static LicenseKey New(int dwKeySize = 1024)
        {
            using (var provider = new RSACryptoServiceProvider(dwKeySize))
            {
                return new LicenseKey
                {
                    PublicKey = provider.ToXml(false),
                    PrivateKey = provider.ToXml(true)
                };
            }
        }

        protected override IEnumerable<object> EqualityValues
        {
            get
            {
                yield return PublicKey;
                yield return PrivateKey;
            }
        }
    }
}
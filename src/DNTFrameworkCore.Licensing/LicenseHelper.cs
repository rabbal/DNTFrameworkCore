using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DNTFrameworkCore.Licensing
{
    public static class LicenseHelper
    {
        public static string CreateLicense<T>(string licensePrivateKey, T licenseData) where T : class
        {
            using (var provider = RSA.Create())
            {
                provider.FromXml(licensePrivateKey);
                var xmlDocument = CreateXmlDocument(licenseData);
                var xmlDigitalSignature = GetXmlDigitalSignature(xmlDocument, provider);
                AppendDigitalSignature(xmlDocument, xmlDigitalSignature);
                return XmlDocumentToString(xmlDocument);
            }
        }

        public static string CreateRsaKeyPair(int dwKeySize = 1024)
        {
            using (var provider = new RSACryptoServiceProvider(dwKeySize))
            {
                return provider.ToXml(true);
            }
        }

        /// <summary>
        /// generate keys pair separately as string.
        /// Item1 Contains PublicKey and Item2 Contains PrivateKey
        /// </summary>
        public static Tuple<string, string> CreateRsaKeys(int dwKeySize = 1024)
        {
            using (var provider = new RSACryptoServiceProvider(dwKeySize))
            {
                return new Tuple<string, string>(provider.ToXml(false), provider.ToXml(true));
            }
        }
        public static T ReadLicense<T>(string licensePublicKey, string xmlFileContent) where T : class
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlFileContent);

            using (var provider = new RSACryptoServiceProvider())
            {
                provider.FromXml(licensePublicKey);

                var nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("sig", "http://www.w3.org/2000/09/xmldsig#");

                var xml = new SignedXml(doc);
                var signatureNode = (XmlElement)doc.SelectSingleNode("//sig:Signature", nsmgr);
                if (signatureNode == null)
                    throw new InvalidOperationException("This license file is not signed.");

                xml.LoadXml(signatureNode);
                if (!xml.CheckSignature(provider))
                    throw new InvalidOperationException("This license file is not valid.");

                var ourXml = xml.GetXml();
                if (ourXml.OwnerDocument?.DocumentElement == null)
                    throw new InvalidOperationException("This license file is coruppted.");

                using (var reader = new XmlNodeReader(ourXml.OwnerDocument.DocumentElement))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(reader);
                }
            }
        }

        private static void AppendDigitalSignature(XmlDocument xmlDocument, XmlNode xmlDigitalSignature)
        {
            xmlDocument.DocumentElement?.AppendChild(xmlDocument.ImportNode(xmlDigitalSignature, true));
        }

        private static XmlDocument CreateXmlDocument<T>(T licenseData) where T : class
        {
            var serializer = new XmlSerializer(licenseData.GetType());
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                var ns = new XmlSerializerNamespaces(); ns.Add("", "");
                serializer.Serialize(writer, licenseData, ns);
                var doc = new XmlDocument();
                doc.LoadXml(sb.ToString());
                return doc;
            }
        }

        private static XmlElement GetXmlDigitalSignature(XmlDocument xmlDocument, AsymmetricAlgorithm key)
        {
            var xml = new SignedXml(xmlDocument) { SigningKey = key };
            var reference = new Reference { Uri = "" };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            xml.AddReference(reference);
            xml.ComputeSignature();
            return xml.GetXml();
        }

        private static string XmlDocumentToString(XmlDocument xmlDocument)
        {
            using (var ms = new MemoryStream())
            {
                var settings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
                var xmlWriter = XmlWriter.Create(ms, settings);
                xmlDocument.Save(xmlWriter);
                ms.Position = 0;
                return new StreamReader(ms).ReadToEnd();
            }
        }
    }
}
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DNTFrameworkCore.Licensing
{
    public static class XmlExtensions
    {
        public static void SignXml(this XmlDocument document, string privateKey)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (string.IsNullOrWhiteSpace(privateKey)) throw new ArgumentNullException(nameof(privateKey));
            
            using (var provider = RSA.Create())
            {
                provider.FromXml(privateKey);

                var xml = new SignedXml(document) {SigningKey = provider};
                var reference = new Reference {Uri = ""};
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                xml.AddReference(reference);
                xml.ComputeSignature();
                var signature = xml.GetXml();

                document.DocumentElement?.AppendChild(document.ImportNode(signature, true));
            }
        }

        public static XmlDocument ToXmlDocument<T>(this T value) where T : class
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var serializer = new XmlSerializer(value.GetType());
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                serializer.Serialize(writer, value, ns);
                var doc = new XmlDocument();
                doc.LoadXml(sb.ToString());
                return doc;
            }
        }

        public static string ToXmlString(this XmlDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            using (var ms = new MemoryStream())
            {
                var settings = new XmlWriterSettings {Indent = true, Encoding = Encoding.UTF8};
                var xmlWriter = XmlWriter.Create(ms, settings);
                document.Save(xmlWriter);
                ms.Position = 0;
                return new StreamReader(ms).ReadToEnd();
            }
        }
    }
}
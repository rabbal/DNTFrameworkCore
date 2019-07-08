using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Licensing
{
    public class License : IXmlSerializable
    {
        private readonly Dictionary<string, string> _attributes;
        private readonly List<LicenseFeature> _features;
        private XmlDocument _document;
        private bool _signed;

        private License()
        {
            _features = new List<LicenseFeature>();
            _attributes = new Dictionary<string, string>();
        }

        public Guid Id { get; private set; }
        public string ProductName { get; private set; }
        public string ProductVersion { get; private set; }
        public ExpirationTime ExpirationTime { get; private set; }
        public string CustomerName { get; private set; }
        public DateTime CreationTime { get; private set; }
        public string SerialNumber { get; private set; }

        public IReadOnlyCollection<LicenseFeature> Features => _features.AsReadOnly();
        public IReadOnlyDictionary<string, string> Attributes => _attributes.AsReadOnly();

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            Id = Guid.Parse(reader.GetAttribute(nameof(Id)) ??
                            throw new InvalidOperationException("This license is invalid."));

            ProductName = reader.GetAttribute(nameof(ProductName)) ??
                          throw new InvalidOperationException("This license is invalid.");

            ProductVersion = reader.GetAttribute(nameof(ProductVersion)) ??
                             throw new InvalidOperationException("This license is invalid.");

            CustomerName = reader.GetAttribute(nameof(CustomerName)) ??
                           throw new InvalidOperationException("This license is invalid.");

            SerialNumber = reader.GetAttribute(nameof(SerialNumber)) ??
                           throw new InvalidOperationException("This license is invalid.");

            CreationTime = DateTime.Parse(reader.GetAttribute(nameof(CreationTime)) ??
                                          throw new InvalidOperationException("This license is invalid."));

            ExpirationTime = ExpirationTime.Parse(reader.GetAttribute(nameof(ExpirationTime)) ??
                                                  throw new InvalidOperationException("This license is invalid."));

            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element) continue;

                switch (reader.Name)
                {
                    case "Feature":
                        var feature = LicenseFeature.New(reader.GetAttribute(nameof(LicenseFeature.Name)),
                            reader.GetAttribute(nameof(LicenseFeature.DisplayName)),
                            reader.GetAttribute(nameof(LicenseFeature.Value)),
                            reader.GetAttribute(nameof(LicenseFeature.Description)));
                        AddFeature(feature);
                        break;
                    case "Attribute":
                        AddAttribute(reader.GetAttribute("Name"), reader.Value);
                        break;
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));

            writer.WriteAttributeString(nameof(Id), Id.ToString());
            writer.WriteAttributeString(nameof(ProductName), ProductName);
            writer.WriteAttributeString(nameof(ProductVersion), ProductVersion);
            writer.WriteAttributeString(nameof(CustomerName), CustomerName);
            writer.WriteAttributeString(nameof(SerialNumber), SerialNumber);
            writer.WriteAttributeString(nameof(CreationTime), CreationTime.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString(nameof(ExpirationTime), ExpirationTime.ToString());

            if (_features.Count > 0)
            {
                writer.WriteStartElement(nameof(Features));

                foreach (var feature in _features)
                {
                    writer.WriteStartElement("Feature");

                    writer.WriteAttributeString(nameof(LicenseFeature.Name), feature.Name);
                    writer.WriteAttributeString(nameof(LicenseFeature.Value), feature.Value);
                    writer.WriteAttributeString(nameof(LicenseFeature.DisplayName), feature.DisplayName);

                    if (!string.IsNullOrWhiteSpace(feature.Description))
                    {
                        writer.WriteElementString(nameof(LicenseFeature.Description), feature.Description);
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            if (_attributes.Count <= 0) return;

            writer.WriteStartElement(nameof(Attributes));

            foreach (var attribute in _attributes)
            {
                writer.WriteStartElement("Attribute");

                writer.WriteAttributeString("Name", attribute.Key);
                writer.WriteString(attribute.Key);

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public static License New(string productName, string productVersion, string customerName,
            string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(productName)) throw new ArgumentNullException(nameof(productName));
            if (string.IsNullOrWhiteSpace(productVersion)) throw new ArgumentNullException(nameof(productVersion));
            if (string.IsNullOrWhiteSpace(customerName)) throw new ArgumentNullException(nameof(customerName));
            if (string.IsNullOrWhiteSpace(serialNumber)) throw new ArgumentNullException(nameof(serialNumber));

            var license = new License
            {
                Id = Guid.NewGuid(),
                CreationTime = DateTime.UtcNow,
                ProductName = productName,
                ProductVersion = productVersion,
                CustomerName = customerName,
                ExpirationTime = ExpirationTime.Infinite,
                SerialNumber = serialNumber
            };

            return license;
        }

        public Result AddFeature(LicenseFeature feature)
        {
            if (feature == null) throw new ArgumentNullException(nameof(feature));

            if (_signed) Result.Fail("This license already is signed. It is impossible to add new feature.");

            if (_features.Contains(feature)) return Result.Fail($"A feature with name {feature.Name} already exists.");

            _features.Add(feature);

            return Result.Ok();
        }

        public Result AddAttribute(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

            if (_signed) return Result.Fail("This license already is signed. It is impossible to add new attribute.");

            if (_attributes.ContainsKey(name)) return Result.Fail($"An attribute with name {name} already exists.");

            _attributes.Add(name, value);

            return Result.Ok();
        }

        public Result ExpireAt(ExpirationTime time)
        {
            if (time == null) throw new ArgumentNullException(nameof(time));

            if (_signed) return Result.Fail("This license already is signed. It is impossible to set expirationTime.");

            if (time.Expired) return Result.Fail("ExpirationTime should not be expired.");

            ExpirationTime = time;

            return Result.Ok();
        }

        public static Result<License> FromXmlFile(string licensePublicKey, string xmlFilePath)
        {
            if (string.IsNullOrWhiteSpace(licensePublicKey)) throw new ArgumentNullException(nameof(licensePublicKey));
            if (string.IsNullOrWhiteSpace(xmlFilePath)) throw new ArgumentNullException(nameof(xmlFilePath));

            var content = File.ReadAllText(xmlFilePath);
            return FromXmlString(licensePublicKey, content);
        }

        public static Result<License> FromXmlString(string licensePublicKey, string xmlFileContent)
        {
            if (string.IsNullOrWhiteSpace(xmlFileContent)) throw new ArgumentNullException(nameof(xmlFileContent));
            if (string.IsNullOrWhiteSpace(licensePublicKey)) throw new ArgumentNullException(nameof(licensePublicKey));

            var doc = new XmlDocument();
            doc.LoadXml(xmlFileContent);

            using (var provider = RSA.Create())
            {
                provider.FromXml(licensePublicKey);

                var manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("sig", "http://www.w3.org/2000/09/xmldsig#");

                var xml = new SignedXml(doc);
                var signatureNode = (XmlElement) doc.SelectSingleNode("//sig:Signature", manager);
                if (signatureNode == null)
                    return Result.Fail<License>("This license file is not signed.");

                xml.LoadXml(signatureNode);
                if (!xml.CheckSignature(provider))
                    return Result.Fail<License>("This license file is invalid.");

                var ourXml = xml.GetXml();
                if (ourXml.OwnerDocument?.DocumentElement == null)
                    return Result.Fail<License>("This license file is corrupted.");

                using (var reader = new XmlNodeReader(ourXml.OwnerDocument.DocumentElement))
                {
                    var xmlSerializer = new XmlSerializer(typeof(License));
                    var license = (License) xmlSerializer.Deserialize(reader);

                    license._document = doc;
                    license._signed = true;

                    return Result.Ok(license);
                }
            }
        }

        public Result Sign(string licensePrivateKey)
        {
            if (string.IsNullOrWhiteSpace(licensePrivateKey))
                throw new ArgumentNullException(nameof(licensePrivateKey));

            if (_signed) return Result.Fail("This license already is signed.");

            _document = this.ToXmlDocument();
            _document.SignXml(licensePrivateKey);
            _signed = true;

            return Result.Ok();
        }

        public Result Verify(ILicensedProduct licensedProduct)
        {
            if (licensedProduct == null) throw new ArgumentNullException(nameof(licensedProduct));

            if (!_signed) return Result.Fail("This license already is not signed.");

            if (ExpirationTime.Expired) return Result.Fail("This license is expired.");

            if (!ProductVersion.Equals(licensedProduct.ProductVersion, StringComparison.Ordinal))
                return Result.Fail("This license is not for this product.");

            if (!SerialNumber.Equals(licensedProduct.SerialNumber, StringComparison.Ordinal))
                return Result.Fail("This license is not for this machine.");

            return Result.Ok();
        }

        public Result WriteToXmlFile(string xmlFilePath)
        {
            if (string.IsNullOrWhiteSpace(xmlFilePath)) throw new ArgumentNullException(nameof(xmlFilePath));

            var result = ToXmlString();
            if (result.Failed) return result;

            File.WriteAllText(xmlFilePath, result.Value);

            return Result.Ok();
        }

        public Result<string> ToXmlString()
        {
            if (!_signed) return Result.Fail<string>("This license already is not signed.");

            var xml = _document.ToXmlString();
            return Result.Ok(xml);
        }

        public override string ToString()
        {
            return $"License: {Id}";
        }
    }
}
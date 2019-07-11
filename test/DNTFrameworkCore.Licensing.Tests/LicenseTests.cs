using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Shouldly;

namespace DNTFrameworkCore.Licensing.Tests
{
    [TestFixture]
    public class LicenseTests
    {
        [Test]
        public void Should_Create_New_License()
        {
            //Arrange, Act
            var license = License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub",
                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");

            //Assert
            license.ShouldNotBeNull();
            license.ExpirationTime.ShouldBe(ExpirationTime.Infinite);
            license.Id.ShouldNotBe(default);
            license.CreationTime.ShouldNotBe(default);
            license.ProductName.ShouldBe("DNTFrameworkCore");
            license.ProductVersion.ShouldBe("1.1.1-beta");
            license.CustomerName.ShouldBe("GitHub");
            license.SerialNumber.ShouldBe("4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");
            license.Attributes.ShouldBeEmpty();
            license.Features.ShouldBeEmpty();
        }

        [Test]
        public void Should_Not_Create_New_With_Empty_ProductName()
        {
            var exception1 = Assert.Throws<ArgumentNullException>(() =>
                License.New(null, "1.1.1-beta", "GitHub", "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9"));

            var exception2 = Assert.Throws<ArgumentNullException>(() =>
                License.New(string.Empty, "1.1.1-beta", "GitHub", "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9"));

            exception1.ShouldNotBeNull();
            exception2.ShouldNotBeNull();
            exception1.ParamName.ShouldBe("productName");
            exception2.ParamName.ShouldBe("productName");
        }

        [Test]
        public void Should_Not_Create_New_With_Empty_ProductVersion()
        {
            var exception1 = Assert.Throws<ArgumentNullException>(() =>
                License.New("DNTFrameworkCore", null, "GitHub", "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9"));

            var exception2 = Assert.Throws<ArgumentNullException>(() =>
                License.New("DNTFrameworkCore", string.Empty, "GitHub",
                    "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9"));

            exception1.ShouldNotBeNull();
            exception2.ShouldNotBeNull();
            exception1.ParamName.ShouldBe("productVersion");
            exception2.ParamName.ShouldBe("productVersion");
        }

        [Test]
        public void Should_Not_Create_New_With_Empty_CustomerName()
        {
            var exception1 = Assert.Throws<ArgumentNullException>(() =>
                License.New("DNTFrameworkCore", "1.1.1-beta", null,
                    "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9"));

            var exception2 = Assert.Throws<ArgumentNullException>(() =>
                License.New("DNTFrameworkCore", "1.1.1-beta", string.Empty,
                    "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9"));

            exception1.ShouldNotBeNull();
            exception2.ShouldNotBeNull();
            exception1.ParamName.ShouldBe("customerName");
            exception2.ParamName.ShouldBe("customerName");
        }

        [Test]
        public void Should_Not_Create_New_With_Empty_SerialNumber()
        {
            var exception1 = Assert.Throws<ArgumentNullException>(() =>
                License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub", null));

            var exception2 = Assert.Throws<ArgumentNullException>(() =>
                License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub", string.Empty));

            exception1.ShouldNotBeNull();
            exception2.ShouldNotBeNull();
            exception1.ParamName.ShouldBe("serialNumber");
            exception2.ParamName.ShouldBe("serialNumber");
        }

        [Test]
        public void Should_Add_New_Attribute()
        {
            var license = License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub",
                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");

            var result = license.AddAttribute("attributeName", "attributeValue");

            result.Failed.ShouldBeFalse();
            license.Attributes.ContainsKey("attributeName").ShouldBeTrue();
        }

        [Test]
        public void Should_Not_Add_New_Attribute_With_Empty_Name()
        {
            var license = License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub",
                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");

            var exception1 =
                Assert.Throws<ArgumentNullException>(() => license.AddAttribute(null, "attributeValue"));
            var exception2 =
                Assert.Throws<ArgumentNullException>(() => license.AddAttribute(string.Empty, "attributeValue"));

            exception1.ShouldNotBeNull();
            exception2.ShouldNotBeNull();
            exception1.ParamName.ShouldBe("name");
            exception2.ParamName.ShouldBe("name");
        }

        [Test]
        public void Should_Not_Add_New_Attribute_With_Empty_Value()
        {
            var license = License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub",
                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");

            var exception1 =
                Assert.Throws<ArgumentNullException>(() => license.AddAttribute("attributeName", null));
            var exception2 =
                Assert.Throws<ArgumentNullException>(() => license.AddAttribute("attributeName", string.Empty));

            exception1.ShouldNotBeNull();
            exception2.ShouldNotBeNull();
            exception1.ParamName.ShouldBe("value");
            exception2.ParamName.ShouldBe("value");
        }

        [Test]
        public void Should_Not_Add_New_Attribute_When_Signed()
        {
            var content = ReadLicense("DNTFrameworkCore.Licensing.Tests.License.lic");

            var license = License.FromString(LicensingKeys.PublicKey, content);

            var exception =
                Assert.Throws<InvalidOperationException>(() => license.AddAttribute("attributeName", "attributeValue"));
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("This license already is signed.");
        }

        [Test]
        public void Should_Not_Add_Duplicate_Attribute()
        {
            var license = License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub",
                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");

            var attributeResult1 = license.AddAttribute("attributeName", "attributeValue");
            attributeResult1.Failed.ShouldBeFalse();
            license.Attributes.ContainsKey("attributeName").ShouldBeTrue();

            var attributeResult2 = license.AddAttribute("attributeName", "attributeValue");
            attributeResult2.Failed.ShouldBeTrue();
            attributeResult2.Message.ShouldBe($"An attribute with name attributeName already exists.");
        }

        [Test]
        public void Should_Sign_License()
        {
            var license = License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub",
                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");

            license.AddAttribute("AttributeName1", "AttributeValue1");
            license.AddFeature(LicenseFeature.New("Feature1", "Feature1DisplayName", 100.ToString()));

            license.Signed.ShouldBeFalse();

            license.Sign(LicensingKeys.PrivateKey);
            license.Signed.ShouldBeTrue();
            var licenseText = license.ToString();
            licenseText.ShouldContain("Signature");
            licenseText.ShouldContain("SerialNumber=\"4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9\"");
            licenseText.ShouldContain("Name=\"AttributeName1\"");
            licenseText.ShouldContain("Name=\"Feature1\"");
        }

        [Test]
        public void Should_Not_Sign_License_When_Already_Signed()
        {
            var content = ReadLicense("DNTFrameworkCore.Licensing.Tests.License.lic");

            var license = License.FromString(LicensingKeys.PublicKey, content);

            license.Signed.ShouldBeTrue();

            var exception = Assert.Throws<InvalidOperationException>(() => license.Sign(LicensingKeys.PrivateKey));
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("This license already is signed.");
        }

        [Test]
        public void Should_Load_FromString()
        {
            var content = ReadLicense("DNTFrameworkCore.Licensing.Tests.License.lic");

            var license = License.FromString(LicensingKeys.PublicKey, content);

            license.Id.ShouldBe(Guid.Parse("aac859ee-0f27-465e-ae00-7d7ae3e32948"));

            license.ExpirationTime.ShouldBe(ExpirationTime.Infinite);
        }

        [Test]
        public void Should_Verify()
        {
            var content = ReadLicense("DNTFrameworkCore.Licensing.Tests.License.lic");

            var license = License.FromString(LicensingKeys.PublicKey, content);

            license.Id.ShouldBe(Guid.Parse("aac859ee-0f27-465e-ae00-7d7ae3e32948"));

            var result =
                license.Verify(new LicensedProduct("1.1.1-beta",
                    "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9", "DNTFrameworkCore"));
            result.Failed.ShouldBeFalse();
        }

        [Test]
        public void Should_Not_ToString_When_Is_Not_Signed()
        {
            var license = License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub",
                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");

            ActualValueDelegate<string> licenseText = () => license.ToString();
            Assert.That(licenseText, Throws.TypeOf<InvalidOperationException>(), "This license already is not signed.");
        }

        [Test]
        public void Should_Not_Verify_With_Invalid_ProductVersion()
        {
            var content = ReadLicense("DNTFrameworkCore.Licensing.Tests.License.lic");

            var license = License.FromString(LicensingKeys.PublicKey, content);

            license.Id.ShouldBe(Guid.Parse("aac859ee-0f27-465e-ae00-7d7ae3e32948"));

            var result =
                license.Verify(new LicensedProduct("1.1.2-beta",
                    "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9", "DNTFrameworkCore"));
            result.Failed.ShouldBeTrue();
            result.Message.ShouldBe("This license is not for this product.");
        }

        [Test]
        public void Should_Not_Verify_With_Invalid_SerialNumber()
        {
            var content = ReadLicense("DNTFrameworkCore.Licensing.Tests.License.lic");

            var license = License.FromString(LicensingKeys.PublicKey, content);

            license.Id.ShouldBe(Guid.Parse("aac859ee-0f27-465e-ae00-7d7ae3e32948"));

            var result =
                license.Verify(new LicensedProduct("1.1.1-beta",
                    "4876-8DB5-EE85-69D3-FE52-8CF7-395D", "DNTFrameworkCore"));
            result.Failed.ShouldBeTrue();
            result.Message.ShouldBe("This license is not for this machine.");
        }
        
        [Test]
        public void Should_Not_Verify_With_Invalid_ProductName()
        {
            var content = ReadLicense("DNTFrameworkCore.Licensing.Tests.License.lic");

            var license = License.FromString(LicensingKeys.PublicKey, content);

            license.Id.ShouldBe(Guid.Parse("aac859ee-0f27-465e-ae00-7d7ae3e32948"));

            var result =
                license.Verify(new LicensedProduct("1.1.1-beta",
                    "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9", "InvalidProductName"));
            result.Failed.ShouldBeTrue();
            result.Message.ShouldBe("This license is not for this product.");
        }

        private static string ReadLicense(string filename)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var stream = thisAssembly.GetManifestResourceStream(filename);
            return new StreamReader(stream).ReadToEnd();
        }
    }
}
using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.Licensing.Tests
{
    [TestFixture]
    public class LicenseTests
    {
        [Test]
        public void Should_Create_New_License_Without_Attribute_Without_Feature()
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
        public void Should_Sign_License()
        {
            var license = License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub",
                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");

            var result = license.Sign(LicensingKeys.PrivateKey);
            result.Failed.ShouldBeFalse();

            var xmlResult = license.ToXmlString();
            xmlResult.Failed.ShouldBeFalse();
            xmlResult.Value.ShouldContain("Signature");
        }

        [Test]
        public void Should_Load_FromXmlString()
        {
            var xmlFileContent = ReadFileContent("DNTFrameworkCore.Licensing.Tests.License.lic");

            var licenseResult = License.FromXmlString(LicensingKeys.PublicKey, xmlFileContent);
            
            licenseResult.Failed.ShouldBeFalse();
            licenseResult.Value.Id.ShouldBe(Guid.Parse("f4067c3b-3e56-472f-9576-341d73501666"));
        }
        
        [Test]
        public void Should_Verify()
        {
            var xmlFileContent = ReadFileContent("DNTFrameworkCore.Licensing.Tests.License.lic");

            var licenseResult = License.FromXmlString(LicensingKeys.PublicKey, xmlFileContent);
            
            licenseResult.Failed.ShouldBeFalse();
            licenseResult.Value.Id.ShouldBe(Guid.Parse("f4067c3b-3e56-472f-9576-341d73501666"));

            var result = licenseResult.Value.Verify(new LicensedProduct("1.1.1-beta", "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9"));
            result.Failed.ShouldBeFalse();
        }

        [Test]
        public void Should_Not_Verify_With_Invalid_ProductVersion()
        {
            var xmlFileContent = ReadFileContent("DNTFrameworkCore.Licensing.Tests.License.lic");

            var licenseResult = License.FromXmlString(LicensingKeys.PublicKey, xmlFileContent);
            
            licenseResult.Failed.ShouldBeFalse();
            licenseResult.Value.Id.ShouldBe(Guid.Parse("f4067c3b-3e56-472f-9576-341d73501666"));

            var result = licenseResult.Value.Verify(new LicensedProduct("1.1.2-beta", "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9"));
            result.Failed.ShouldBeTrue();
            result.Message.ShouldBe("This license is not for this product.");
        }
        
        public static string ReadFileContent(string filename)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var stream = thisAssembly.GetManifestResourceStream(filename);
            return new StreamReader(stream).ReadToEnd();
        }

//        [Test]
//        public void Should_Not_Create_New_With_Empty_ProductName()
//        {
//            var licenseResult1 = License.New(null, "1.1.1-beta", "GitHub", "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");
//            var licenseResult2 = License.New(string.Empty, "1.1.1-beta", "GitHub",
//                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");
//
//            licenseResult1.Failed.ShouldBeTrue();
//            licenseResult1.Message.ShouldBe("ProductName should not be empty");
//
//            licenseResult2.Failed.ShouldBeTrue();
//            licenseResult2.Message.ShouldBe("ProductName should not be empty");
//        }
//
//        [Test]
//        public void Should_Not_Create_New_With_Empty_ProductVersion()
//        {
//            var licenseResult1 =
//                License.New("DNTFrameworkCore", null, "GitHub", "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");
//            var licenseResult2 = License.New("DNTFrameworkCore", string.Empty, "GitHub",
//                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");
//
//            licenseResult1.Failed.ShouldBeTrue();
//            licenseResult1.Message.ShouldBe("ProductVersion should not be empty");
//
//            licenseResult2.Failed.ShouldBeTrue();
//            licenseResult2.Message.ShouldBe("ProductVersion should not be empty");
//        }
//
//        [Test]
//        public void Should_Not_Create_New_With_Empty_CustomerName()
//        {
//            var licenseResult1 = License.New("DNTFrameworkCore", "1.1.1-beta", null,
//                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");
//            var licenseResult2 = License.New("DNTFrameworkCore", "1.1.1-beta", string.Empty,
//                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");
//
//            licenseResult1.Failed.ShouldBeTrue();
//            licenseResult1.Message.ShouldBe("CustomerName should not be empty");
//
//            licenseResult2.Failed.ShouldBeTrue();
//            licenseResult2.Message.ShouldBe("CustomerName should not be empty");
//        }
//
//        [Test]
//        public void Should_Not_Create_New_With_Empty_SerialNumber()
//        {
//            var licenseResult1 = License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub", null);
//            var licenseResult2 = License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub", string.Empty);
//
//            licenseResult1.Failed.ShouldBeTrue();
//            licenseResult1.Message.ShouldBe("SerialNumber should not be empty");
//
//            licenseResult2.Failed.ShouldBeTrue();
//            licenseResult2.Message.ShouldBe("SerialNumber should not be empty");
//        }
//
//        [Test]
//        public void Should_Add_New_Attribute()
//        {
//            var licenseResult = License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub",
//                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");
//
//            licenseResult.Failed.ShouldBeFalse();
//            var license = licenseResult.Value;
//
//            var result = license.AddAttribute("attributeName", "attributeValue");
//
//            result.Failed.ShouldBeFalse();
//            license.Attributes.ContainsKey("attributeName").ShouldBeTrue();
//        }
//
//        [Test]
//        public void Should_Not_Add_New_Attribute_When_Signed()
//        {
//            var licenseResult = License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub",
//                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");
//
//            licenseResult.Failed.ShouldBeFalse();
//            var license = licenseResult.Value;
//
//            var signResult = license.Sign(LicensingKeys.PrivateKey);
//            signResult.Failed.ShouldBeFalse();
//
//            var result = license.AddAttribute("attributeName", "attributeValue");
//            result.Failed.ShouldBeTrue();
//            result.Message.ShouldBe("This license already is signed. It is impossible to add new attribute.");
//            license.Attributes.ContainsKey("attributeName").ShouldBeFalse();
//        }
//
//        [Test]
//        public void Should_Not_Add_Duplicate_Attribute()
//        {
//            var licenseResult = License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub",
//                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");
//            licenseResult.Failed.ShouldBeFalse();
//            var license = licenseResult.Value;
//
//            var attributeResult1 = license.AddAttribute("attributeName", "attributeValue");
//            attributeResult1.Failed.ShouldBeFalse();
//            license.Attributes.ContainsKey("attributeName").ShouldBeTrue();
//
//            var attributeResult2 = license.AddAttribute("attributeName", "attributeValue");
//            attributeResult2.Failed.ShouldBeTrue();
//            attributeResult2.Message.ShouldBe($"An attribute with name attributeName already exists.");
//        }
//
//        [Test]
//        public void Should_Sign_License()
//        {
//            var licenseResult = License.New("DNTFrameworkCore", "1.1.1-beta", "GitHub",
//                "4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9");
//
//            licenseResult.Failed.ShouldBeFalse();
//            var license = licenseResult.Value;
//
//            var result = license.Sign(LicensingKeys.PrivateKey);
//            result.Failed.ShouldBeFalse();
//
//            var xmlResult = license.ToXmlString();
//            xmlResult.Failed.ShouldBeFalse();
//            xmlResult.Value.ShouldContain("Signature");
//        }
    }
}
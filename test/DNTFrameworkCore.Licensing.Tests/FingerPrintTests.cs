using System.Text;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.Licensing.Tests
{
    [TestFixture]
    public class FingerPrintTests
    {
        [Test]
        public void Should_Create_16_Bytes_FingerPrint()
        {
            var value = FingerPrint.Value;
            value.ShouldNotBeNull();
        }
    }
}
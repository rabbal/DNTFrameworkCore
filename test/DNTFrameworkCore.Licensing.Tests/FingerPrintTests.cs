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
            var value = FingerPrint.Value(FingerPrintHardwares.Cpu | FingerPrintHardwares.Bios |
                                          FingerPrintHardwares.Disk | FingerPrintHardwares.Mac |
                                          FingerPrintHardwares.Motherboard | FingerPrintHardwares.Video);
            value.ShouldNotBeNull();
        }
    }
}
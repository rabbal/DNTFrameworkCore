using System;
using System.Collections.Generic;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace DNTFrameworkCore.Licensing
{
    /// <summary>
    /// Generates a 16 byte Unique Identification code of a computer
    /// Example: 4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9
    /// </summary>
    public static class FingerPrint
    {
        private static string _value = string.Empty;

        public static string Value
        {
            get
            {
                if (!string.IsNullOrEmpty(_value)) return _value;

                _value = ComputeHash(
                    $"CPU >> {CpuId()} \nBIOS >> {BiosId()} \nBASE >> {MotherboardId()} \nDISK >> {DiskId()} \nVIDEO >> {VideoId()} \nMAC >> {MacId()}");

                return _value;
            }
        }

        private static string ComputeHash(string s)
        {
            using (var md5 = MD5.Create())
            {
                var bt = Encoding.ASCII.GetBytes(s);
                return ToHexString(md5.ComputeHash(bt));
            }
        }

        private static string ToHexString(IReadOnlyList<byte> bytes)
        {
            var s = string.Empty;
            for (var i = 0; i < bytes.Count; i++)
            {
                var b = bytes[i];
                var n = (int) b;
                var n1 = n & 15;
                var n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char) (n2 - 10 + 'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char) (n1 - 10 + 'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bytes.Count && (i + 1) % 2 == 0) s += "-";
            }

            return s;
        }

        private static string CpuId()
        {
            return HardwareQuery("Win32_processor", "ProcessorId");
        }

        private static string BiosId()
        {
            return HardwareQuery("Win32_BIOS", "Manufacturer")
                   + HardwareQuery("Win32_BIOS", "SMBIOSBIOSVersion")
                   + HardwareQuery("Win32_BIOS", "IdentificationCode")
                   + HardwareQuery("Win32_BIOS", "SerialNumber")
                   + HardwareQuery("Win32_BIOS", "ReleaseDate")
                   + HardwareQuery("Win32_BIOS", "Version");
        }

        private static string MotherboardId()
        {
            return HardwareQuery("Win32_BaseBoard", "Model")
                   + HardwareQuery("Win32_BaseBoard", "Manufacturer")
                   + HardwareQuery("Win32_BaseBoard", "Name")
                   + HardwareQuery("Win32_BaseBoard", "SerialNumber");
        }

        private static string DiskId()
        {
            return HardwareQuery("Win32_DiskDrive", "Model")
                   + HardwareQuery("Win32_DiskDrive", "Manufacturer")
                   + HardwareQuery("Win32_DiskDrive", "Signature")
                   + HardwareQuery("Win32_DiskDrive", "TotalHeads");
        }

        private static string VideoId()
        {
            return HardwareQuery("Win32_VideoController", "DriverVersion")
                   + HardwareQuery("Win32_VideoController", "Name");
        }

        private static string MacId()
        {
            return HardwareQuery("Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'", "MACAddress");
        }

        private static string HardwareQuery(string hardware, string property,
            Predicate<ManagementBaseObject> predicate = null)
        {
            var queryString = $"Select {property} From {hardware}";

            using (var query = new ManagementObjectSearcher(queryString))
            {
                foreach (var item in query.Get())
                {
                    if (predicate == null || predicate.Invoke(item))
                        return item[property]?.ToString() ?? "None";
                }
            }

            throw new InvalidOperationException(queryString);
        }
    }
}
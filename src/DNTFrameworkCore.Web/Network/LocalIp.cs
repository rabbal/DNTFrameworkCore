using System;
using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.Web.Network
{
    /// <summary>
    /// IP Address Validation
    /// </summary>
    public static class LocalIp
    {
        /// <summary>
        /// ::1 or null-IP v6
        /// </summary>
        public const string NullIPv6 = "::1";

        /// <summary>
        /// Determines whether the specified `ip` address is a private IP address.
        /// </summary>
        public static bool IsLocalIp(this string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return false;
            }

            return ip.IsInSubnet("127.0.0.1/8") ||
                   ip.IsInSubnet("10.0.0.0/8") ||
                   ip.IsInSubnet("172.16.0.0/12") ||
                   ip.IsInSubnet("192.168.0.0/16") ||
                   ip.IsInSubnet("169.254.0.0/16 ");
        }

        /// <summary>
        /// IP Address Validation Using CIDR
        /// http://social.msdn.microsoft.com/forums/en-US/netfxnetcom/thread/29313991-8b16-4c53-8b5d-d625c3a861e1/
        /// </summary>
        public static bool IsInSubnet(this string ipAddress, string cidr)
        {
            var parts = cidr.Split('/');
            var baseAddress = BitConverter.ToInt32(IPAddress.Parse(parts[0]).GetAddressBytes(), 0);
            var address = BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes(), 0);
            var mask = IPAddress.HostToNetworkOrder(-1 << (32 - int.Parse(parts[1], CultureInfo.InvariantCulture)));
            return (baseAddress & mask) == (address & mask);
        }

        /// <summary>
        /// Identifies local requests
        /// </summary>
        public static bool IsLocal(this ConnectionInfo conn)
        {
            if (!conn.RemoteIpAddress.IsSet())
                return true;

            if (conn.LocalIpAddress.IsSet())
                return conn.RemoteIpAddress.Equals(conn.LocalIpAddress);

            return conn.RemoteIpAddress.IsLoopback();
        }

        /// <summary>
        /// Identifies local requests
        /// </summary>
        public static bool IsLocal(this HttpContext ctx)
        {
            return ctx.Connection.IsLocal();
        }

        /// <summary>
        /// Identifies local requests
        /// </summary>
        public static bool IsLocal(this HttpRequest req)
        {
            return req.HttpContext.IsLocal();
        }

        /// <summary>
        /// address != NullIPv6
        /// </summary>
        public static bool IsSet(this IPAddress address)
        {
            return address != null && address.ToString() != NullIPv6;
        }

        /// <summary>
        /// Indicates whether the specified IP address is the loopback address.
        /// </summary>
        public static bool IsLoopback(this IPAddress address)
        {
            return IPAddress.IsLoopback(address);
        }
    }
}
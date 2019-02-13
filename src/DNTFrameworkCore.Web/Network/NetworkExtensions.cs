using System;
using System.Net;
using System.Net.Sockets;

namespace DNTFrameworkCore.Web.Network
{
    /// <summary>
    /// Network Extensions
    /// </summary>
    public static class NetworkExtensions
    {
        /// <summary>
        /// Determines whether ex is a SocketException or WebException
        /// </summary>
        public static bool IsNetworkError(this Exception ex)
        {
            return ex is SocketException ||
                   ex is WebException ||
                   ex.InnerException != null && ex.InnerException.IsNetworkError();
        }
    }
}
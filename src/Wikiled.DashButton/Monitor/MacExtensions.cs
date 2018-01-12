using System;
using System.Net.NetworkInformation;

namespace Wikiled.DashButton.Monitor
{
    public static class MacExtensions
    {
        public static string GetMacName(this byte[] data)
        {
            return BitConverter.ToString(data);
        }

        public static string GetMacName(this PhysicalAddress address)
        {
            return GetMacName(address.GetAddressBytes());
        }
    }
}

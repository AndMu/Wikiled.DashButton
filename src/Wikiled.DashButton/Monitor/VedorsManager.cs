using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using Wikiled.Core.Standard.Arguments;

namespace Wikiled.DashButton.Monitor
{
    public class VedorsManager : IVedorsManager
    {
        private Dictionary<string, string> vendorNames;

        private VedorsManager()
        {
        }

        public static VedorsManager Load(string dataFile)
        {
            Guard.NotNullOrEmpty(() => dataFile, dataFile);
            VedorsManager vendor = new VedorsManager();
            vendor.vendorNames = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(dataFile));
            return vendor;
        }

        public VendorInfo ResolveVendor(PhysicalAddress macAddress)
        {
            var macAddrBytes = macAddress.GetAddressBytes().Take(3).ToArray();
            var identifier = BitConverter.ToString(macAddrBytes).Replace('-', ':');
            var mask = identifier.Replace(":", string.Empty);
            if (!vendorNames.TryGetValue(mask, out var name))
            {
                name = "Unknown";
            }

            return new VendorInfo(identifier, name);
        }
    }
}

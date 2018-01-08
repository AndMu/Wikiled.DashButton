using System.Net.NetworkInformation;

namespace Wikiled.DashButton.Monitor
{
    public class PacketInformation
    {
        public PacketInformation(VendorInfo vendor, PhysicalAddress mac)
        {
            Vendor = vendor;
            Mac = mac;
        }

        public VendorInfo Vendor { get; }

        public PhysicalAddress Mac { get; }

        public override string ToString()
        {
            return $"Packet: {Mac.GetMacName()} {Vendor}";
        }
    }
}

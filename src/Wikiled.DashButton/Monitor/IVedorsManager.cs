using System.Net.NetworkInformation;

namespace Wikiled.DashButton.Monitor
{
    public interface IVedorsManager
    {
        VendorInfo ResolveVendor(PhysicalAddress macAddress);
    }
}
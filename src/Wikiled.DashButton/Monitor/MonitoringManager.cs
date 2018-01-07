using System;
using System.Reactive.Subjects;
using NLog;
using PacketDotNet;
using SharpPcap;
using SharpPcap.WinPcap;
using Wikiled.Core.Standard.Arguments;

namespace Wikiled.DashButton.Monitor
{
    public class MonitoringManager
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private const int ReadTimeoutMilliseconds = 1000;

        private readonly IVedorsManager vendorManager;

        private Subject<PacketInformation> subscription;

        public MonitoringManager(IVedorsManager vendorManager)
        {
            Guard.NotNull(() => vendorManager, vendorManager);
            this.vendorManager = vendorManager;
        }

        public IObservable<PacketInformation> StartListening()
        {
            subscription?.Dispose();
            subscription = new Subject<PacketInformation>();
            var devices = CaptureDeviceList.Instance;
            if (devices.Count < 1)
            {
                throw new PcapMissingException("No interfaces found! Make sure WinPcap is installed.");
            }

            foreach (var device in devices)
            {
                ProcessDevice(device);
            }

            return subscription;
        }

        private void ProcessDevice(ICaptureDevice device)
        {
            var winpack = device as WinPcapDevice;
            if (device == null ||
                winpack?.Addresses.Count == 0)
            {
                return;
            }

            device.Open(DeviceMode.Promiscuous, ReadTimeoutMilliseconds);

            try
            {
                log.Info("Subscribing to: {0}-{1}", device.Description, device.MacAddress);
            }
            catch (Exception)
            {
                log.Info("Failed subscribption to: {0}", device.Description);
                return;
            }

            Subscribe(device);
            // tcpdump filter to capture only ARP Packets
            device.Filter = "arp";
            Action action = device.Capture;
            action.BeginInvoke(ar => action.EndInvoke(ar), null);
        }

        private void Subscribe(ICaptureDevice device)
        {
            device.OnPacketArrival += (sender, e) => OnArrived(device, e);
        }

        private void OnArrived(ICaptureDevice device, CaptureEventArgs e)
        {
            try
            {
                var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
                var eth = (EthernetPacket)packet;
                var dashMac = eth.SourceHwAddress.ToString();
                if (dashMac.Equals(device.MacAddress.ToString()))
                {
                    // ignore packets from our own device
                    return;
                }

                subscription.OnNext(new PacketInformation(vendorManager.ResolveVendor(eth.SourceHwAddress), eth.SourceHwAddress));
            }
            catch (Exception exception)
            {
                log.Error(exception);
            }
        }
    }
}

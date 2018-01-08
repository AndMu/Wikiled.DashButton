using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Wikiled.DashButton.Lights
{
    public class BridgeSetup
    {
        public async Task<LocatedBridge[]> Bridge()
        {
            IBridgeLocator locator = new HttpBridgeLocator();
            var bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
            foreach (var bridge in bridges)
            {
                ILocalHueClient client = new LocalHueClient(bridge.IpAddress);
                var appKey = await client.RegisterAsync("DashService", "DashHost");
            }
            
            return bridges.ToArray();
        }
    }
}

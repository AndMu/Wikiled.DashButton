using Wikiled.DashButton.Config;

namespace Wikiled.DashButton.Lights
{
    public class LightsManagerFactory : ILightsManagerFactory
    {
        public ILightsManager Construct(BridgeConfig config)
        {
            return new LightsManager(config);
        }
    }
}

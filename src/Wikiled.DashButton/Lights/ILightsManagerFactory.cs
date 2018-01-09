using Wikiled.DashButton.Config;

namespace Wikiled.DashButton.Lights
{
    public interface ILightsManagerFactory
    {
        ILightsManager Construct(BridgeConfig config);
    }
}
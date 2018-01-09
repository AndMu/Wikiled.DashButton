using System.Collections.Generic;

namespace Wikiled.DashButton.Config
{
    public class ServiceConfig
    {
        public Dictionary<string, ButtonConfig> Buttons { get; set; }

        public Dictionary<string, BridgeConfig> Bridges { get; set; }
    }
}

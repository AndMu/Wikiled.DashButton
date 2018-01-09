using System.Collections.Generic;

namespace Wikiled.DashButton.Config
{
    public class BridgeConfig
    {
        public string Id { get; set; }

        public string Ip { get; set; }

        public string AppKey { get; set; }

        public Dictionary<string, ButtonAction> ButtonAction { get; set; }
    }
}

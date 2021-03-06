﻿using System.Collections.Generic;

namespace Wikiled.DashButton.Config
{
    public class ServiceConfig
    {
        public ServiceConfig()
        {
            Buttons = new Dictionary<string, ButtonConfig>();
            Bridges = new Dictionary<string, BridgeConfig>();
        }

        public Dictionary<string, ButtonConfig> Buttons { get; set; }

        public Dictionary<string, BridgeConfig> Bridges { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.DashButton.Config;
using Wikiled.DashButton.Lights;
using Wikiled.DashButton.Service;

namespace Wikiled.DashButton.App.Commands
{
    [Description("Setup Bridge")]
    public class SetupBridgeCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public override void Execute()
        {
            log.Info("Discover Hue Bridge...");
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var serviceFile = Path.Combine(directory, "service.json");
            ServiceConfig serviceConfig = new ServiceConfig();
            if (File.Exists(serviceFile))
            {
                serviceConfig = JsonConvert.DeserializeObject<ServiceConfig>(File.ReadAllText(serviceFile));
            }

            BridgeSetup setup = new BridgeSetup();
            setup.Setup(serviceConfig).Wait();
            File.WriteAllText(serviceFile, JsonConvert.SerializeObject(serviceConfig));
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reactive.Linq;
using System.Reflection;
using Newtonsoft.Json;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.DashButton.Config;
using Wikiled.DashButton.Monitor;
using Wikiled.DashButton.Service;

namespace Wikiled.DashButton.App.Commands
{
    [Description("Find dash buttons")]
    public class DiscoverButtonsCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public override void Execute()
        {
            log.Info("Finding Dash Buttons...");
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var vendors = Path.Combine(directory, "vendors.json");
            if (!File.Exists(Path.Combine(directory, "vendors.json")))
            {
                log.Error("Vendors file vendors.json not found");
                return;
            }

            var serviceFile = Path.Combine(directory, "service.json");
            ConcurrentDictionary<string, string> buttonsRegister = new ConcurrentDictionary<string, string>();
            ServiceConfig serviceConfig = new ServiceConfig();
            serviceConfig.Buttons = new Dictionary<string, ButtonConfig>();
            if (File.Exists(serviceFile))
            {
                serviceConfig = JsonConvert.DeserializeObject<ServiceConfig>(File.ReadAllText(serviceFile));
                if (serviceConfig.Buttons == null)
                {
                    serviceConfig.Buttons = new Dictionary<string, ButtonConfig>();
                }

                foreach (var existingButton in serviceConfig.Buttons)
                {
                    buttonsRegister[existingButton.Value.Mac] = existingButton.Key;
                }
            }

            try
            {
                MonitoringManager manager = new MonitoringManager(VedorsManager.Load(vendors));
                manager.StartListening()
                       .Where(item => item.Vendor.Organization.ToLower().Contains("amazon"))
                       .Subscribe(
                           item =>
                           {
                               var name = item.Mac.GetMacName();
                               if (!buttonsRegister.TryAdd(name, name))
                               {
                                   log.Info("Existing dashbutton found: {0}", item);
                                   return;
                               }

                               log.Info("NEW dashbutton found: {0}", item);
                               lock (serviceConfig)
                               {
                                   ButtonConfig config = new ButtonConfig();
                                   config.Mac = name;
                                   serviceConfig.Buttons.Add(name, config);
                                   File.WriteAllText(serviceFile, JsonConvert.SerializeObject(serviceConfig));
                               }
                           });
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}

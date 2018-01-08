using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Newtonsoft.Json;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.DashButton.Monitor;
using Wikiled.DashButton.Service;

namespace Wikiled.DashButton.App.Commands
{
    [Description("Find dash buttons")]
    public class DiscoveryCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public override void Execute()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var vendors = Path.Combine(directory, "vendors.json");
            if (!File.Exists(Path.Combine(directory, "vendors.json")))
            {
                log.Error("Vendors file vendors.json not found");
                return;
            }

            var serviceFile = Path.Combine(directory, "service.json");
            ConcurrentDictionary<string, string> buttonsRegister = new ConcurrentDictionary<string, string>();
            ServiceConfiguration serviceConfiguration = new ServiceConfiguration();
            if (File.Exists(serviceFile))
            {
                serviceConfiguration = JsonConvert.DeserializeObject<ServiceConfiguration>(File.ReadAllText(serviceFile));
                foreach (var existingButton in serviceConfiguration.Buttons)
                {
                    buttonsRegister[existingButton] = existingButton;
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
                               serviceConfiguration.Buttons = buttonsRegister.Keys.ToArray();
                               lock (serviceConfiguration)
                               {
                                   File.WriteAllText(serviceFile, JsonConvert.SerializeObject(serviceConfiguration));
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

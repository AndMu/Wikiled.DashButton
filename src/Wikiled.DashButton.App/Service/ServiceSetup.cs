using System.IO;
using System.Reactive.Concurrency;
using Newtonsoft.Json;
using NLog;
using Topshelf;
using Wikiled.DashButton.Config;
using Wikiled.DashButton.Lights;
using Wikiled.DashButton.Monitor;
using Wikiled.DashButton.Service;

namespace Wikiled.DashButton.App.Service
{
    public class ServiceSetup
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void StartService(string directory)
        {
            var serviceFile = Path.Combine(directory, "service.json");
            if (!File.Exists(serviceFile))
            {
                log.Error("Configuration file service.json not found");
                return;
            }

            var vendors = Path.Combine(directory, "vendors.json");
            if (!File.Exists(Path.Combine(directory, "vendors.json")))
            {
                log.Error("Vendors file vendors.json not found");
                return;
            }

            var serviceConfig = JsonConvert.DeserializeObject<ServiceConfig>(File.ReadAllText(serviceFile));

            HostFactory.Run(x =>
            {
                x.Service<LightsService>(s =>
                {
                    s.ConstructUsing(name => new LightsService(serviceConfig, new MonitoringManager(VedorsManager.Load(vendors)), new LightsManagerFactory(), TaskPoolScheduler.Default));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("Dash button Hue Monitoring Service");
                x.SetDisplayName("DashHue Service");
                x.SetServiceName("DashHueScanner");
            });
        }
    }
}

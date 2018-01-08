using System.IO;
using NLog;
using Topshelf;
using Wikiled.DashButton.Service;

namespace Wikiled.DashButton.App.Service
{
    public class ServiceSetup
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void StartService(string directory)
        {
            if (!File.Exists(Path.Combine(directory, "service.json")))
            {
                log.Error("Configuration file appsettings.json not found");
                return;
            }

            HostFactory.Run(x =>
            {
                x.Service<LightsService>(s =>
                {
                    s.ConstructUsing(name => new LightsService());
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

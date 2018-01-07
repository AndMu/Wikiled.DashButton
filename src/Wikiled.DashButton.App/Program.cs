using System;
using System.Reactive.Linq;
using NLog;
using Wikiled.DashButton.Monitor;

namespace Wikiled.DashButton.App
{
    class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            try
            {
                MonitoringManager manager = new MonitoringManager(VedorsManager.Load("vendors.json"));
                manager.StartListening().Where(item => item.Vendor.Organization.ToLower().Contains("amazon")).Subscribe(item => { log.Info(item); });
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}

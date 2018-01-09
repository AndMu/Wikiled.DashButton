using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.DashButton.Config;
using Wikiled.DashButton.Lights;
using Wikiled.DashButton.Monitor;

namespace Wikiled.DashButton.Service
{
    public class LightsService
    {
        private readonly IMonitoringManager monitoring;

        private readonly ILightsManagerFactory factory;

        private readonly ServiceConfig config;

        private readonly IScheduler scheduler;

        private readonly Dictionary<string, Tuple<string, ButtonConfig>> buttons;

        public LightsService(ServiceConfig config, IMonitoringManager monitoring, ILightsManagerFactory factory, IScheduler scheduler)
        {
            Guard.NotNull(() => config, config);
            Guard.NotNull(() => monitoring, monitoring);
            Guard.NotNull(() => scheduler, scheduler);
            buttons = config.Buttons.ToDictionary(item => item.Value.Mac, item => new Tuple<string, ButtonConfig>(item.Key, item.Value));
            this.config = config;
            this.monitoring = monitoring;
            this.scheduler = scheduler;
            this.factory = factory;
        }

        public void Start()
        {
            var bridges = config.Bridges.Select(item => factory.Construct(item.Value));
            foreach (var lightsManager in bridges)
            {
                lightsManager.Start();
            }

            monitoring.StartListening()
                      .Select(item => item.Mac.GetMacName())
                      .Where(item => buttons.ContainsKey(item))
                      .Subscribe(
                          item =>
                          {
                              //buttons[item].
                          });
        }

        public void Stop()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.DashButton.Config;
using Wikiled.DashButton.Helpers;
using Wikiled.DashButton.Lights;
using Wikiled.DashButton.Monitor;

namespace Wikiled.DashButton.Service
{
    public class LightsService
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IMonitoringManager monitoring;

        private readonly ILightsManagerFactory factory;

        private readonly ServiceConfig config;

        private readonly IScheduler scheduler;

        private readonly Dictionary<string, Tuple<string, ButtonConfig>> buttons;

        public LightsService(ServiceConfig config, IMonitoringManager monitoring, ILightsManagerFactory factory, IScheduler scheduler)
        {
            Guard.NotNull(() => config, config);
            Guard.NotNull(() => factory, factory);
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
            var bridges = config.Bridges.Select(item => factory.Construct(item.Value)).ToArray();
            foreach (var lightsManager in bridges)
            {
                lightsManager.Start();
            }

            monitoring.StartListening()
                      .Select(item => item.Mac.GetMacName())
                      .Where(item => buttons.ContainsKey(item))
                      .GroupBy(item => item)
                      .Subscribe(
                          item =>
                              {
                                  item.SampleFirst(TimeSpan.FromSeconds(2), scheduler)
                                      .Subscribe(
                                          button =>
                                              {
                                                  if (buttons.TryGetValue(button, out var configPair))
                                                  {
                                                      foreach (var bridge in bridges)
                                                      {
                                                          bridge.ButtonPressed(configPair.Item1);
                                                      }
                                                  }

                                              });
                              });
        }

        public void Stop()
        {
        }
    }
}

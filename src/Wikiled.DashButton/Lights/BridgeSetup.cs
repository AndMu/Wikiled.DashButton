using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using Polly;
using Polly.Retry;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Wikiled.Core.Utility.Arguments;
using Wikiled.DashButton.Config;
using BridgeConfig = Wikiled.DashButton.Config.BridgeConfig;

namespace Wikiled.DashButton.Lights
{
    public class BridgeSetup
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly RetryPolicy policy;

        public BridgeSetup()
        {
            policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(10, time => TimeSpan.FromSeconds(10),
                    (exception, span, counter, context) =>
                    {
                        log.Error(exception.Message);
                        if (counter < 10)
                        {
                            log.Info("Retrying...");
                        }
                    });
        }

        public async Task Setup(ServiceConfig config)
        {
            Guard.NotNull(() => config, config);
            if (config.Bridges == null)
            {
                config.Bridges = new Dictionary<string, BridgeConfig>();
            }

            IBridgeLocator locator = new HttpBridgeLocator();
            var bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            foreach (var bridge in bridges)
            {
                if (config.Bridges.TryGetValue(bridge.BridgeId, out var bridgeConfig))
                {
                    log.Info("Bridge {0} is already registered", bridge.BridgeId);
                    continue;
                }

                log.Info("Registering bridge: {0}. Please press button on it.", bridge.BridgeId);
                ILocalHueClient client = new LocalHueClient(bridge.IpAddress);
                var appKey = await policy.ExecuteAsync(() => client.RegisterAsync("DashService", "DashHost")).ConfigureAwait(false);
                bridgeConfig = new BridgeConfig();
                bridgeConfig.AppKey = appKey;
                bridgeConfig.Id = bridge.BridgeId;
                config.Bridges[bridge.BridgeId] = bridgeConfig;
            }
        }
    }
}

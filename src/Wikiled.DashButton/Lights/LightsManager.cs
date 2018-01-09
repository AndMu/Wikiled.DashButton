using System;
using System.Threading.Tasks;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Wikiled.Core.Utility.Arguments;
using BridgeConfig = Wikiled.DashButton.Config.BridgeConfig;

namespace Wikiled.DashButton.Lights
{
    public class LightsManager : ILightsManager
    {
        private readonly BridgeConfig config;

        private ILocalHueClient client;

        public LightsManager(BridgeConfig config)
        {
            Guard.NotNull(() => config, config);
            this.config = config;
        }

        public void Start()
        {
            client = new LocalHueClient(config.Ip);
            client.Initialize(config.AppKey);
        }

        public async Task TurnGroup(string groupName)
        {
            if (client == null)
            {
                throw new InvalidOperationException("Lights manager is not started");
            }

            var group = await client.GetGroupAsync(groupName).ConfigureAwait(false);
            var command = new LightCommand();
            command.On = !group.State.AnyOn;
            await client.SendGroupCommandAsync(command, groupName).ConfigureAwait(false);
        }
    }
}

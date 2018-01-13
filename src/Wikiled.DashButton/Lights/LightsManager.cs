using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Groups;
using Wikiled.Core.Utility.Arguments;
using BridgeConfig = Wikiled.DashButton.Config.BridgeConfig;

namespace Wikiled.DashButton.Lights
{
    public class LightsManager : ILightsManager
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly BridgeConfig config;

        private readonly Dictionary<string, Group> groupsTable = new Dictionary<string, Group>(StringComparer.OrdinalIgnoreCase);

        private ILocalHueClient client;

        public LightsManager(BridgeConfig config)
        {
            Guard.NotNull(() => config, config);
            this.config = config;
        }

        public async Task Start()
        {
            IBridgeLocator locator = new HttpBridgeLocator();
            var bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            var bridge = bridges.FirstOrDefault(item => item.BridgeId == config.Id);
            if (bridge == null)
            {
                throw new InvalidOperationException($"Bridge [{config.Id}] not found");
            }

            client = new LocalHueClient(bridge.IpAddress);
            client.Initialize(config.AppKey);
            var groups = await client.GetGroupsAsync().ConfigureAwait(false);
            groupsTable.Clear();
            foreach (var item in groups)
            {
                if (groupsTable.ContainsKey(item.Name))
                {
                    log.Error("{0} Groups is already registered", item.Name);
                    continue;
                }

                groupsTable[item.Name] = item;
            }
        }

        public async Task<bool> TurnGroup(string groupName)
        {
            if (client == null)
            {
                throw new InvalidOperationException("Lights manager is not started");
            }

            log.Info("TurnGroup: {0}", groupName);
            if (!groupsTable.TryGetValue(groupName, out var @group))
            {
                log.Error("Group [{0}] is not found", groupName);
                return false;
            }

            group = await client.GetGroupAsync(@group.Id).ConfigureAwait(false);
            var command = new LightCommand();
            command.On = !group.State.AnyOn;
            if (command.On == true)
            {
                command.Brightness = 255;
            }

            await client.SendGroupCommandAsync(command, @group.Id).ConfigureAwait(false);
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Groups;
using Wikiled.Core.Utility.Arguments;
using Wikiled.DashButton.Config;
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
            client = new LocalHueClient(config.Ip);
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

        public async Task<bool> ButtonPressed(string buttonName)
        {
            if (!config.ButtonAction.TryGetValue(buttonName, out var action))
            {
                return false;
            }

            foreach (var actionGroup in action.Groups)
            {
                await TurnGroup(actionGroup).ConfigureAwait(false);
            }

            return true;
        }

        public async Task<bool> TurnGroup(string groupName)
        {
            if (client == null)
            {
                throw new InvalidOperationException("Lights manager is not started");
            }

            if (!groupsTable.TryGetValue(groupName, out var @group))
            {
                log.Error("Group [{0}] is not found", groupName);
                return false;
            }

            group = await client.GetGroupAsync(@group.Id).ConfigureAwait(false);
            var command = new LightCommand();
            command.On = !group.State.AnyOn;
            await client.SendGroupCommandAsync(command, groupName).ConfigureAwait(false);
            return true;
        }
    }
}

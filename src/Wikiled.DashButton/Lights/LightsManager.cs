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

        private object syncRoot = new object();

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

        public async Task<bool> IsAnyOn(string[] groups)
        {
            foreach (var @group in groups)
            {
                if (await IsOn(@group).ConfigureAwait(false))
                {
                    log.Info("{0} is On", @group);
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> TurnGroup(string[] groups, bool isOn)
        {
            bool isSuccessful = true;
            foreach (var @group in groups)
            {
                var result = await TurnGroup(@group, isOn).ConfigureAwait(false);
                isSuccessful &= result;
                await Task.Delay(100).ConfigureAwait(false);
            }

            return isSuccessful;
        }

        private async Task<bool> TurnGroup(string groupName, bool isOn)
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

            var command = new LightCommand();
            command.On = isOn;
            log.Info("TurnGroup: {0} On:{1} current AnyOn:{2} AllOn:{3}", groupName, command.On, group.State.AnyOn, group.State.AllOn);
            var result = await client.SendGroupCommandAsync(command, @group.Id).ConfigureAwait(false);
            if (result.HasErrors())
            {
                foreach (var error in result.Errors)
                {
                    log.Info("TurnGroup: {0} On:{1} Error:{2}", groupName, command.On, error.Error);
                }

                return false;
            }

            return true;
        }

        private async Task<bool> IsOn(string groupName)
        {
            if (!groupsTable.TryGetValue(groupName, out var @group))
            {
                log.Error("Group [{0}] is not found", groupName);
                return false;
            }

            group = await client.GetGroupAsync(@group.Id).ConfigureAwait(false);
            return group.State.AnyOn == true;
        }
    }
}

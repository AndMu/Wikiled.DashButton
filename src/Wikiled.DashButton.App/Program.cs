using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.DashButton.App.Commands;
using Wikiled.DashButton.App.Service;

namespace Wikiled.DashButton.App
{
    public class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!File.Exists(Path.Combine(directory, "appsettings.json")))
            {
                log.Error("Configuration file appsettings.json not found");
                return;
            }

            log.Info("Starting {0} version utility...", Assembly.GetExecutingAssembly().GetName().Version);
            List<Command> commandsList = new List<Command>();
            commandsList.Add(new DiscoveryDashCmd());
            var commands = commandsList.ToDictionary(item => item.Name, item => item, StringComparer.OrdinalIgnoreCase);

            if (args.Length == 0 ||
                !commands.TryGetValue(args[0], out var command))
            {
                log.Info("Starting as service");
                var setup = new ServiceSetup();
                setup.StartService(directory);
                return;
            }

            command.ParseArguments(args.Skip(1));
            command.Execute();

        }
    }
}

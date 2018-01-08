using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.DashButton.Lights;

namespace Wikiled.DashButton.App.Commands
{
    public class BridgeCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public override void Execute()
        {
            BridgeSetup setup = new BridgeSetup();
            var results = setup.Bridge().Result;
        }
    }
}

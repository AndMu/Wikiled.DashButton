using System.Threading.Tasks;

namespace Wikiled.DashButton.Lights
{
    public interface ILightsManager
    {
        Task Start();

        Task<bool> IsAnyOn(string[] groups);

        Task<bool> TurnGroup(string[] groups, bool isOn);
    }
}
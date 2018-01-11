using System.Threading.Tasks;

namespace Wikiled.DashButton.Lights
{
    public interface ILightsManager
    {
        Task Start();

        Task<bool> TurnGroup(string groupName);

        Task<bool> ButtonPressed(string buttonName);
    }
}
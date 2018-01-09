using System.Threading.Tasks;

namespace Wikiled.DashButton.Lights
{
    public interface ILightsManager
    {
        void Start();

        Task TurnGroup(string groupName);
    }
}
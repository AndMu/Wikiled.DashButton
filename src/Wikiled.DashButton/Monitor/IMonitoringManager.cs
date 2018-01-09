using System;

namespace Wikiled.DashButton.Monitor
{
    public interface IMonitoringManager
    {
        IObservable<PacketInformation> StartListening();
    }
}
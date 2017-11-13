using System;
using System.Threading.Tasks;

namespace Titanbot.Core.Startup.Interfaces
{
    public interface ITitanbotController : IDisposable
    {
        Task WhileConnected { get; }

        Task StartAsync();
        Task StopAsync();
    }
}
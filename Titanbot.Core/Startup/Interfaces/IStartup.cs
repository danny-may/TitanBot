using System;
using System.Threading.Tasks;

namespace Titanbot.Startup.Interfaces
{
    public interface IStartup : IDisposable
    {
        Task WhileConnected { get; }

        Task StartAsync();
        Task StopAsync();
    }
}
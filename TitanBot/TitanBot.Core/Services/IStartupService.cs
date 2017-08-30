using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Core.Services
{
    public interface IStartupService
    {
        Task WhileRunning { get; }

        Task StartAsync();

        Task StopAsync();
    }
}
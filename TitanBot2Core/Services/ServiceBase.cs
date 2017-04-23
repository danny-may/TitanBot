using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TitanBot2.Services
{
    public abstract class ServiceBase
    {
        protected Task MainLoop { get; private set; }
        protected CancellationTokenSource TokenSource { get; private set; }
        protected int CycleDelay { get; set; } = 10;

        protected ServiceBase()
        {

        }

        public virtual void Initialise()
        {
            TokenSource = new CancellationTokenSource();

            var token = TokenSource.Token;
            MainLoop = new Task(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(CycleDelay);
                    await Main(DateTime.Now);
                }
            }, token);

            MainLoop.Start();
        }

        public async Task StopAsync()
        {
            TokenSource.Cancel();
            await MainLoop;
        }

        protected abstract Task Main(DateTime loopTime);
    }
}

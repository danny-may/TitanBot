using System;
using System.Reflection;
using System.Threading.Tasks;
using TitanBotBase;
using TitanBotBase.Dependencies;

namespace TT2BotCore
{
    public class TT2BotClient
    {
        BotClient Client;

        public TT2BotClient(Action<IDependencyFactory> mapper)
        {
            Client = new BotClient(mapper);
            Client.Install(Assembly.GetExecutingAssembly());
        }

        public async Task StartAsync(Func<string, string> getToken)
        {
            await Client.StartAsync(getToken);
        }

        public Task StopAsync()
            => Client.StopAsync();
    }
}

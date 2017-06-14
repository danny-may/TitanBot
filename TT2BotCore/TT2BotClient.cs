using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase;
using TitanBotBase.Dependencies;
using TitanBotBase.Logger;

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

        public async Task StartAsync(Func<string> getToken)
        {
            await Client.StartAsync(getToken);
        }

        public Task StopAsync()
            => Client.StopAsync();
    }
}

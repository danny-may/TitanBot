using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase;
using TitanBotBase.Dependencies;
using TitanBotBase.Logger;
using TT2BotCore.Common;

namespace TT2BotCore
{
    public class TT2BotClient
    {
        BotClient Client;

        public TT2BotClient(Action<IDependencyFactory> mapper)
        {
            Client = new BotClient(mapper);
            Client.Install(Assembly.GetExecutingAssembly());
            Client.CommandService.DefaultPrefix = Configuration.Instance.Prefix;
        }

        public async Task StartAsync()
        {
            await Client.StartAsync(Configuration.Instance.Token);
        }

        public Task StopAsync()
            => Client.StopAsync();
    }
}

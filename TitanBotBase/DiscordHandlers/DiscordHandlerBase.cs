using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.DiscordHandlers
{
    public abstract class DiscordHandlerBase
    {
        protected IDiscordClient Client { get; }

        public DiscordHandlerBase(IDiscordClient client)
        {
            Client = client;
        }


    }
}

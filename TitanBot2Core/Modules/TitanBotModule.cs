using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;

namespace TitanBot2.Modules
{
    public abstract class TitanBotModule : ModuleBase<TitanbotCmdContext>
    {
        protected override Task<IUserMessage> ReplyAsync(string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return Context.Channel.SendMessageSafeAsync(message, ex => Context.Logger.Log(ex, "TitanBotModule"), isTTS, embed, options);
        }

        protected Task<IUserMessage> ReplyAsync(string message, Func<Exception, Task> handler = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return Context.Channel.SendMessageSafeAsync(message, handler, isTTS, embed, options);
        }
    }
}

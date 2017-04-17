using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;

namespace TitanBot2.Modules
{
    public class TitanBotModule : ModuleBase<TitanbotCmdContext>
    {
        protected override Task<IUserMessage> ReplyAsync(string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return Context.Channel.SendMessageAsync(message, ex => Context.TitanBot.Log(ex, "TitanBotModule"), isTTS, embed, options);
        }

        protected Task<IUserMessage> ReplyAsync(string message, Func<Exception, Task> handler = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return Context.Channel.SendMessageAsync(message, handler, isTTS, embed, options);
        }
    }
}

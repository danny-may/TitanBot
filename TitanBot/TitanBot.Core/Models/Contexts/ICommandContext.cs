using Discord;
using System.Collections.Generic;
using TitanBot.Core.Services.Command.Models;
using TitanBot.Core.Services.Messaging;

namespace TitanBot.Core.Models.Contexts
{
    public interface ICommandContext : IMessageContext
    {
        IDiscordClient Discord { get; }

        MessageCommandState State { get; }

        int ArgPos { get; }
        string Prefix { get; }
        string CommandText { get; }
        bool IsCommand { get; }
        bool ExplicitPrefix { get; }
        ICommandInfo Command { get; }
        string[] Arguments { get; }
        IReadOnlyDictionary<char, string> Flags { get; }

        IMessageService MessageService { get; }
    }
}
using Discord;
using System;
using System.Collections.Generic;
using TitanBot.Core.Models.Contexts;
using TitanBot.Core.Services.Command;
using TitanBot.Core.Services.Command.Models;
using TitanBot.Core.Services.Formatting;
using TitanBot.Core.Services.Messaging;

namespace TitanBot.Models.Contexts
{
    public class CommandContext : ICommandContext
    {
        #region Fields

        private readonly ICommandService _commandService;

        #endregion Fields

        #region Constructors

        public CommandContext(IUserMessage message,
                              ICommandService cmdService,
                              ITranslationService translationService,
                              IFormatterService formatterService,
                              IMessageService messageService,
                              IDiscordClient discord)
        {
            Discord = discord;
            Message = message;
            MessageService = messageService;
            TranslationService = translationService;
            FormatterService = formatterService;

            _commandService = cmdService;

            State = MessageCommandState.Invalid;

            if (!CheckPrefix()) return;
            State = State | MessageCommandState.HasPrefix;
        }

        #endregion Constructors

        #region Methods

        private bool CheckPrefix()
        {
            var sComp = StringComparison.InvariantCultureIgnoreCase;

            if (Message.HasStringPrefix(_commandService.DefaultPrefix, out var pfxLength, sComp))
                ExplicitPrefix = true;
            else if (Message.HasMentionPrefix(Discord.CurrentUser, out pfxLength) ||
                     Message.HasStringPrefix(Discord.CurrentUser.Username, out pfxLength, sComp))
                ExplicitPrefix = false;
            else if (Guild == null)
            {
                ExplicitPrefix = false;
                pfxLength = 0;
            }
            else
                return false;

            Prefix = Message.Content.Substring(0, pfxLength);
            return true;
        }

        #endregion Methods

        #region ICommandContext

        public IDiscordClient Discord { get; }
        public IUserMessage Message { get; }
        public IMessageChannel Channel => Message?.Channel;
        public IGuild Guild => (Channel as IGuildChannel)?.Guild;
        public IUser User => Message?.Author;

        public MessageCommandState State { get; }
        public int ArgPos { get; private set; }
        public string Prefix { get; private set; }
        public string CommandText { get; }
        public bool IsCommand { get; }
        public bool ExplicitPrefix { get; private set; }
        public ICommandInfo Command { get; }

        public IMessageService MessageService { get; }
        public ITranslationService TranslationService { get; }
        public IFormatterService FormatterService { get; }
        public ITranslationSet TranslationSet { get; }
        public IValueFormatter ValueFormatter { get; }
        public string[] Arguments { get; }
        public IReadOnlyDictionary<char, string> Flags { get; }

        #endregion ICommandContext
    }
}
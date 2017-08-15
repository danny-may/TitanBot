using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Dependencies;
using TitanBot.Formatting;
using TitanBot.Formatting.Interfaces;
using TitanBot.Logging;
using TitanBot.Settings;

namespace TitanBot.Replying
{
    class ModifyContext : IModifyContext
    {
        private IUserMessage Message { get; }
        private IMessageChannel Channel => Message.Channel;
        private IUser User { get; }
        private ILogger Logger { get; }
        private DiscordSocketClient Client { get; }

        private IGuild Guild => (Channel as IGuildChannel)?.Guild;

        private GeneralUserSetting GeneralUserSetting { get; }
        private ITextResourceCollection TextResource { get; }

        private ILocalisable<string> Text { get; set; }
        private string Localised => Text.Localise(TextResource);
        private RequestOptions Options { get; set; }
        private IEmbedable Embedable { get; set; }

        private event MessageModifyErrorHandler Handler;
        public event OnModifyEventHandler OnModify;

        public ModifyContext(IUserMessage message, IUser user, IDependencyFactory factory, ILogger logger)
        {
            Message = message;
            Text = (RawString)message.Content;
            Embedable = Commands.Embedable.FromEmbed(message.Embeds.FirstOrDefault(e => e.Type == EmbedType.Rich) as Embed);
            Logger = logger;

            User = user;
            Client = factory.Get<DiscordSocketClient>();

            var settings = factory.Get<ISettingManager>();

            GeneralUserSetting = settings.GetContext(User).Get<GeneralUserSetting>();
            var guildSetting = settings.GetContext(Guild)?.Get<GeneralGuildSetting>();
            var formatter = factory.GetOrStore<ValueFormatter>();
            var textManager = factory.Get<ITextResourceManager>();

            TextResource = textManager.GetForLanguage(guildSetting?.PreferredLanguage ?? GeneralUserSetting.Language, GeneralUserSetting.FormatType);

            OnModify += (s, m) => Task.CompletedTask;
        }

        public IModifyContext WithErrorHandler(MessageModifyErrorHandler handler)
        {
            Handler += handler;
            return this;
        }

        public IModifyContext ChangeMessage(string message)
            => ChangeMessage(new LocalisedString(message));
        public IModifyContext ChangeRawMessage(string message)
            => ChangeMessage(new RawString(message));
        public IModifyContext ChangeMessage(string message, ReplyType replyType)
            => ChangeMessage(new LocalisedString(message, replyType));
        public IModifyContext ChangeMessage(string message, params object[] values)
            => ChangeMessage(new LocalisedString(message, values));
        public IModifyContext ChangeMessage(string message, ReplyType replyType, params object[] values)
            => ChangeMessage(new LocalisedString(message, replyType, values));
        public IModifyContext ChangeMessage(ILocalisable<string> message)
        {
            Text = message;
            return this;
        }

        public IModifyContext WithRequestOptions(RequestOptions options)
        {
            Options = options;
            return this;
        }

        public IModifyContext ChangeEmbedable(ILocalisable<EmbedBuilder> embedable)
            => ChangeEmbedable(Commands.Embedable.FromEmbed(embedable));
        public IModifyContext ChangeEmbedable(IEmbedable embedable)
        {
            Embedable = embedable;
            return this;
        }

        public void Modify(bool stealthy = false)
            => ModifyAsync(stealthy).DontWait();

        public async Task ModifyAsync(bool stealthy = false)
        {
            if (Message.Author.Id != Client.CurrentUser.Id)
                throw new InvalidOperationException($"Unable to modify the message of another person.\n{Message.Id}");
            try
            {
                var message = Text.Localise(TextResource);
                IUser me = Guild?.GetUserAsync(Client.CurrentUser.Id).Result ?? (IUser)Client.CurrentUser;
                if (!(GeneralUserSetting.UseEmbeds && Message.Channel.UserHasPermission(me, ChannelPermission.EmbedLinks)))
                {
                    message = message + "\n" + Embedable?.GetString().Localise(TextResource);
                    Embedable = null;
                }

                await Message.ModifyAsync(m =>
                {
                    m.Content = message;
                    m.Embed = Embedable?.GetEmbed().Localise(TextResource).Build();
                }, Options);
            }
            catch (Exception ex)
            {
                if (Handler == null)
                    throw;
                await Handler(ex, Message, Localised, Embedable);
            }

            if (!stealthy)
                await OnModify(this, Message);

            Logger.Log(Logging.LogSeverity.Verbose, LogType.Message, $"Modified Message | Channel: {Channel} | Guild: {Guild} | Message: {Message.Id}", "Modifier");
        }
    }
}

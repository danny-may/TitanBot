using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Commands;
using TitanBot.Dependencies;
using TitanBot.Formatting;
using TitanBot.Settings;
using TitanBot.Util;

namespace TitanBot.Replying
{
    class ModifyContext : IModifyContext
    {
        private IUserMessage Message { get; }
        private IMessageChannel Channel => Message.Channel;
        private IUser User { get; }
        private DiscordSocketClient Client { get; }

        private IGuild Guild => (Channel as IGuildChannel)?.Guild;

        private GeneralUserSetting GeneralUserSetting { get; }
        private ITextResourceCollection TextResource { get; }

        private string Text { get; set; }
        private RequestOptions Options { get; set; }
        private IEmbedable Embedable { get; set; }

        private event MessageModifyErrorHandler Handler;
        public event OnModifyEventHandler OnModify;

        public ModifyContext(IUserMessage message, IUser user, IDependencyFactory factory)
        {
            Message = message;
            Text = message.Content;
            Embedable = Commands.Embedable.FromEmbed(message.Embeds.FirstOrDefault(e => e.Type == EmbedType.Rich) as Embed);

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
        {
            Text = TextResource.GetResource(message);
            return this;
        }

        public IModifyContext ChangeMessage(string message, ReplyType replyType)
        {
            Text = TextResource.GetResource(message, replyType);
            return this;
        }

        public IModifyContext ChangeMessage(string message, params object[] values)
        {
            Text = TextResource.Format(message, values);
            return this;
        }

        public IModifyContext ChangeMessage(string message, ReplyType replyType, params object[] values)
        {
            Text = TextResource.Format(message, replyType, values);
            return this;
        }

        public IModifyContext WithRequestOptions(RequestOptions options)
        {
            Options = options;
            return this;
        }
        public IModifyContext ChangeEmbedable(LocalisedEmbedBuilder embedable)
            => ChangeEmbedable(embedable.Localise(TextResource));
        public IModifyContext ChangeEmbedable(Embedable embedable)
            => ChangeEmbedable((IEmbedable)embedable);
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
                IUser me = Guild?.GetUserAsync(Client.CurrentUser.Id).Result ?? (IUser)Client.CurrentUser;
                if (!(GeneralUserSetting.UseEmbeds && Message.Channel.UserHasPermission(me, ChannelPermission.EmbedLinks)))
                {
                    Text = Text + "\n" + Embedable?.GetString();
                    Embedable = null;
                }

                await Message.ModifyAsync(m =>
                {
                    m.Content = Text;
                    m.Embed = Embedable?.GetEmbed();
                }, Options);
            }
            catch (Exception ex)
            {
                if (Handler == null)
                    throw;
                await Handler(ex, Message, Text, Embedable);
            }

            if (!stealthy)
                await OnModify(this, Message);
        }
    }
}

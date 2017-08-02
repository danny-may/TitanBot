using Discord;
using Discord.Net;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using TitanBot.Commands;
using TitanBot.Dependencies;
using TitanBot.Formatting;
using TitanBot.Settings;
using TitanBot.Util;

namespace TitanBot.Replying
{
    class ReplyContext : IReplyContext
    {
        private IMessageChannel Channel { get; set; }
        private IUser User { get; }
        private DiscordSocketClient Client { get; }

        private IGuild Guild => (Channel as IGuildChannel)?.Guild;

        private GeneralUserSetting GeneralUserSetting { get; }
        private ITextResourceCollection TextResource { get; }

        private string Message { get; set; } = "";
        private bool IsTTS { get; set; }
        private RequestOptions Options { get; set; }
        private IEmbedable Embedable { get; set; }
        private Func<Stream> Attachment { get; set; }
        private string AttachmentName { get; set; }

        private event MessageSendErrorHandler Handler;
        public event OnSendEventHandler OnSend;
        
        public ReplyContext(IMessageChannel channel, IUser user, IDependencyFactory factory)
        {
            Channel = channel;
            User = user;
            Client = factory.Get<DiscordSocketClient>();

            var settings = factory.Get<ISettingManager>();

            GeneralUserSetting = settings.GetContext(User).Get<GeneralUserSetting>();
            var guildSetting = settings.GetContext(Guild)?.Get<GeneralGuildSetting>();
            var formatter = factory.WithInstance(GeneralUserSetting).Construct<ValueFormatter>();
            var textManager = factory.Get<ITextResourceManager>();

            TextResource = textManager.GetForLanguage(guildSetting?.PreferredLanguage ?? GeneralUserSetting.Language, GeneralUserSetting.FormatType);

            OnSend += (s, m) => Task.CompletedTask;
        }

        public void Send(bool stealthy = false)
            => SendAsync(stealthy).DontWait();

        public async ValueTask<IUserMessage> SendAsync(bool stealthy = false)
        {
            if (string.IsNullOrWhiteSpace(Message) && Embedable == null)
                throw new InvalidOperationException("Unable to send a message without an embed or message");

            IUserMessage msg = null;

            try
            {
                var me = Guild?.GetUserAsync(Client.CurrentUser.Id).Result ?? (IUser)Client.CurrentUser;
                IsTTS = IsTTS && Channel.UserHasPermission(me, ChannelPermission.SendTTSMessages);
                if (Attachment == null)
                    if (GeneralUserSetting.UseEmbeds && Channel.UserHasPermission(me, ChannelPermission.EmbedLinks))
                        msg = await Channel.SendMessageAsync(Message, IsTTS, Embedable?.GetEmbed(), Options);
                    else
                        msg = await Channel.SendMessageAsync(Message + "\n" + Embedable?.GetString(), IsTTS, null, Options);
                else
                    msg = await Channel.SendFileAsync(Attachment(), AttachmentName, Message + "\n" + Embedable?.GetString(), IsTTS, Options);
            }
            catch (HttpException ex) when (ex.DiscordCode == 50013 || ex.DiscordCode == 50001)
            {
                Message = TextResource.Format(TitanBotResource.UNABLE_SEND, ReplyType.Error, Channel, Message);
                Channel = await User.GetOrCreateDMChannelAsync();
                return await SendAsync(stealthy);
            }
            catch (ArgumentException ex) when (ex.Message.StartsWith("Message content is too long,"))
            {
                var message = (Message + "\n" + Embedable?.GetString()).Trim();
                if (Attachment != null)
                    message += "\n\n" + TextResource.Format(TitanBotResource.MESSAGE_CONTAINED_ATTACHMENT, AttachmentName);
                Attachment = () => message.ToStream();
                AttachmentName = "Output.txt";
                Message = TextResource.GetResource(TitanBotResource.MESSAGE_TOO_LONG, ReplyType.Error);
                Embedable = null;

                return await SendAsync(stealthy);
            }
            catch (Exception ex)
            {
                if (Handler == null)
                    throw;
                await Handler(ex, Channel, Message, Embedable);
            }
            if (!stealthy)
                await OnSend(this, msg);
            return msg;
        }

        public IReplyContext WithErrorHandler(MessageSendErrorHandler handler)
        {
            Handler += handler;
            return this;
        }

        public IReplyContext WithMessage(string message)
        {
            Message = TextResource.GetResource(message);
            return this;
        }

        public IReplyContext WithMessage(string message, ReplyType replyType)
        {
            Message = TextResource.GetResource(message, replyType);
            return this;
        }

        public IReplyContext WithMessage(string message, params object[] values)
        {
            Message = TextResource.Format(message, values);
            return this;
        }

        public IReplyContext WithMessage(string message, ReplyType replyType, params object[] values)
        {
            Message = TextResource.Format(message, replyType, values);
            return this;
        }

        public IReplyContext WithRequestOptions(RequestOptions options)
        {
            Options = options;
            return this;
        }

        public IReplyContext WithTTS(bool tts)
        {
            IsTTS = tts;
            return this;
        }

        public IReplyContext WithAttachment(Func<Stream> attachment, string name)
        {
            Attachment = attachment;
            AttachmentName = name;
            return this;
        }

        public IReplyContext WithEmbedable(LocalisedEmbedBuilder embedable)
            => WithEmbedable(embedable.Localise(TextResource));
        public IReplyContext WithEmbedable(Embedable embedable)
            => WithEmbedable((IEmbedable)embedable);
        public IReplyContext WithEmbedable(IEmbedable embedable)
        {
            Embedable = embedable;
            return this;
        }
    }
}

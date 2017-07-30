using Discord;
using Discord.Net;
using System;
using System.IO;
using System.Threading.Tasks;
using TitanBot.Util;

namespace TitanBot.Commands
{
    class ReplyContext : IReplyContext
    {
        private ICommandContext Context { get; }
        private IMessageChannel Channel { get; set; }
        private string Message { get; set; } = "";
        private bool IsTTS { get; set; }
        private RequestOptions Options { get; set; }
        private IEmbedable Embedable { get; set; }
        private Func<Stream> Attachment { get; set; }
        private string AttachmentName { get; set; }

        private event MessageSendErrorHandler Handler;
        public event OnSendEventHandler OnSend;
        
        public ReplyContext(IMessageChannel channel, ICommandContext context)
        {
            Context = context;
            Channel = channel;
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
                IUser me = Context.Client.CurrentUser;
                me = Context.Guild?.GetUserAsync(me.Id).Result ?? me;
                IsTTS = IsTTS && Channel.UserHasPermission(me, ChannelPermission.SendTTSMessages);
                if (Attachment == null)
                    if (Context.GeneralUserSetting.UseEmbeds && Channel.UserHasPermission(me, ChannelPermission.EmbedLinks))
                        msg = await Channel.SendMessageAsync(Message, IsTTS, Embedable?.GetEmbed(), Options);
                    else
                        msg = await Channel.SendMessageAsync(Message + "\n" + Embedable?.GetString(), IsTTS, null, Options);
                else
                    msg = await Channel.SendFileAsync(Attachment(), AttachmentName, Message + "\n" + Embedable?.GetString(), IsTTS, Options);
            }
            catch (HttpException ex) when (ex.DiscordCode == 50013)
            {
                Message = Context.TextResource.Format("UNABLE_SEND", ReplyType.Error, Channel, Message);
                Channel = await Context.Author.GetOrCreateDMChannelAsync();
                return await SendAsync(stealthy);
            }
            catch (ArgumentException ex) when (ex.Message.StartsWith("Message content is too long,"))
            {
                var message = (Message + "\n" + Embedable?.GetString()).Trim();
                if (Attachment != null)
                    message += "\n\n" + Context.TextResource.Format("MESSAGE_CONTAINED_ATTACHMENT", AttachmentName);
                Attachment = () => message.ToStream();
                AttachmentName = "Output.txt";
                Message = Context.TextResource.GetResource("MESSAGE_TOO_LONG", ReplyType.Error);
                Embedable = null;

                return await SendAsync(stealthy);
            }
            catch (Exception ex)
            {
                if (Handler == null)
                    throw;
                await Handler(ex, Channel, Context, Message, Embedable);
            }
            if (!stealthy)
                await OnSend(this, msg);
            return msg;
        }

        public IReplyContext WithErrorHandler(MessageSendErrorHandler handler)
            => MiscUtil.InlineAction(this, v => v.Handler += handler);

        public IReplyContext WithMessage(string message)
            => MiscUtil.InlineAction(this, v => v.Message = Context.TextResource.GetResource(message));

        public IReplyContext WithMessage(string message, ReplyType replyType)
            => MiscUtil.InlineAction(this, v => v.Message = Context.TextResource.GetResource(message, replyType));

        public IReplyContext WithMessage(string message, params object[] values)
            => MiscUtil.InlineAction(this, v => v.Message = Context.TextResource.Format(message, values));

        public IReplyContext WithMessage(string message, ReplyType replyType, params object[] values)
            => MiscUtil.InlineAction(this, v => v.Message = Context.TextResource.Format(message, replyType, values));

        public IReplyContext WithRequestOptions(RequestOptions options)
            => MiscUtil.InlineAction(this, v => v.Options = options);

        public IReplyContext WithTTS(bool tts)
            => MiscUtil.InlineAction(this, v => v.IsTTS = tts);

        public IReplyContext WithAttachment(Func<Stream> attachment, string name)
            => MiscUtil.InlineAction(this, v => { v.Attachment = attachment; v.AttachmentName = name; });

        public IReplyContext WithEmbedable(IEmbedable embedable)
            => MiscUtil.InlineAction(this, v => v.Embedable = embedable);
    }
}

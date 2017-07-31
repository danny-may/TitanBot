using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Util;

namespace TitanBot.Commands
{
    class ModifyContext : IModifyContext
    {
        private ICommandContext Context { get; }
        private IUserMessage Message { get; }

        private string Text { get; set; }
        private RequestOptions Options { get; set; }
        private IEmbedable Embedable { get; set; }

        private event MessageModifyErrorHandler Handler;
        public event OnModifyEventHandler OnModify;

        public ModifyContext(ICommandContext context, IUserMessage message)
        {
            Message = message;
            Context = context;
            Text = message.Content;
            Embedable = Commands.Embedable.FromEmbed(message.Embeds.FirstOrDefault(e => e.Type == EmbedType.Rich) as Embed);
            OnModify += (s, m) => Task.CompletedTask;
        }

        public IModifyContext WithErrorHandler(MessageModifyErrorHandler handler)
            => MiscUtil.InlineAction(this, v => v.Handler += handler);

        public IModifyContext ChangeMessage(string message)
            => MiscUtil.InlineAction(this, v => v.Text = Context.TextResource.GetResource(message));

        public IModifyContext ChangeMessage(string message, ReplyType replyType)
            => MiscUtil.InlineAction(this, v => v.Text = Context.TextResource.GetResource(message, replyType));

        public IModifyContext ChangeMessage(string message, params object[] values)
            => MiscUtil.InlineAction(this, v => v.Text = Context.TextResource.Format(message, values));

        public IModifyContext ChangeMessage(string message, ReplyType replyType, params object[] values)
            => MiscUtil.InlineAction(this, v => v.Text = Context.TextResource.Format(message, replyType, values));

        public IModifyContext WithRequestOptions(RequestOptions options)
            => MiscUtil.InlineAction(this, v => v.Options = options);

        public IModifyContext ChangeEmbedable(IEmbedable embedable)
            => MiscUtil.InlineAction(this, v => v.Embedable = embedable);

        public void Modify(bool stealthy = false)
            => ModifyAsync(stealthy).DontWait();

        public async Task ModifyAsync(bool stealthy = false)
        {
            if (Message.Author.Id != Context.Client.CurrentUser.Id)
                throw new InvalidOperationException($"Unable to modify the message of another person.\n{Message.Id}");
            try
            {
                IUser me = Context.Client.CurrentUser;
                me = Context.Guild?.GetUserAsync(me.Id).Result ?? me;
                if (!(Context.GeneralUserSetting.UseEmbeds && Message.Channel.UserHasPermission(me, ChannelPermission.EmbedLinks)))
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
                await Handler(ex, Message, Context, Text, Embedable);
            }

            if (!stealthy)
                await OnModify(this, Message);
        }
    }
}

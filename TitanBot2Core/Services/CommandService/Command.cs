using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Responses;
using TitanBot2.Services.CommandService.Flags;
using TitanBot2.Services.CommandService.Models;
using TitanBot2.TypeReaders;

namespace TitanBot2.Services.CommandService
{
    public abstract class Command
    {
        public ulong[] GuildRestrictions { get; protected set; } = new ulong[0];
        protected string DelayMessage { get; set; } = "This seems to be taking longer than expected... ";

        private bool HasReplied;
        private IUserMessage awaitMessage;
        private object _lock = new object();

        protected CallCheckResponse CheckResult { get; private set; }

        protected CmdContext Context { get; private set; }
        protected TypeReaderCollection Readers { get; private set; }
        protected FlagCollection Flags { get; private set; }

        private Dictionary<CallInfo, CallCheckResponse> CheckResponses = new Dictionary<CallInfo, CallCheckResponse>();

        public async Task<bool> ExecuteAsync(CallInfo callInfo, ArgumentReadResponse argResponse, FlagCollection flags)
        {
            if (!argResponse.IsSuccess)
                return false;

            if (!CheckResponses.ContainsKey(callInfo))
                CheckResponses.Add(callInfo, await CheckCallAsync(callInfo));

            if (!CheckResponses[callInfo].IsSuccess)
                return false;

            new Task(async () =>
            {
                await Task.Delay(3000);
                lock (_lock)
                {
                    if (!HasReplied)
                    {
                        awaitMessage = Context.Channel.SendMessageSafeAsync(DelayMessage).Result;
                    }
                }
            }).Start();

            try
            {
                Flags = flags;
                await (Task)callInfo.Call.Invoke(this, argResponse.ParsedArguments);
            } catch (Exception ex)
            {
                await Context.Logger.Log(ex, callInfo.ParentInfo.Name);
                try
                {
                    if (ex is AggregateException)
                        await ReplyAsync($"Aggregate exception thrown: `{string.Join("`, `", (ex as AggregateException).InnerExceptions.Select(e => e.GetType()))}`", ReplyType.Error);
                    else
                        await ReplyAsync($"Exception thrown: `{ex.GetType()}`", ReplyType.Error);
                }
                catch { }
            }

            return true;
        }

        public void SetContext(CmdContext context, TypeReaderCollection readers)
        {
            if (Context != null || Readers != null)
                throw new InvalidOperationException("Unable to set Command Context once it has already been set");
            Context = context;
            Readers = readers;
        }

        public virtual async Task<CallCheckResponse> CheckCallAsync(CallInfo info)
        {
            var results = new List<CallCheckResponse>();

            results.Add(await CheckContexts(info.RequiredContexts));
            if (info.RequireOwner)
                results.Add(await CheckOwner());
            results.Add(await CheckPermissions(info.DefaultPermissions, info.PermissionKey));
            results.Add(await CheckGuild(GuildRestrictions));            

            if (results.Any(r => !r.IsSuccess))
                return CallCheckResponse.FromError(string.Join("\n", results.Where(r => !r.IsSuccess).Select(r => r.Message)));

            return CallCheckResponse.FromSuccess();
        }

        protected virtual Task<CallCheckResponse> CheckGuild(ulong[] guilds)
        {
            if (guilds == null || guilds.Length == 0)
                return Task.FromResult(CallCheckResponse.FromSuccess());

            if (Context.Guild != null && guilds.Contains(Context.Guild.Id))
                return Task.FromResult(CallCheckResponse.FromSuccess());

            return Task.FromResult(CallCheckResponse.FromError("Invalid guild! This command can only be used in " + string.Join(", ", guilds.Select(g => Context.Client.GetGuild(g).Name ?? "UNKNOWN GUILD"))));
        }

        protected virtual Task<CallCheckResponse> CheckContexts(ContextType contexts)
        {
            bool isValid = false;

            if ((contexts & ContextType.Guild) != 0)
                isValid = isValid || Context.Channel is IGuildChannel;
            if ((contexts & ContextType.DM) != 0)
                isValid = isValid || Context.Channel is IDMChannel;
            if ((contexts & ContextType.Group) != 0)
                isValid = isValid || Context.Channel is IGroupChannel;

            if (isValid)
                return Task.FromResult(CallCheckResponse.FromSuccess());
            else
                return Task.FromResult(CallCheckResponse.FromError($"Invalid context for command! Accepted contexts: {contexts}"));
        }

        protected virtual async Task<CallCheckResponse> CheckOwner()
        {
            switch (Context.Client.TokenType)
            {
                case TokenType.Bot:
                    var application = await Context.Client.GetApplicationInfoAsync();
                    if (Context.User.Id != application.Owner.Id)
                        return CallCheckResponse.FromError("Command can only be run by the owner of the bot");
                    return CallCheckResponse.FromSuccess();
                case TokenType.User:
                    if (Context.User.Id != Context.Client.CurrentUser.Id)
                        return CallCheckResponse.FromError("Command can only be run by the owner of the bot");
                    return CallCheckResponse.FromSuccess();
                default:
                    return CallCheckResponse.FromError($"{nameof(TokenType)} applications do not have an owner");
            }
        }

        protected virtual async Task<CallCheckResponse> CheckPermissions(ulong defaultPerm, string permKey)
        {
            var owner = Context.BotClient?.Owner ?? (await Context.Client.GetApplicationInfoAsync()).Owner;

            if (Context.User.Id == owner.Id || Context.Channel is IDMChannel || Context.Channel is IGroupChannel)
                return CallCheckResponse.FromSuccess();

            if (Context.Guild.OwnerId == Context.User.Id)
                return CallCheckResponse.FromSuccess();

            var guildData = await Context.Database.Guilds.GetGuild(Context.Guild.Id);
            var guildUser = Context.User as IGuildUser;

            if (guildUser.HasAll(guildData.PermOverride))
                return CallCheckResponse.FromSuccess();

            if ((guildData.BlackListed?.Length ?? 0) != 0)
                if (guildData.BlackListed.Contains(Context.Channel.Id))
                    return CallCheckResponse.FromError(null);

            var cmdPerm = (await Context.Database.CmdPerms.GetCmdPerm(Context.Guild.Id, permKey));

            if ((cmdPerm?.blackListed?.Length ?? 0) != 0)
                if (cmdPerm.blackListed.Contains(Context.Channel.Id))
                    return CallCheckResponse.FromError(null);

            if (cmdPerm?.permissionId == null && cmdPerm?.roleIds == null)
            {
                if (guildUser.HasAll(cmdPerm?.permissionId ?? defaultPerm))
                    return CallCheckResponse.FromSuccess();
            }

            if (cmdPerm?.roleIds != null && guildUser.RoleIds.Any(r => cmdPerm?.roleIds?.Contains(r) ?? false))
                return CallCheckResponse.FromSuccess();

            if (cmdPerm?.permissionId != null && guildUser.HasAll(cmdPerm?.permissionId ?? defaultPerm))
                return CallCheckResponse.FromSuccess();

            return CallCheckResponse.FromError($"{Context.User.Mention} You do not have the required permissions to use that command");
        }

        protected Task<IUserMessage> ReplyAsync(string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ReplyAsync(message, ReplyType.None, null, isTTS, embed, options);

        protected Task<IUserMessage> ReplyAsync(string message, ReplyType replyType, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ReplyAsync(message, replyType, null, isTTS, embed, options);

        protected Task<IUserMessage> ReplyAsync(string message, Func<Exception, Task> handler, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ReplyAsync(message, ReplyType.None, handler, isTTS, embed, options);

        protected async Task<IUserMessage> ReplyAsync(string message, ReplyType replyType, Func<Exception, Task> handler, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            message = FormatMessage(message, replyType);
            return await SendSafe(Context.Channel, message, handler, isTTS, embed, options);
        }

        protected Task<IUserMessage> TrySend(ulong channelid, string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => TrySend(channelid, message, ReplyType.None, null, isTTS, embed, options);

        protected Task<IUserMessage> TrySend(ulong channelid, string message, ReplyType replyType, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => TrySend(channelid, message, replyType, null, isTTS, embed, options);

        protected Task<IUserMessage> TrySend(ulong channelid, string message, Func<Exception, Task> handler, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => TrySend(channelid, message, ReplyType.None, handler, isTTS, embed, options);

        protected async Task<IUserMessage> TrySend(ulong channelid, string message, ReplyType replyType, Func<Exception, Task> handler, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            IMessageChannel channel = Context.Client.AllChannels().SingleOrDefault(c => c.Id == channelid) as IMessageChannel;
            if (channel == null)
                channel = Context.Client.DMChannels.SingleOrDefault(c => c.Recipient.Id == channelid) as IMessageChannel;
            if (channel == null)
                channel = await Context.Client.GetUser(channelid)?.CreateDMChannelAsync();

            var msgChannel = channel as IMessageChannel;
            if (msgChannel == null)
                return await ReplyAsync(message, replyType, handler, isTTS, embed, options);

            message = FormatMessage(message, replyType);

            return await SendSafe(msgChannel, message, handler, isTTS, embed, options);
        }

        private async Task<IUserMessage> SendSafe(IMessageChannel channel, string message, Func<Exception, Task> handler, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            lock (_lock)
            {
                HasReplied = true;
                if (awaitMessage != null)
                {
                    var temp = awaitMessage;
                    awaitMessage = null;
                    temp.DeleteAsync().Wait();
                }
            }
            return await channel.SendMessageSafeAsync(message, async e => {
                try
                {
                    await Context.Channel.SendMessageSafeAsync($"{Res.Str.ErrorText} There was an error when trying to handle your command! `{e.GetType()}`");
                }
                catch { }
                await Context.Logger.Log(e, $"Command: {this.GetType().Name}");
                await (handler?.Invoke(e) ?? Task.CompletedTask);
            }, isTTS, embed, options);
        }

        public static string FormatMessage(string message, ReplyType replyType)
        {
            switch (replyType)
            {
                case ReplyType.Success:
                    message = Res.Str.SuccessText + " " + message;
                    break;
                case ReplyType.Error:
                    message = Res.Str.ErrorText + " " + message;
                    break;
                case ReplyType.Info:
                    message = Res.Str.InfoText + " " + message;
                    break;
                default:
                    break;
            }

            return message;
        }

        public enum ReplyType
        {
            Success,
            Error,
            Info,
            None
        }
    }
}

using Discord;
using System;
using System.Collections.Concurrent;
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
        protected string DelayMessage { get; set; } = "This seems to be taking longer than expected... ";

        private bool HasReplied;
        private IUserMessage awaitMessage;
        
        private static ConcurrentDictionary<Type, object> _commandLocks = new ConcurrentDictionary<Type, object>();
        protected object _globalCommandLock => _commandLocks.GetOrAdd(GetType(), new object());

        private static ConcurrentDictionary<Type, ConcurrentDictionary<ulong?, object>> _guildLocks = new ConcurrentDictionary<Type, ConcurrentDictionary<ulong?, object>>();
        protected object _guildCommandLock => _guildLocks.GetOrAdd(GetType(), new ConcurrentDictionary<ulong?, object>()).GetOrAdd(Context.Guild?.Id, new object());

        private static ConcurrentDictionary<Type, ConcurrentDictionary<ulong, object>> _channelLocks = new ConcurrentDictionary<Type, ConcurrentDictionary<ulong, object>>();
        protected object _channelCommandLock => _channelLocks.GetOrAdd(GetType(), new ConcurrentDictionary<ulong, object>()).GetOrAdd(Context.Channel.Id, new object());

        protected object _instanceCommandLock { get; } = new object();

        protected CallCheckResponse CheckResult { get; private set; }

        protected CmdContext Context { get; private set; }
        protected TypeReaderCollection Readers { get; private set; }
        protected FlagCollection Flags { get; private set; }

        public static int TotalCommands { get; private set; } = 0;

        private Dictionary<CallInfo, CallCheckResponse> CheckResponses = new Dictionary<CallInfo, CallCheckResponse>();

        public async Task<bool> ExecuteAsync(CallInfo callInfo, ArgumentReadResponse argResponse, FlagCollection flags)
        {
            if (!argResponse.IsSuccess)
                return false;

            if (!CheckResponses.ContainsKey(callInfo))
                await CheckCallsAsync(new CallInfo[] { callInfo });

            if (!CheckResponses[callInfo].IsSuccess)
                return false;

            new Task(async () =>
            {
                await Task.Delay(3000);
                lock (_instanceCommandLock)
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
                TotalCommands++;
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

            if (!HasReplied)
            {
                await ReplyAsync("No reply was set up for this path (blame Titansmasher), but I think it executed correctly", ReplyType.Error);
                HasReplied = true;
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

        public virtual async Task<Dictionary<CallInfo, CallCheckResponse>> CheckCallsAsync(CallInfo[] calls)
        {
            var results = calls.ToDictionary(c => c, c => new List<CallCheckResponse>());

            foreach (var res in CheckContexts(calls))
                results[res.Key].Add(res.Value);
            foreach (var res in await CheckOwner(calls))
                results[res.Key].Add(res.Value);
            foreach (var res in await CheckPermissions(calls))
                results[res.Key].Add(res.Value);
            foreach (var res in CheckGuild(calls))
                results[res.Key].Add(res.Value);

            var response = results.ToDictionary(c => c.Key, c =>
            {
                if (c.Value.Any(r => !r.IsSuccess))
                    return CallCheckResponse.FromError(string.Join("\n", c.Value.Where(r => !r.IsSuccess).Select(r => r.Message)));

                return CallCheckResponse.FromSuccess();
            });

            foreach (var res in response)
                CheckResponses.Add(res.Key, res.Value);

            return response;
        }

        protected virtual Dictionary<CallInfo, CallCheckResponse> CheckGuild(CallInfo[] calls)
        {
            return calls.ToDictionary(c => c, c =>
            {
                if (c.ParentInfo.RequireGuild == null)
                    return CallCheckResponse.FromSuccess();

                if (Context.Guild != null && c.ParentInfo.RequireGuild == Context.Guild.Id)
                    return CallCheckResponse.FromSuccess();

                return CallCheckResponse.FromError($"Invalid guild! This command can only be used in {Context.Client.GetGuild(c.ParentInfo.RequireGuild.Value)?.Name ?? "UNKNOWN GUILD"}");
            });
            
        }

        protected virtual IDictionary<CallInfo, CallCheckResponse> CheckContexts(CallInfo[] calls)
        {
            return calls.ToDictionary(c => c, c =>
            {
                bool isValid = false;

                if ((c.RequiredContexts & ContextType.Guild) != 0)
                    isValid = isValid || Context.Channel is IGuildChannel;
                if ((c.RequiredContexts & ContextType.DM) != 0)
                    isValid = isValid || Context.Channel is IDMChannel;
                if ((c.RequiredContexts & ContextType.Group) != 0)
                    isValid = isValid || Context.Channel is IGroupChannel;

                if (isValid)
                    return CallCheckResponse.FromSuccess();
                else
                    return CallCheckResponse.FromError($"Invalid context for command! Accepted contexts: {c.RequiredContexts}");
            });
        }

        protected virtual async Task<IDictionary<CallInfo, CallCheckResponse>> CheckOwner(CallInfo[] calls)
        {
            CallCheckResponse response;

            switch (Context.Client.TokenType)
            {
                case TokenType.Bot:
                    var application = await Context.Client.GetApplicationInfoAsync();
                    if (Context.User.Id != application.Owner.Id)
                        response = CallCheckResponse.FromError("Command can only be run by the owner of the bot");
                    else
                        response = CallCheckResponse.FromSuccess();
                    break;
                case TokenType.User:
                    if (Context.User.Id != Context.Client.CurrentUser.Id)
                        response = CallCheckResponse.FromError("Command can only be run by the owner of the bot");
                    else
                        response = CallCheckResponse.FromSuccess();
                    break;
                default:
                    response = CallCheckResponse.FromError($"{Context.Client.TokenType} applications do not have an owner");
                    break;
            }

            return calls.ToDictionary(c => c, c => c.RequireOwner ? response : CallCheckResponse.FromSuccess());
        }

        protected virtual async Task<IDictionary<CallInfo, CallCheckResponse>> CheckPermissions(CallInfo[] calls)
        {
            var owner = Context.BotClient?.Owner ?? (await Context.Client.GetApplicationInfoAsync()).Owner;

            if (Context.User.Id == owner.Id || Context.Channel is IDMChannel || Context.Channel is IGroupChannel)
                return calls.ToDictionary(c=>c, c=> CallCheckResponse.FromSuccess());

            if (Context.Guild.OwnerId == Context.User.Id)
                return calls.ToDictionary(c => c, c => CallCheckResponse.FromSuccess());

            var guildData = await Context.Database.Guilds.GetGuild(Context.Guild.Id);
            var guildUser = Context.User as IGuildUser;

            if (guildUser.HasAll(guildData.PermOverride))
                return calls.ToDictionary(c => c, c => CallCheckResponse.FromSuccess());

            if ((guildData.BlackListed?.Length ?? 0) != 0)
                if (guildData.BlackListed.Contains(Context.Channel.Id))
                    return calls.ToDictionary(c => c, c => CallCheckResponse.FromError(null));

            var cmdPerms = calls.ToDictionary(c => c, c => Context.Database.CmdPerms.GetCmdPerm(Context.Guild.Id, c.PermissionKey).Result);

            return cmdPerms.ToDictionary(c => c.Key, c => 
            {
                if ((c.Value?.blackListed?.Length ?? 0) != 0)
                    if (c.Value.blackListed.Contains(Context.Channel.Id))
                        return CallCheckResponse.FromError(null);

                if (c.Value?.permissionId == null && c.Value?.roleIds == null)
                {
                    if (guildUser.HasAll(c.Value?.permissionId ?? c.Key.DefaultPermissions))
                        return CallCheckResponse.FromSuccess();
                }

                if (c.Value?.roleIds != null && guildUser.RoleIds.Any(r => c.Value?.roleIds?.Contains(r) ?? false))
                    return CallCheckResponse.FromSuccess();

                if (c.Value?.permissionId != null && guildUser.HasAll(c.Value?.permissionId ?? c.Key.DefaultPermissions))
                    return CallCheckResponse.FromSuccess();

                return CallCheckResponse.FromError($"{Context.User.Mention} You do not have the required permissions to use that command");
            });
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
            lock (_instanceCommandLock)
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
                await Context.Logger.Log(e, $"Command: {GetType().Name}");
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

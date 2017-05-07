using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.TypeReaders;

namespace TitanBot2.Services.CommandService
{
    public abstract class Command
    {
        public string Group { get; protected set; }
        public string Name { get; protected set; }
        public string[] Alias { get; protected set; } = new string[0];
        public string Description { get; protected set; }
        public string[] Usage { get; protected set; } = new string[0];
        public bool RequireOwner { get; protected set; } = false;
        public ContextType RequiredContexts { get; protected set; } = ContextType.DM | ContextType.Group | ContextType.Guild;
        public ulong DefaultPermission { get; protected set; } = 0;
        public Dictionary<string, Func<Task>> SubCommands { get; protected set; } = new Dictionary<string, Func<Task>>();
        protected TypeReaderCollection Readers { get; set; }

        protected CommandCheckResponse CheckResult { get; private set; }

        public TitanbotCmdContext Context { get; private set; }

        public Command(TitanbotCmdContext context, TypeReaderCollection readers)
        {
            Context = context;
            Readers = readers;
        }

        public async Task ExecuteAsync(TypeReaderCollection readers)
        {
            var start = DateTime.Now;
            await Context.Logger.Log(new LogEntry(LogType.Handler, LogSeverity.Debug, $"Enter Command | mId: {Context.Message.Id} | uId: {Context.User.Id} | cId: {Context.Channel.Id}", "Commands"));

            try
            {
                if (Context.Arguments.Length > 0)
                {
                    var subCommand = SubCommands.FirstOrDefault(c => Regex.IsMatch(Context.Arguments.First(), c.Key, RegexOptions.IgnoreCase));
                    if (subCommand.Value != null)
                        await subCommand.Value();
                    else
                        await RunAsync();
                }
                else
                    await RunAsync();
            }
            catch (Exception ex)
            {
                await Context.Logger.Log(ex, "Command:" +(Name ?? GetType().Name));
            }

            await Context.Logger.Log(new LogEntry(LogType.Handler, LogSeverity.Debug, $"Exit Command  | mId: {Context.Message.Id} | {(DateTime.Now - start).TotalMilliseconds}ms", "Commands"));
        }

        protected abstract Task RunAsync();

        public virtual async Task<CommandCheckResponse> CheckCommandAsync()
        {
            var results = new List<CommandCheckResponse>();

            results.Add(await CheckContexts(RequiredContexts));
            if (RequireOwner)
                results.Add(await CheckOwner());
            results.Add(await CheckPermissions(DefaultPermission));
            results.Add(await CheckArguments());

            if (results.Any(r => !r.IsSuccess))
                return CommandCheckResponse.FromError(string.Join("\n", results.Where(r => !r.IsSuccess).Select(r => r.Message)));

            return CommandCheckResponse.FromSuccess();
        }

        protected virtual Task<CommandCheckResponse> CheckArguments()
        {
            return Task.FromResult(CommandCheckResponse.FromSuccess());
        }

        protected virtual Task<CommandCheckResponse> CheckContexts(ContextType contexts)
        {
            bool isValid = false;

            if ((contexts & ContextType.Guild) != 0)
                isValid = isValid || Context.Channel is IGuildChannel;
            if ((contexts & ContextType.DM) != 0)
                isValid = isValid || Context.Channel is IDMChannel;
            if ((contexts & ContextType.Group) != 0)
                isValid = isValid || Context.Channel is IGroupChannel;

            if (isValid)
                return Task.FromResult(CommandCheckResponse.FromSuccess());
            else
                return Task.FromResult(CommandCheckResponse.FromError($"Invalid context for command! Accepted contexts: {RequiredContexts}"));
        }

        protected virtual async Task<CommandCheckResponse> CheckOwner()
        {
            switch (Context.Client.TokenType)
            {
                case TokenType.Bot:
                    var application = await Context.Client.GetApplicationInfoAsync();
                    if (Context.User.Id != application.Owner.Id)
                        return CommandCheckResponse.FromError("Command can only be run by the owner of the bot");
                    return CommandCheckResponse.FromSuccess();
                case TokenType.User:
                    if (Context.User.Id != Context.Client.CurrentUser.Id)
                        return CommandCheckResponse.FromError("Command can only be run by the owner of the bot");
                    return CommandCheckResponse.FromSuccess();
                default:
                    return CommandCheckResponse.FromError($"{nameof(RequireOwnerAttribute)} is not supported by this {nameof(TokenType)}.");
            }
        }

        protected virtual async Task<CommandCheckResponse> CheckPermissions(ulong defaultPerm)
        {
            var owner = Context.TitanBot?.Owner ?? (await Context.Client.GetApplicationInfoAsync()).Owner;

            if (Context.User.Id == owner.Id || Context.Channel is IDMChannel || Context.Channel is IGroupChannel)
                return CommandCheckResponse.FromSuccess();

            if (Context.Guild.OwnerId == Context.User.Id)
                return CommandCheckResponse.FromSuccess();

            var guildData = await Context.Database.Guilds.GetGuild(Context.Guild.Id);
            var guildUser = Context.User as IGuildUser;

            if (guildUser.HasAll(guildData.PermOverride))
                return CommandCheckResponse.FromSuccess();

            if ((guildData.BlackListed?.Length ?? 0) != 0)
                if (guildData.BlackListed.Contains(Context.Channel.Id))
                    return CommandCheckResponse.FromError(null);

            var cmdPerm = (await Context.Database.CmdPerms.GetCmdPerm(Context.Guild.Id, Context.Command));

            if (cmdPerm?.permissionId == null && cmdPerm?.roleIds == null)
            {
                if (guildUser.HasAll(cmdPerm?.permissionId ?? defaultPerm))
                    return CommandCheckResponse.FromSuccess();
            }

            if ((cmdPerm?.blackListed?.Length ?? 0) != 0)
                if (cmdPerm.blackListed.Contains(Context.Channel.Id))
                    return CommandCheckResponse.FromError(null);

            if (cmdPerm?.roleIds != null && guildUser.RoleIds.Any(r => cmdPerm?.roleIds?.Contains(r) ?? false))
                return CommandCheckResponse.FromSuccess();

            if (cmdPerm?.permissionId != null && guildUser.HasAll(cmdPerm?.permissionId ?? defaultPerm))
                return CommandCheckResponse.FromSuccess();

            return CommandCheckResponse.FromError($"{Context.User.Mention} You do not have the required permissions to use that command");
        }

        protected Task<IUserMessage> ReplyAsync(string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return Context.Channel.SendMessageSafeAsync(message, ex => Context.Logger.Log(ex, "TitanBotModule"), isTTS, embed, options);
        }

        protected Task<IUserMessage> ReplyAsync(string message, Func<Exception, Task> handler, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return Context.Channel.SendMessageSafeAsync(message, handler, isTTS, embed, options);
        }

        protected async Task<TypeReaderResponse<T>> ReadAndReplyError<T>(int argId)
        {
            if (Context.Arguments.Length <= argId)
            {
                await ReplyAsync($"{Res.Str.ErrorText} Missing argument #{argId + 1}");
                return TypeReaderResponse.FromError<T>($"Missing argument #{argId + 1}");
            }

            string value = Context.Arguments[argId];

            var res = await Readers.Read<T>(Context, value);
            if (!res.IsSuccess)
                await ReplyAsync($"{Res.Str.ErrorText} {res.Message}");
            return res;
        }
    }
}

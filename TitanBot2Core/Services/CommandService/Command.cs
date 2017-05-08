using Discord;
using DC = Discord.Commands;
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
        public List<string> Alias { get; protected set; } = new List<string>();
        public string Description { get; protected set; }
        public List<string> Usage { get; protected set; } = new List<string>();
        public bool RequireOwner { get; protected set; } = false;
        public DC.ContextType RequiredContexts { get; protected set; } = DC.ContextType.DM | DC.ContextType.Group | DC.ContextType.Guild;
        public ulong DefaultPermission { get; protected set; } = 0;
        public CallCollection Calls = new CallCollection();
        protected TypeReaderCollection Readers { get; set; }

        protected CommandCheckResponse CheckResult { get; private set; }

        public TitanbotCmdContext Context { get; private set; }

        public Command(TitanbotCmdContext context, TypeReaderCollection readers)
        {
            Context = context;
            Readers = readers;

            Group = GetType().Namespace.Split('.').Last();
            Name = GetType().Name;
            if (Name.EndsWith("Command"))
                Name = Name.Substring(0, Name.Length - "Command".Length);
        }

        public async Task ExecuteAsync()
        {
            var start = DateTime.Now;
            await Context.Logger.Log(new LogEntry(LogType.Handler, LogSeverity.Debug, $"Enter Command | mId: {Context.Message.Id} | uId: {Context.User.Id} | cId: {Context.Channel.Id}", "Commands"));

            try
            {
                var executionContexts = Calls.Select(c => c.BuildExecutionContext(Context.Arguments)).Where(c => c != null).ToArray();

                await Task.WhenAll(executionContexts.Select(e => e.Parse(Readers, Context)));

                var canProceed = executionContexts.Where(e => e.IsSuccess);
                var failed = executionContexts.Where(e => !e.IsSuccess);

                if (canProceed.Count() > 0)
                    await canProceed.First().Execute(Context);
                else if (failed.Count() == 1)
                    if (failed.First().Results.Count(r => !r.IsSuccess) > 1)
                        await ReplyAsync($"{Res.Str.ErrorText} Multiple errors:\n" + string.Join("\n", failed.First().Results.Where(r => !r.IsSuccess).Select(r => r.Message)));
                    else
                        await ReplyAsync($"{Res.Str.ErrorText} {failed.First().Results.First(r => !r.IsSuccess).Message}");
                else
                    await ReplyAsync($"{Res.Str.ErrorText} The arguments you gave could not be matched to a method. Please use `{Context.Prefix}help {Name}` for more info");
            }
            catch (Exception ex)
            {
                await Context.Logger.Log(ex, "Command:" +(Name ?? GetType().Name));
            }

            await Context.Logger.Log(new LogEntry(LogType.Handler, LogSeverity.Debug, $"Exit Command  | mId: {Context.Message.Id} | {(DateTime.Now - start).TotalMilliseconds}ms", "Commands"));
        }

        public virtual async Task<CommandCheckResponse> CheckCommandAsync()
        {
            var results = new List<CommandCheckResponse>();

            results.Add(await CheckContexts(RequiredContexts));
            if (RequireOwner)
                results.Add(await CheckOwner());
            results.Add(await CheckPermissions(DefaultPermission));

            if (results.Any(r => !r.IsSuccess))
                return CommandCheckResponse.FromError(string.Join("\n", results.Where(r => !r.IsSuccess).Select(r => r.Message)));

            return CommandCheckResponse.FromSuccess();
        }

        protected virtual Task<CommandCheckResponse> CheckContexts(DC.ContextType contexts)
        {
            bool isValid = false;

            if ((contexts & DC.ContextType.Guild) != 0)
                isValid = isValid || Context.Channel is IGuildChannel;
            if ((contexts & DC.ContextType.DM) != 0)
                isValid = isValid || Context.Channel is IDMChannel;
            if ((contexts & DC.ContextType.Group) != 0)
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
                    return CommandCheckResponse.FromError($"{nameof(TokenType)} applications do not have an owner");
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

        protected async Task<TypeReaderResult> ReadAndReplyError<T>(int argId)
        {
            if (Context.Arguments.Length <= argId)
            {
                await ReplyAsync($"{Res.Str.ErrorText} Missing argument #{argId + 1}");
                return TypeReaders.TypeReaderResult.FromError($"Missing argument #{argId + 1}");
            }

            string value = Context.Arguments[argId];

            var res = await Readers.Read(typeof(T), Context, value);
            if (!res.IsSuccess)
                await ReplyAsync($"{Res.Str.ErrorText} {res.Message}");
            return res;
        }
    }
}

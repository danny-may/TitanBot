using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Commands.Responses;
using TitanBot.Contexts;
using TitanBot.Dependencies;
using TitanBot.Formatting;
using TitanBot.Formatting.Interfaces;
using TitanBot.Logging;
using TitanBot.Replying;
using TitanBot.Settings;
using TitanBot.Storage;
using TitanBot.TypeReaders;
using TitanBot.Util;
using static TitanBot.TBLocalisation.Logic;

namespace TitanBot.Commands
{
    public class CommandProcessor
    {
        private IDependencyFactory DependencyFactory { get; }
        private ICommandService Owner { get; }
        private IPermissionManager PermissionManager { get; }
        private IDatabase Database { get; }
        private ILogger Logger { get; }
        private ITypeReaderCollection Readers { get; }
        private ITextResourceManager TextManager { get; }
        private ISettingManager Settings { get; }

        public CommandProcessor(IDependencyFactory factory,
                               ICommandService owner,
                               IPermissionManager permissionManager,
                               IDatabase database,
                               ILogger logger,
                               ITypeReaderCollection typeReaders,
                               ISettingManager settings)
        {
            DependencyFactory = factory;
            Owner = owner;
            PermissionManager = permissionManager;
            Database = database;
            Logger = logger;
            Readers = typeReaders;
            Settings = settings;
        }

        private void ReplyAsync(ICommandContext context, string text)
            => ReplyAsync(context, new LocalisedString(text));
        private void ReplyAsync(ICommandContext context, string text, ReplyType replyType)
            => ReplyAsync(context, new LocalisedString(text, replyType));
        private void ReplyAsync(ICommandContext context, string text, params object[] values)
            => ReplyAsync(context, new LocalisedString(text, values));
        private void ReplyAsync(ICommandContext context, string text, ReplyType replyType, params object[] values)
            => ReplyAsync(context, new LocalisedString(text, replyType, values));

        private async void ReplyAsync(ICommandContext context, ILocalisable<string> message)
            => await context.Replier.Reply(context.Channel, context.Author).WithMessage(message).SendAsync();

        public bool ShouldRun(ICommandContext context, out Func<Task> call)
        {
            call = null;
            try
            {
                if (!context.IsCommand && !context.ExplicitPrefix)
                    return false;
                else if (!context.IsCommand)
                    ReplyAsync(context, COMMANDEXECUTOR_COMMAND_UNKNOWN, ReplyType.Error, context.Prefix, context.CommandText);
                return TryGetExecutor(context, out call);
            }
            catch (Exception ex)
            {
                var record = new Error
                {
                    Channel = context.Channel.Id,
                    Message = context.Message.Id,
                    User = context.Author.Id,
                    Description = $"{ex.GetType()}\n{ex.Message}",
                    Content = ex.ToString(),
                    Type = ex.GetType().Name
                };
                Database.Insert(record).Wait();
                try
                {
                    ReplyAsync(context, COMMANDEXECUTOR_EXCEPTION_ALERT, ReplyType.Error, ex.GetType().Name, record.Id);
                }
                catch { }
                return false;
            }
        }

        private bool TryGetExecutor(ICommandContext context, out Func<Task> call)
        {
            call = null;
            if (context.Command == null)
                return false;

            var command = context.Command.Value;

            var instance = DependencyFactory.WithInstance(context).Construct(command.CommandType) as Command;
            if (instance == null)
                throw new InvalidOperationException($"Unable to create an instance of the {command.CommandType} command");
            ArgumentCheckResponse response = default(ArgumentCheckResponse);
            var calls = PermissionManager.CheckContext(context, command.Calls.ToArray());
            if (calls.Count() == 0)
            {
                ReplyAsync(context, COMMANDEXECUTOR_DISALLOWED_CHANNEL, ReplyType.Error);
                return false;
            }
            var subCalls = CheckSubcommands(context, calls);
            if (subCalls.Count() == 0)
            {
                ReplyAsync(context, COMMANDEXECUTOR_SUBCALL_UNKNOWN, ReplyType.Error, context.Prefix, context.CommandText);
                return false;
            }
            foreach (var subCall in subCalls)
            {
                response = MatchArguments(context, subCall.CallInfo, subCall.CallName).Result;
                if (response.SuccessStatus == ArgumentCheckResult.Successful)
                {
                    var permCheck = PermissionManager.CheckAllowed(context, new CallInfo[] { subCall.CallInfo });
                    if (!permCheck.IsSuccess)
                    {
                        if (permCheck.ErrorMessage != null)
                            ReplyAsync(context, permCheck.ErrorMessage, ReplyType.Error, context.Prefix, context.CommandText);
                    }
                    else
                    {
                        call = () =>
                        {
                            instance.Install(context, DependencyFactory);
                            foreach (var buildEvent in Owner.GetBuildEvents(instance.GetType()))
                                buildEvent.Invoke(instance);
                            LogCommand(context, subCall.CallInfo);
                            if (subCall.CallInfo.ShowTyping)
                                context.Channel.TriggerTypingAsync().Wait();

                            return (Task)subCall.CallInfo.Method.Invoke(instance, response.CallArguments);
                        };
                        return true;
                    }
                    return false;
                }
            }
            ReplyAsync(context, response.ErrorMessage.message, ReplyType.Error, new object[] { context.Prefix, context.CommandText }
                                                                                            .Concat(response.ErrorMessage.values.Select(v => v(context.TextResource)))
                                                                                            .ToArray());
            return false;
        }

        private void LogCommand(ICommandContext context, CallInfo call)
        {
            var record = new CommandRecord
            {
                AuthorId = context.Author.Id,
                MessageId = context.Message.Id,
                ChannelId = context.Channel.Id,
                GuildId = context.Guild?.Id,
                MessageContent = context.Message.Content,
                UserName = context.Author.Username,
                ChannelName = context.Channel.Name,
                GuildName = context.Guild?.Name,
                TimeStamp = context.Message.Timestamp,
                Prefix = context.Prefix,
                CommandName = context.Command?.Name
            };
            Logger.Log(Logging.LogSeverity.Debug, LogType.Command, $"{(record.GuildName != null ? $"{record.GuildName} ({record.GuildId}) \n" : "")}#{record.ChannelName} ({record.ChannelId})\n{record.UserName} ({record.AuthorId})\n{record.CommandName}", "CommandService");
            Database.Insert(record);
        }

        private (string CallName, CallInfo CallInfo)[] CheckSubcommands(ICommandContext context, CallInfo[] calls)
        {
            var allCalls = calls.Select(c => (CallName: c.SubCall, CallInfo: c))
                                .Concat(calls.Where(c => c.Aliases != null)
                                             .SelectMany(c => c.Aliases.Select(a => (CallName: a, CallInfo: c))));
            var noSubcalls = allCalls.Where(c => c.CallName == null);
            var subcalls = allCalls.Where(c => c.CallName != null)
                                   .GroupBy(c => c.CallName)
                                   .OrderByDescending(g => g.Key.Length);
            foreach (var group in subcalls)
            {
                if (context.Message.Content.Substring(context.ArgPos).ToLower().Trim().StartsWith(group.Key.ToLower()))
                    return group.ToArray();
            }
            return noSubcalls.ToArray();
        }

        private async ValueTask<ArgumentCheckResponse> MatchArguments(ICommandContext context, CallInfo call, string callName)
        {
            var reader = Readers.NewCache();
            var responses = new List<ArgumentCheckResponse>();
            foreach (var argPattern in call.ArgumentPermatations.OrderByDescending(p => p.Count(a => !a.UseDefault)))
            {
                var readResult = await ReadArguments(context, reader, argPattern, call, callName);
                if (readResult.SuccessStatus == ArgumentCheckResult.Successful)
                    return readResult;
                responses.Add(readResult);
            }
            return responses.OrderBy(r => r.SuccessStatus).First();
        }

        private async ValueTask<ArgumentCheckResponse> ReadArguments(ICommandContext context, ITypeReaderCollection reader, ArgumentInfo[] argPattern, CallInfo call, string callName)
        {
            int? denseIndex = argPattern.IndexOf(p => p.IsDense);
            if (denseIndex == -1) denseIndex = null;
            if (callName != null) denseIndex++;
            var maxLength = denseIndex.HasValue ? argPattern.Count(a => !a.UseDefault) + (callName != null ? 1 : 0) : (int?)null;
            var stringArgs = context.SplitArguments(call.Flags.Count() == 0, out var flags, maxLength, denseIndex);
            var argStrings = stringArgs.Select(s => s.Trim()).ToArray();

            if (callName != null)
            {
                if (argStrings.Length == 0)
                    return ArgumentCheckResponse.FromError(ArgumentCheckResult.InvalidSubcall, COMMANDEXECUTOR_SUBCALL_UNSPECIFIED);
                if (callName.ToLower() != argStrings[0].ToLower())
                    return ArgumentCheckResponse.FromError(ArgumentCheckResult.InvalidSubcall, COMMANDEXECUTOR_SUBCALL_UNKNOWN);
                argStrings = argStrings.Skip(1).ToArray();
            }

            var acceptedLength = argPattern.Count(a => !a.UseDefault);
            if (argStrings.Length > acceptedLength)
                return ArgumentCheckResponse.FromError(ArgumentCheckResult.TooManyArguments, COMMANDEXECUTOR_ARGUMENTS_TOOMANY);
            else if (argStrings.Length < acceptedLength)
                return ArgumentCheckResponse.FromError(ArgumentCheckResult.NotEnoughArguments, COMMANDEXECUTOR_ARGUMENTS_TOOFEW);

            var argResults = new object[argPattern.Length];
            var argIterator = argStrings.GetEnumerator();
            for (int i = 0; i < argResults.Length; i++)
            {
                if (argPattern[i].UseDefault)
                    argResults[i] = argPattern[i].DefaultValue;
                else if (argPattern[i].IsRawArgument)
                    argResults[i] = context.Message.Content.Substring(context.ArgPos);
                else
                {
                    if (!argIterator.MoveNext())
                        return ArgumentCheckResponse.FromError(ArgumentCheckResult.NotEnoughArguments, COMMANDEXECUTOR_ARGUMENTS_TOOFEW);
                    var readRes = await reader.Read(argPattern[i].Type, context, (string)argIterator.Current);
                    if (!readRes.IsSuccess)
                        return ArgumentCheckResponse.FromError(readRes);
                    argResults[i] = readRes.Best;
                }
            }

            var flagResults = call.Flags.Select(f => f.DefaultValue).ToArray();
            for (int i = 0; i < call.Flags.Length; i++)
            {
                var flag = call.Flags[i];
                var flagVals = flags.Where(f => f.Key.ToLower() == flag.LongKey?.ToLower() ||
                                                f.Key.ToLower() == flag.ShortKey.ToString().ToLower());
                foreach (var val in flagVals)
                {
                    if (!flag.RequiresInput)
                        flagResults[i] = !(bool)flag.DefaultValue;
                    else
                    {
                        var readResult = await reader.Read(flag.Type, context, val.Value);
                        if (readResult.IsSuccess)
                        {
                            flagResults[i] = readResult.Best;
                            break;
                        }
                    }
                }
            }

            return ArgumentCheckResponse.FromSuccess(argResults, flagResults);
        }
    }
}

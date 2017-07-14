using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Commands.Responses;
using TitanBot.Dependencies;
using TitanBot.Logging;
using TitanBot.Settings;
using TitanBot.Storage;
using TitanBot.TextResource;
using TitanBot.TypeReaders;
using TitanBot.Util;

namespace TitanBot.Commands
{
    class CommandExecutor
    {
        private IDependencyFactory DependencyFactory { get; }
        private IPermissionManager PermissionManager { get; }
        private IDatabase Database { get; }
        private ILogger Logger { get; }
        private ITypeReaderCollection Readers { get; }
        private ICommandContext Context { get; }
        private ITextResourceManager TextManager { get; }
        private ITextResourceCollection TextResource { get; }
        private ISettingsManager Settings { get; }
        private IReplier Replier { get; }

        public CommandExecutor(IDependencyFactory factory, 
                               ICommandContext context, 
                               IPermissionManager permissionManager,
                               IDatabase database,
                               ILogger logger,
                               ITypeReaderCollection typeReaders,
                               ISettingsManager settings)
        {
            DependencyFactory = factory;
            PermissionManager = permissionManager;
            Context = context;
            Database = database;
            Logger = logger;
            Readers = typeReaders;
            Settings = settings;
            Replier = Context.Replier;
            TextResource = Context.TextResource;
        }

        public async Task Run()
        {
            if (!Context.IsCommand && !Context.ExplicitPrefix)
                return;
            else if (!Context.IsCommand)
                await Replier.Reply(Context.Channel, Context.Author)
                             .WithMessage("COMMANDEXECUTOR_COMMAND_UNKNOWN", ReplyType.Error, Context.Prefix, Context.CommandText)
                             .SendAsync();
            await TryExecute();
        }

        private async Task TryExecute()
        {
            if (Context.Command == null)
                return;

            var command = Context.Command.Value;

            var instance = DependencyFactory.WithInstance(Context).Construct(command.CommandType) as Command;
            if (instance == null)
                throw new InvalidOperationException($"Unable to create an instance of the {command.CommandType} command");
            ArgumentCheckResponse response = default(ArgumentCheckResponse);
            var calls = PermissionManager.CheckContext(Context, command.Calls.ToArray());
            if (calls.Count() == 0)
            {
                await Replier.Reply(Context.Channel, Context.Author)
                             .WithMessage("COMMANDEXECUTOR_DISALLOWED_CHANNEL", ReplyType.Error)
                             .SendAsync();
                return;
            }
            calls = CheckSubcommands(calls);
            if (calls.Count() == 0)
            {
                await Replier.Reply(Context.Channel, Context.Author)
                             .WithMessage("COMMANDEXECUTOR_SUBCALL_UNKNOWN", ReplyType.Error, Context.Prefix, Context.CommandText)
                             .SendAsync();
                return;
            }
            foreach (var call in calls)
            {
                response = await MatchArguments(call);
                if (response.SuccessStatus == ArgumentCheckResult.Successful)
                {
                    var permCheck = PermissionManager.CheckAllowed(Context, new CallInfo[] { call });
                    if (!permCheck.IsSuccess)
                    {
                        if (permCheck.ErrorMessage != null)
                            await Replier.Reply(Context.Channel, Context.Author)
                                         .WithMessage(permCheck.ErrorMessage, ReplyType.Error, Context.Prefix, Context.CommandText)
                                         .SendAsync();
                    }
                    else
                    {
                        try
                        {
                            instance.Install(Context, DependencyFactory);
                            LogCommand(call);
                            using (Context.Channel.EnterTypingState())
                            {
                                await (Task)call.Call.Invoke(instance, response.CallArguments);
                            }
                        }
                        catch (Exception ex)
                        {
                            var record = new Error
                            {
                                Channel = Context.Channel.Id,
                                Message = Context.Message.Id,
                                User = Context.Author.Id,
                                Content = JsonConvert.SerializeObject(ex)
                            };
                            await Database.Insert(record);
                            await Replier.Reply(Context.Channel, Context.Author)
                                         .WithMessage("COMMANDEXECUTOR_EXCEPTION_ALERT", ReplyType.Error, record.Id)
                                         .SendAsync();
                        }
                    }
                    return;
                }
            }
            await Replier.Reply(Context.Channel, Context.Author)
                         .WithMessage(response.ErrorMessage.message, ReplyType.Error, new object[] { Context.Prefix, Context.CommandText}
                                                                                            .Concat(response.ErrorMessage.values.Select(v => v(TextResource)))
                                                                                            .ToArray())
                         .SendAsync();
        }

        private void LogCommand(CallInfo call)
        {
            var record = new CommandRecord
            {
                AuthorId = Context.Author.Id,
                MessageId = Context.Message.Id,
                ChannelId = Context.Channel.Id,
                GuildId = Context.Guild?.Id,
                MessageContent = Context.Message.Content,
                UserName = Context.Author.Username,
                ChannelName = Context.Channel.Name,
                GuildName = Context.Guild?.Name,
                TimeStamp = Context.Message.Timestamp,
                Prefix = Context.Prefix,
                CommandName = Context.Command?.Name
            };
            Logger.LogAsync(LogSeverity.Debug, LogType.Command, $"{(record.GuildName != null ? $"{record.GuildName} ({record.GuildId}) \n" : "")}#{record.ChannelName} ({record.ChannelId})\n{record.UserName} ({record.AuthorId})\n{record.CommandName}", "CommandService");
            Database.Insert(record);
        }

        private CallInfo[] CheckSubcommands(CallInfo[] calls)
        {
            var noSubcalls = calls.Where(c => c.SubCall == null);
            var subcalls = calls.Where(c => c.SubCall != null).GroupBy(c => c.SubCall).OrderByDescending(g => g.Key.Length);
            foreach (var group in subcalls)
            {
                if (Context.Message.Content.Substring(Context.ArgPos).ToLower().Trim().StartsWith(group.Key.ToLower()))
                    return group.ToArray();
            }
            return noSubcalls.ToArray();
        }

        private async Task<ArgumentCheckResponse> MatchArguments(CallInfo call)
        {
            var reader = Readers.NewCache();
            var responses = new List<ArgumentCheckResponse>();
            foreach (var argPattern in call.ArgumentPermatations.OrderByDescending(p => p.Count(a => !a.UseDefault)))
            {
                var denseIndex = argPattern.Select((a, i) => a.IsDense ? i : (int?)null).FirstOrDefault(i => i != null);
                if (call.SubCall != null)
                    denseIndex++;
                var maxLength = denseIndex.HasValue ? argPattern.Count(a => !a.UseDefault) + (call.SubCall != null ? 1 : 0) : (int?)null;
                var stringArgs = Context.SplitArguments(call.Flags.Count() == 0, out (string Key, string Value)[] flagStrings, maxLength, denseIndex);
                var readResult = await ReadArguments(reader, stringArgs.Select(s => s.Trim()).ToArray(), flagStrings, argPattern, call);
                if (readResult.SuccessStatus == ArgumentCheckResult.Successful)
                    return readResult;
                responses.Add(readResult);
            }
            return responses.OrderByDescending(r => r.SuccessStatus).Last();
        }

        private async Task<ArgumentCheckResponse> ReadArguments(ITypeReaderCollection reader, string[] argStrings, (string Key, string Value)[] flags, ArgumentInfo[] argPattern, CallInfo call)
        {
            if (call.SubCall != null)
            {
                if (argStrings.Length == 0)
                    return ArgumentCheckResponse.FromError(ArgumentCheckResult.InvalidSubcall, "COMMANDEXECUTOR_SUBCALL_UNSPECIFIED");
                if (call.SubCall.ToLower() != argStrings[0].ToLower())
                    return ArgumentCheckResponse.FromError(ArgumentCheckResult.InvalidSubcall, "COMMANDEXECUTOR_SUBCALL_UNKNOWN");
                argStrings = argStrings.Skip(1).ToArray();
            }

            var acceptedLength = argPattern.Count(a => !a.UseDefault);
            if (argStrings.Length > acceptedLength)
                return ArgumentCheckResponse.FromError(ArgumentCheckResult.TooManyArguments, "COMMANDEXECUTOR_ARGUMENTS_TOOMANY");
            else if (argStrings.Length < acceptedLength)
                return ArgumentCheckResponse.FromError(ArgumentCheckResult.NotEnoughArguments, "COMMANDEXECUTOR_ARGUMENTS_TOOFEW");

            var argResults = new object[argPattern.Length];
            var argIterator = argStrings.GetEnumerator();
            for (int i = 0; i < argResults.Length; i++)
            {
                if (argPattern[i].UseDefault)
                    argResults[i] = argPattern[i].DefaultValue;
                else if (argPattern[i].IsRawArgument)
                    argResults[i] = Context.Message.Content.Substring(Context.ArgPos);
                else
                {
                    if (!argIterator.MoveNext())
                        return ArgumentCheckResponse.FromError(ArgumentCheckResult.NotEnoughArguments, "COMMANDEXECUTOR_ARGUMENTS_TOOFEW");
                    var readRes = await reader.Read(argPattern[i].Type, Context, (string)argIterator.Current);
                    if (!readRes.IsSuccess)
                        return ArgumentCheckResponse.FromError(ArgumentCheckResult.ArgumentMismatch, readRes.Message.message, readRes.Message.values);
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
                        flagResults[i] = true;
                    else
                    {
                        var readResult = await reader.Read(flag.Type, Context, val.Value);
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

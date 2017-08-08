using Discord;
using Discord.WebSocket;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot.Contexts;
using TitanBot.Downloader;
using TitanBot.Formatting;
using TitanBot.Logging;
using TitanBot.Replying;
using TitanBot.Scheduling;
using TitanBot.Settings;
using TitanBot.Storage;
using TitanBot.Util;
using static TitanBot.TBLocalisation.Help;
using static TitanBot.TBLocalisation.Commands;
using TitanBot.Formatting.Interfaces;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description(Desc.EXEC)]
    [Alias("Eval", "Run")]
    [RequireOwner]
    [Notes(Notes.EXEC)]
    public class ExecCommand : Command
    {
        protected override int DelayMessageMs => 10000;

        public static Func<ExecCommand, object> GetGlobals = c => new ExecGlobals(c);

        public Assembly[] GetAssemblies()
        {
            var assemblies = new HashSet<Assembly> { Assembly.GetEntryAssembly() };
            foreach (var assembly in Assembly.GetEntryAssembly().GetReferencedAssemblies())
            {
                if (assembly.Name.Contains(".CodeAnalysis") || assembly.Name.Contains(".Reflection"))
                    continue;
                assemblies.Add(Assembly.Load(assembly));
            }
            return assemblies.ToArray();
        }

        public string[] GetNamespaces(Assembly[] assemblies)
        {
            return assemblies.SelectMany(a => a.GetTypes())
                             .Select(t => t.Namespace)
                             .Distinct()
                             .Where(n => !string.IsNullOrWhiteSpace(n))
                             .Where(n => n.All(c => "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz._".Contains(c)))
                             .ToArray();
        }

        bool TryConstruct(string code, Assembly[] assemblies, Type globals, out TimeSpan constructTime, out object result)
        {
            (result, constructTime) = MiscUtil.TimeExecution<object>(() => {
                try
                {
                    return CSharpScript.Create(code, ScriptOptions.Default.WithReferences(assemblies), globals);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            });
            return !(result is Exception);
        }

        protected virtual bool TryCompile(Script<object> script, out TimeSpan compileTime, out object result)
        {
            (result, compileTime) = MiscUtil.TimeExecution<object>(() => {
                try
                {
                    return script.Compile();
                }
                catch (Exception ex)
                {
                    return ex;
                }
            });
            return !(result is Exception);
        }

        protected virtual bool TryExecute(Script<object> script, object globals, out TimeSpan execTime, out object result)
        {
            (result, execTime) = MiscUtil.TimeExecution(() => {
                try
                {
                    return script.RunAsync(globals).Result?.ReturnValue;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            });
            return !(result is Exception);
        }

        [Call]
        [Usage(Usage.EXEC)]
        protected virtual async Task ExecAsync([Dense, RawArguments]string code)
        {
            var assemblies = GetAssemblies();
            var namespaces = GetNamespaces(assemblies);

            var test = string.Join("\n", assemblies.SelectMany(a => a.GetTypes())
                                                   .Select(t => t.Name)
                                                   .Where(t => t.All(c => "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz._".Contains(c)))
                                                   .OrderBy(x => x));

            var codeWithUsings = string.Join("\n", namespaces.Select(n => $"using {n};")) + "\n" + code;
            var globals = GetGlobals(this);
            object result = null;

            ILocalisable<string> footerText;

            if (!TryConstruct(codeWithUsings, assemblies.ToArray(), globals.GetType(), out var constructTime, out var constructRes))
            {
                result = constructRes;
                footerText = new LocalisedString(ExecText.FOOTER_CONSTRUCTFAILED, constructTime);
            }
            else if (!TryCompile(constructRes as Script<object>, out var compileTime, out var compileRes))
            {
                result = compileRes;
                footerText = new LocalisedString(ExecText.FOOTER_COMPILEFAILED, constructTime, compileTime);
            }
            else if (!TryExecute(constructRes as Script<object>, globals, out var execTime, out result))
            {
                footerText = new LocalisedString(ExecText.FOOTER_EXECUTEFAILED, constructTime, compileTime, execTime);
            }
            else
            {
                footerText = new LocalisedString(ExecText.FOOTER_SUCCESS, constructTime, compileTime, execTime);
            }

            var builder = new LocalisedEmbedBuilder
            {
                Footer = new LocalisedFooterBuilder
                {
                    Text = footerText
                }
            }.AddField(f => f.WithName(TBLocalisation.INPUT).WithValue(ExecText.INPUT_FORMAT, code));

            if (result is Exception exception)
            {
                builder.WithTitle(ExecText.TITLE_EXCEPTION);
                builder.WithColor(System.Drawing.Color.Red.ToDiscord());
                var exceptions = new List<Exception>();
                if (exception is AggregateException aggex)
                    exceptions.AddRange(aggex.InnerExceptions);
                else
                    exceptions.Add(exception);
                foreach (var ex in exceptions)
                {
                    builder.AddField(f => f.WithName(TBLocalisation.ERROR).WithValue(ExecText.OUTPUT_FORMAT, Format.Sanitize(ex.GetType().ToString()), Format.Sanitize(ex.Message)));
                }
            }
            else
            {
                builder.WithTitle(ExecText.TITLE_SUCCESS);
                builder.WithColor(System.Drawing.Color.LimeGreen.ToDiscord());
                if (result == null)
                    builder.AddField(f => f.WithName(TBLocalisation.OUTPUT).WithValue(ExecText.OUTPUT_NULL, null, null));
                else
                {
                    var resString = "";
                    if (result is IEnumerable && !(result is string))
                        resString = "[" + string.Join(", ", (result as IEnumerable<object>) ?? new List<string>()) + "]";
                    else
                        resString = result?.ToString();
                    builder.AddField(f => f.WithName(TBLocalisation.OUTPUT).WithName(ExecText.OUTPUT_FORMAT, Format.Sanitize(result?.GetType().ToString() ?? ""), Format.Sanitize(resString ?? "")));
                }
            }

            await ReplyAsync(builder);
        }

        public class ExecGlobals
        {
            public IMessageContext Context { get; }
            public BotClient Bot { get; }
            public DiscordSocketClient Client { get; }
            public ILogger Logger { get; }
            public ICommandService CommandService { get; }
            public ISettingManager SettingsManager { get; }
            public GeneralGuildSetting GeneralGuildSetting => GuildSettings.Get<GeneralGuildSetting>();
            public GeneralUserSetting GeneralUserSetting => UserSettings.Get<GeneralUserSetting>();
            public GeneralGlobalSetting GeneralGlobalSetting => GlobalSettings.Get<GeneralGlobalSetting>();
            public ISettingContext GlobalSettings { get; }
            public ISettingContext ChannelSettings { get; }
            public ISettingContext UserSettings { get; }
            public ISettingContext GuildSettings { get; }
            public IDatabase Database { get; }
            public IScheduler Scheduler { get; }
            public IDownloader Downloader { get; }
            public ITextResourceManager TextManager { get; }

            private Func<IReplyContext> ReplyBase { get; }
            private Func<IUser, IReplyContext> ReplyUser { get; }
            private Func<IMessageChannel, IReplyContext> ReplyChannel { get; }
            private Func<IMessageChannel, IUser, IReplyContext> ReplyBoth { get; }
            private Func<IUserMessage, IModifyContext> ModifyBase { get; }
            private Func<IUserMessage, IUser, IModifyContext> ModifyUser { get; }

            public IReplyContext Reply() => ReplyBase();
            public IReplyContext Reply(IMessageChannel channel) => ReplyChannel(channel);
            public IReplyContext Reply(IUser user) => ReplyUser(user);
            public IReplyContext Reply(IMessageChannel channel, IUser user)  => ReplyBoth(channel, user);

            public IModifyContext Modify(IUserMessage message) => ModifyBase(message);
            public IModifyContext Modify(IUserMessage message, IUser user) => ModifyUser(message, user);

            public ExecGlobals(ExecCommand parent)
            {
                Context = parent.Context;
                Client = parent.Client;
                Bot = parent.Bot;
                Logger = parent.Logger;
                CommandService = parent.CommandService;
                SettingsManager = parent.SettingsManager;
                TextManager = Bot.DependencyFactory.GetOrStore<ITextResourceManager>();

                Database = parent.Database;
                Scheduler = parent.Scheduler;
                Downloader = parent.Downloader;

                ReplyBase = parent.Reply;
                ReplyChannel = parent.Reply;
                ReplyUser = parent.Reply;
                ReplyBoth = parent.Reply;
                ModifyBase = parent.Modify;
                ModifyUser = parent.Modify;
            }
        }
    }
}

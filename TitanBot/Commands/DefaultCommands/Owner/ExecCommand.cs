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
using TitanBot.Downloader;
using TitanBot.Formatter;
using TitanBot.Logging;
using TitanBot.Scheduling;
using TitanBot.Settings;
using TitanBot.Storage;
using TitanBot.Util;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description("Allows for arbitrary code execution")]
    [Alias("Eval", "Run")]
    [RequireOwner]
    [Notes("https://github.com/Titansmasher/TitanBot/blob/rewrite/TitanBotBase/Commands/DefaultCommands/Owner/ExecCommand.cs#L111")]
    public class ExecCommand : Command
    {
        protected override int DelayMessageMs => 10000;

        ICommandContext Context { get; }
        public ExecCommand(ICommandContext context)
        {
            Context = context;
        }

        public static Func<ExecCommand, ExecGlobals> GetGlobals = c => new ExecGlobals(c);

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
        [Usage("Executes arbitrary code")]
        protected virtual async Task ExecAsync([Dense, RawArguments]string code)
        {
            var assemblies = GetAssemblies();
            var namespaces = GetNamespaces(assemblies);

            var codeWithUsings = string.Join("\n", namespaces.Select(n => $"using {n};")) + "\n" + code;
            var globals = GetGlobals(this);
            object result = null;

            string footerText = "";

            if (!TryConstruct(codeWithUsings, assemblies.ToArray(), globals.GetType(), out var constructTime, out var constructRes))
            {
                result = constructRes;
                footerText = "Failed to construct";
            }
            else if (!TryCompile(constructRes as Script<object>, out var compileTime, out var compileRes))
            {
                result = compileRes;
                footerText = "Failed to compile";
            }
            else if (!TryExecute(constructRes as Script<object>, globals, out var execTime, out result))
            {
                footerText = "Failed to execute";
            }
            else
            {
                footerText = $"Constructed in: {constructTime.TotalMilliseconds}ms | Compiled in: {compileTime.TotalMilliseconds}ms | Executed in: {execTime.TotalMilliseconds}ms";
            }

            var builder = new EmbedBuilder
            {
                Footer = new EmbedFooterBuilder
                {
                    Text = footerText
                }
            }.AddField("Input", $"```csharp\n{code}\n```");

            if (result is Exception exception)
            {
                builder.WithTitle($":no_entry_sign: Execution Result");
                builder.WithColor(System.Drawing.Color.Red.ToDiscord());
                var exceptions = new List<Exception>();
                if (exception is AggregateException aggex)
                    exceptions.AddRange(aggex.InnerExceptions);
                else
                    exceptions.Add(exception);
                foreach (var ex in exceptions)
                {
                    builder.AddField("Error", $"Type: {Format.Sanitize(ex.GetType().ToString())}\n```csharp\n{Format.Sanitize(ex.Message)}\n```");
                }
            }
            else
            {
                builder.WithTitle($":white_check_mark: Execution Result");
                builder.WithColor(System.Drawing.Color.LimeGreen.ToDiscord());
                if (result == null)
                    builder.AddField("Output", "No output from execution...");
                else
                {
                    var resString = "";
                    if (result is IEnumerable && !(result is string))
                        resString = "[" + string.Join(", ", (result as IEnumerable<object>) ?? new List<string>()) + "]";
                    else
                        resString = result?.ToString();
                    builder.AddField("Output", $"Type: {Format.Sanitize(result?.GetType().ToString() ?? "")}\n```csharp\n{Format.Sanitize(resString ?? "")}\n```");
                }
            }

            await ReplyAsync("", embed: builder.Build());
        }

        public class ExecGlobals
        {
            public ICommandContext Context { get; }
            public BotClient Bot { get; }
            public DiscordSocketClient Client { get; }
            public ILogger Logger { get; }
            public ICommandService CommandService { get; }
            public ISettingsManager SettingsManager { get; }
            public GeneralSettings GuildData { get; }
            public IDatabase Database { get; }
            public IScheduler Scheduler { get; }
            public IReplier Replier { get; }
            public IDownloader Downloader { get; }
            public OutputFormatter Formatter { get; }

            public ExecGlobals(ExecCommand parent)
            {
                Context = parent.Context;
                Client = parent.Client;
                Bot = parent.Bot;
                Logger = parent.Logger;
                CommandService = parent.CommandService;
                SettingsManager = parent.SettingsManager;
                GuildData = parent.GuildData;
                Database = parent.Database;
                Scheduler = parent.Scheduler;
                Replier = parent.Replier;
                Downloader = parent.Downloader;
                Formatter = parent.Formatter;
            }
        }
    }
}

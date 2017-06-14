using Discord;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Database;
using TitanBotBase.Downloader;
using TitanBotBase.Formatter;
using TitanBotBase.Logger;
using TitanBotBase.Scheduler;
using TitanBotBase.Settings;
using TitanBotBase.Util;

namespace TitanBotBase.Commands.DefaultCommands.Owner
{
    [Description("Allows for arbitrary code execution")]
    [Alias("Eval", "Run")]
    [RequireOwner]
    [Notes("")]
    public class ExecCommand : Command
    {
        ICommandContext Context { get; }
        public ExecCommand(ICommandContext context)
        {
            Context = context;
        }

        [Call]
        [Usage("Executes arbitrary code")]
        async Task ExecAsync([Dense, RawArguments]string code)
        {
            var assemblies = new HashSet<Assembly> { Assembly.GetEntryAssembly() };
            foreach (var assembly in Assembly.GetEntryAssembly().GetReferencedAssemblies())
            {
                if (assembly.Name.Contains(".CodeAnalysis") || assembly.Name.Contains(".Reflection"))
                    continue;
                assemblies.Add(Assembly.Load(assembly));
            }

            var namespaces = assemblies.SelectMany(a => a.GetTypes())
                                       .Select(t => t.Namespace)
                                       .Distinct()
                                       .Where(n => !string.IsNullOrWhiteSpace(n))
                                       .Where(n => n.All(c => "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz._".Contains(c)));

            var codeWithUsings = string.Join("\n", namespaces.Select(n => $"using {n};")) + "\n" + code;

            object result = null;
            object execResult = null;
            Script<object> script = null;
            ImmutableArray<Diagnostic> compileDiagnostics = new ImmutableArray<Diagnostic>();
            TimeSpan? constructTime, compileTime, execTime;
            constructTime = compileTime = execTime = null;

            var globals = new ExecGlobals(this);

            try
            {
                (script, constructTime) = MiscUtil.TimeExecution(() => CSharpScript.Create(codeWithUsings, ScriptOptions.Default.WithReferences(assemblies.ToArray()), typeof(ExecGlobals)));
                (compileDiagnostics, compileTime) = MiscUtil.TimeExecution(() => script.Compile());
                (execResult, execTime) = MiscUtil.TimeExecution(() => script?.RunAsync(globals).Result?.ReturnValue ?? result);
                result = execResult;
            }
            catch (Exception ex)
            {
                result = ex;
            }

            var footerText = $"Constructed in: {(constructTime != null ? constructTime.Value.TotalMilliseconds + "ms" : "Failed to construct")}";
            if (constructTime != null)
                footerText += $" | Compiled in: {(compileTime != null ? compileTime.Value.TotalMilliseconds + "ms" : "Failed to compile")}";
            if (constructTime != null && compileTime != null)
                footerText += $" | Executed in: {(execTime != null ? execTime.Value.TotalMilliseconds + "ms" : "Failed to execute")}";

            var builder = new EmbedBuilder
            {
                Footer = new EmbedFooterBuilder
                {
                    Text = footerText
                }
            }.AddField("Input", $"```csharp\n{code}\n```");

            if (result is Exception)
            {
                builder.WithTitle($":no_entry_sign: Execution Result");
                builder.WithColor(System.Drawing.Color.Red.ToDiscord());
                builder.AddField("Output", $"Type: {Format.Sanitize(result.GetType().ToString())}\n```csharp\n{Format.Sanitize((result as Exception).Message)}\n```");
            }
            else
            {
                builder.WithTitle($":white_check_mark: Execution Result");
                builder.WithColor(System.Drawing.Color.LimeGreen.ToDiscord());
                var resString = "";
                if (result is IEnumerable && !(result is string))
                    resString = "[" + string.Join(", ", (result as IEnumerable<object>) ?? new List<string>()) + "]";
                else
                    resString = result?.ToString();
                builder.AddField("Output", $"Type: {Format.Sanitize(result?.GetType().ToString() ?? "")}\n```csharp\n{Format.Sanitize(resString ?? "")}\n```");
            }

            await ReplyAsync("", embed: builder.Build());
        }

        public class ExecGlobals
        {
            public ICommandContext Context { get; }
            public BotClient Bot { get; }
            public ILogger Logger { get; }
            public ICommandService CommandService { get; }
            public ISettingsManager SettingsManager { get; }
            public GeneralSettings GuildData { get; }
            public IDatabase Database { get; }
            public IScheduler Scheduler { get; }
            public IReplier Replier { get; }
            public IDownloader Downloader { get; }
            public OutputFormatter Formatter { get; }

            internal ExecGlobals(ExecCommand parent)
            {
                Context = parent.Context;
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

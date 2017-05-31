using Discord;
using Discord.WebSocket;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.Owner
{
    [Description("Allows for arbitrary code execution")]
    [RequireOwner]
    class ExecCommand : Command
    {

        [Call]
        [Usage("Executes arbitrary code")]
        async Task ExecAsync([Dense]string code)
        {
            code = Context.Message.Content.Substring(Context.Prefix.Length + Context.Command.Name.Length + 1);

            var codeWithUsings = $@"using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Discord.WebSocket;
using TitanBot2;
using TitanBot2.Commands;
using TitanBot2.Extensions;
using TitanBot2.Services.Database;
using TitanBot2.Services.Database.Extensions;
using TitanBot2.Services.Database.Models;
using System.Threading.Tasks;
{code}";
            var globals = new ExecGlobals
            {
                Context = Context
            };

            object result = null;

            Script<object> compiled = null;

            var compileStart = DateTime.Now;
            try
            {

                compiled = CSharpScript.Create(codeWithUsings,
                                                   ScriptOptions.Default.WithReferences(typeof(EmbedBuilder).Assembly,
                                                                                        typeof(Enumerable).Assembly,
                                                                                        typeof(ArrayList).Assembly,
                                                                                        typeof(AsyncEnumerator).Assembly,
                                                                                        typeof(CmdContext).Assembly,
                                                                                        typeof(DiscordSocketClient).Assembly,
                                                                                        typeof(MiscExtensions).Assembly,
                                                                                        typeof(File).Assembly,
                                                                                        Assembly.Load("System.Threading.Tasks, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                                                   typeof(ExecGlobals));
                compiled.Compile();
            }
            catch (Exception ex)
            { result = ex; }

            var compileDone = DateTime.Now;
            var executeStart = DateTime.Now;
            try
            {
                result = (await compiled?.RunAsync(globals))?.ReturnValue ?? result;
            }
            catch (Exception ex)
            { result = ex; }
            var executeDone = DateTime.Now;

            var builder = new EmbedBuilder
            {
                Footer = new EmbedFooterBuilder
                {
                    Text = $"Compiled in {(compileDone - compileDone).TotalMilliseconds}ms | Executed in {(executeDone - executeStart).TotalMilliseconds}ms"
                }
            };

            builder.AddField("Input", $"```csharp\n{code}\n```");

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

            await ReplyAsync("", embed: builder.Build(), handler: ex => Context.BotClient.Logger.Log(ex, "ExecModule"));
        }

        class ExecGlobals
        {
            public CmdContext Context { get; set; }
        }
    }
}

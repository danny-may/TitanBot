using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;

namespace TitanBot2.Modules.Owner
{
    public partial class OwnerModule
    {
        [Group("Exec"), Alias("Eval")]
        [Summary("Allows arbitrary code execution")]
        public class ExecModule : TitanBotModule
        {
            [Command(RunMode = RunMode.Async)]
            [Remarks("Executes the given code as best as possible")]
            public async Task ExecAsync([Remainder]string code)
            {
                var codeWithUsings = $@"using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Discord.WebSocket;
using TitanBot2;
using TitanBot2.Modules;
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

                object result;

                var start = DateTime.Now;
                try
                {
                    result = await CSharpScript.EvaluateAsync(codeWithUsings,
                                                                  ScriptOptions.Default.WithReferences(typeof(EmbedBuilder).Assembly,
                                                                                                       typeof(CommandAttribute).Assembly,
                                                                                                       typeof(Enumerable).Assembly,
                                                                                                       typeof(ArrayList).Assembly,
                                                                                                       typeof(AsyncEnumerator).Assembly,
                                                                                                       typeof(TitanbotCmdContext).Assembly,
                                                                                                       typeof(DiscordSocketClient).Assembly,
                                                                                                       typeof(MiscExtensions).Assembly,
                                                                                                       Assembly.Load("System.Threading.Tasks, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                                                                  new ExecGlobals { Context = Context },
                                                                  typeof(ExecGlobals));
                }
                catch (Exception ex)
                {
                    result = ex;
                }
                

                var builder = new EmbedBuilder
                {
                    Footer = new EmbedFooterBuilder
                    {
                        Text = $"Executed in {(DateTime.Now - start).TotalMilliseconds}ms"
                    }
                };

                builder.AddField("Input", $"```csharp\n{code}\n```");

                if (result is Exception)
                {
                    builder.WithTitle($":no_entry_sign: Execution Result");
                    builder.WithColor(System.Drawing.Color.Red.ToDiscord());
                    builder.AddField("Output", $"Type: {result.GetType()}\n```csharp\n{(result as Exception).Message}\n```");
                }
                else
                {
                    builder.WithTitle($":white_check_mark: Execution Result");
                    builder.WithColor(System.Drawing.Color.LimeGreen.ToDiscord());
                    builder.AddField("Output", $"Type: {result?.GetType()}\n```csharp\n{result?.ToString()}\n```");
                }

                await ReplyAsync("", embed: builder.Build(), handler: ex => Context.TitanBot.Logger.Log(ex, "ExecModule"));
            }
        }

        public class ExecGlobals
        {
            public TitanbotCmdContext Context { get; set; }
        }
    }
}

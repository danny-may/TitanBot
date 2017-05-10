using Discord;
using Discord.WebSocket;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Owner
{
    public class ExecCommand : Command
    {
        public ExecCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => ExecAsync((string)a[0]))
                 .WithArgTypes(typeof(string))
                 .WithItemAsParams(0);

            RequireOwner = true;
            Usage.Add("`{0} <code>` - Executes arbitrary code");
            Description = "Allows for arbitrary code execution";
        }

        public async Task ExecAsync(string code)
        {
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

            object result;

            var start = DateTime.Now;
            try
            {
                result = await CSharpScript.EvaluateAsync(codeWithUsings,
                                                              ScriptOptions.Default.WithReferences(typeof(EmbedBuilder).Assembly,
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

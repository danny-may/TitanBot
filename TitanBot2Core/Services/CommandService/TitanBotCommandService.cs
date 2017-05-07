using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Services
{
    public class TitanBotCommandService
    {
        public IReadOnlyList<CommandInfo> Commands { get { return _commands; } }
        private List<CommandInfo> _commands = new List<CommandInfo>();
        private TypeReaderCollection _readers = new TypeReaderCollection();

        private object _lock = new object();

        public Task Install(Assembly assembly)
        {
            return Task.Run(() =>
            {
                lock (_lock)
                {
                    var commandQuery = assembly.GetTypes()
                                            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Command)));
                    foreach (var type in commandQuery)
                    {
                        _commands.AddRange(CommandInfo.FromType(type));
                    }

                    _readers.Install(assembly);
                }
            });
        }

        public async Task ExecuteAsync(TitanbotCmdContext context, int argPos)
        {
            if (context.Command == null)
                return;

            context.CommandService = this;

            var matches = Commands.Where(c => c.Name.Trim().ToLower() == context.Command.Trim().ToLower());

            var cmdInfos = matches.GroupBy(m => m.CommandType);
            
            var responses = new Dictionary<Command, CommandCheckResponse>();

            foreach (var cmdInfo in cmdInfos)
            {
                var command = cmdInfo.First().CreateInstance(context, _readers);
                responses.Add(command, await command.CheckCommandAsync());
            }

            if (!responses.Any(r => r.Value.IsSuccess))
            {
                foreach (var response in responses)
                {
                    if (response.Value.Message != null)
                        await context.Channel.SendMessageSafeAsync($"{Res.Str.ErrorText} {response.Value.Message}");
                }
                return;
            }

            var successCommand = responses.Where(r => r.Value.IsSuccess).Select(r => r.Key).SingleOrDefault();

            if (successCommand == null)
                await context.Channel.SendMessageSafeAsync($"{Res.Str.ErrorText} I was unable to determine which command to run!");
            else
                try
                {
                    Task.Run(() => successCommand.ExecuteAsync(_readers));
                }
                catch (Exception ex)
                {
                    await context.Logger.Log(ex, successCommand.Name);
                }
        }
    }
}

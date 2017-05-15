using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Models;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;
using TitanBot2.TypeReaders.Readers;

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
                        var cmdInfo = CommandInfo.FromType(type);
                        if (cmdInfo != null)
                            _commands.Add(cmdInfo);
                    }

                    _readers.AddTypeReader<Artifact>(new ArtifactTypeReader());
                    _readers.AddTypeReader<Equipment>(new EquipmentTypeReader());
                    _readers.AddTypeReader<Pet>(new PetTypeReader());
                    _readers.AddTypeReader<Helper>(new HelperTypeReader());
                    _readers.AddTypeReader<TimeSpan>(new TimeSpanTypeReader());
                }
            });
        }

        public async Task ExecuteAsync(TitanbotCmdContext context, int argPos)
        {
            if (context.Command == null)
                return;

            context.CommandService = this;

            var cmdInfos = Commands.Where(c => c.Alias.Select(a => a.ToLower()).Contains(context.Command.ToLower()));
            
            var responses = new Dictionary<Command, CommandCheckResponse>();

            foreach (var cmdInfo in cmdInfos)
            {
                var command = cmdInfo.CreateInstance(context, _readers);
                responses.Add(command, await command.CheckCommandAsync());
            }

            if (!responses.Any(r => r.Value.IsSuccess))
            {
                foreach (var response in responses)
                {
                    if (!string.IsNullOrWhiteSpace(response.Value.Message))
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
                    Task.Run(() => successCommand.ExecuteAsync());
                }
                catch (Exception ex)
                {
                    await context.Logger.Log(ex, successCommand.Name);
                }
        }
    }
}

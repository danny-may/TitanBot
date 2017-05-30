using System;
using System.Collections.Generic;
using System.Drawing;
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
using TitanBot2.Services.CommandService.Models;
using TitanBot2.Responses;

namespace TitanBot2.Services
{
    public class BotCommandService
    {
        public IReadOnlyList<CommandInfo> Commands { get { return _calls; } }
        private List<CommandInfo> _calls = new List<CommandInfo>();
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
                        _calls.Add(CommandInfo.FromType(type));
                    }

                    _readers.AddTypeReader<Artifact>(new ArtifactTypeReader());
                    _readers.AddTypeReader<Equipment>(new EquipmentTypeReader());
                    _readers.AddTypeReader<Pet>(new PetTypeReader());
                    _readers.AddTypeReader<Helper>(new HelperTypeReader());
                    _readers.AddTypeReader<TimeSpan>(new TimeSpanTypeReader());
                    _readers.AddTypeReader<Color>(new ColourTypeReader());
                }
            });
        }

        public void ExecuteAsync(CmdContext context)
        {
            Task.Run(async () =>
            {

                if (context.Command == null)
                    return;

                context.CommandService = this;

                var allowed = await FindAllowed(context, context.Command);

                if (allowed.Count() == 0 && context.ExplicitCommand)
                {
                    await context.Channel.SendMessageSafeAsync(Command.FormatMessage("That command either does not exist, or you do not have permission to use it!", Command.ReplyType.Error));
                    return;
                }
                else if (allowed.Count() != 1)
                    return;

                var inst = allowed.First().Key.CreateInstance(context, _readers);
                var calls = allowed.First();

                Dictionary<CallInfo, SignatureMatchResponse> validationResults;
                var reader = _readers.NewCache();
                var validators = calls.ToDictionary(c => c, c => c.ValidateSignature(context, reader));
                await Task.WhenAll(validators.Select(v => v.Value));
                validationResults = validators.ToDictionary(v => v.Key, v => v.Value.Result);

                var valid = validationResults.Where(r => r.Value.IsSuccess).OrderByDescending(v => v.Value.Weight);
                var invalid = validationResults.Where(r => !r.Value.IsSuccess);

                if (valid.Count() == 0)
                {
                    await context.Channel.SendMessageSafeAsync(Command.FormatMessage($"There was an issue with the arguments you gave. Try using `{context.Prefix}help {calls.Key.Name}` for usage", Command.ReplyType.Error));
                    return;
                }

                var success = false;
                foreach (var call in valid)
                {
                    foreach (var arg in call.Value.Arguments)
                    {
                        success = await inst.ExecuteAsync(call.Key, arg, call.Value.Flags);
                        if (success)
                            break;
                    }
                    if (success)
                        break;
                }
            });
        }

        public async Task<IEnumerable<IGrouping<CommandInfo, CallInfo>>> FindAllowed(CmdContext context, string command = null)
            => (await FindCommand(context, command)).Where(c => c.Value.IsSuccess)
                                                    .Select(c => c.Key)
                                                    .GroupBy(c => c.ParentInfo);

        public async Task<Dictionary<CallInfo, CallCheckResponse>> FindCommand(CmdContext context, string command = null)
        {
            var cmdInfos = Commands.Where(c => command == null || c.Alias.Select(a => a.ToLower()).Contains(command.ToLower()) || c.Name.ToLower() == command.ToLower());

            var checkResults = cmdInfos.Select(c => c.CheckCalls(context, _readers));
            return (await Task.WhenAll(checkResults)).SelectMany(c => c).ToDictionary(c => c.Key, c => c.Value);
            
        }
    }
}

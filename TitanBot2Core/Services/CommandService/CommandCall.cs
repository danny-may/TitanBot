using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.TypeReaders;

namespace TitanBot2.Services.CommandService
{
    public class CommandCall
    {
        public Func<object[], Task> Execute { get; private set; }
        public string[] SubCommand { get; private set; } = new string[0];
        public Type[] ArgTypes { get; private set; } = new Type[0];
        public int? ParamsLocation { get; private set; }
        public int AcceptsCount => ArgTypes.Length + SubCommand.Length;

        public CommandCall(Func<object[], Task> call)
        {
            Execute = call;
        }

        public CommandCall WithSubCommand(params string[] subCommands)
        {
            SubCommand = subCommands ?? new string[0];
            return this;
        }

        public CommandCall WithArgTypes(params Type[] argTypes)
        {
            ArgTypes = argTypes ?? new Type[0];
            return this;
        }

        public CommandCall WithItemAsParams(int? paramsLocation)
        {
            ParamsLocation = paramsLocation;
            return this;
        }

        public ExecutionContext BuildExecutionContext(string[] args)
        {
            if (AcceptsCount > args.Length)
                return null;
            if (AcceptsCount < args.Length && ParamsLocation == null)
                return null;

            var subCommandStrings = args.Take(SubCommand.Length).ToArray();
            var argStrings = args.Skip(SubCommand.Length).Squeeze(ArgTypes.Length, ParamsLocation ?? -1).ToArray();

            return new ExecutionContext(Execute,
                ArgTypes.MemberwisePair(argStrings).Select(p => new ArgTypePair(p.Item1, p.Item2)).ToArray(),
                SubCommand.MemberwisePair(subCommandStrings).Select(p => new ArgSubcommandPair(p.Item1, p.Item2)).ToArray());
        }

        public class ExecutionContext
        {
            public Func<object[], Task> Call { get; }
            public ArgTypePair[] Arguments { get; }
            public ArgSubcommandPair[] SubCommands { get; }
            public bool HasParsed { get; private set; }
            public bool IsSuccess => Results?.All(r => r.IsSuccess) ?? false;

            public TypeReaderResult[] Results { get; set; }

            internal ExecutionContext(Func<object[], Task> call, ArgTypePair[] args, ArgSubcommandPair[] subCommands)
            {
                Call = call;
                Arguments = args;
                SubCommands = subCommands;
            }

            public async Task Parse(TypeReaderCollection readers, CmdContext context)
            {
                if (!SubCommands.All(c => c.Matches()))
                    Results = new TypeReaderResult[] { TypeReaderResult.FromError("Could not match subcommand parameters") };
                else
                    Results = await Task.WhenAll(Arguments.Select(a => readers.Read(a.Type, context, a.Argument)));
                HasParsed = true;
            }

            public async Task Execute(CmdContext context)
            {
                if (!IsSuccess)
                    return;

                await Call(Results.Select(r => r.Best).ToArray());
            }
        }

        public struct ArgTypePair
        {
            public Type Type { get; }
            public string Argument { get; }
            internal ArgTypePair(Type type, string argument)
            {
                Type = type;
                Argument = argument;
            }
        }

        public struct ArgSubcommandPair
        {
            public string SubCommand { get; }
            public string Argument { get; }
            internal ArgSubcommandPair(string subCommand, string argument)
            {
                SubCommand = subCommand;
                Argument = argument;
            }

            public bool Matches()
                => SubCommand.ToLower() == Argument.ToLower();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Responses;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.Services.CommandService.Flags;
using TitanBot2.TypeReaders;

namespace TitanBot2.Services.CommandService.Models
{
    public class CallInfo
    {
        public MethodInfo Call { get; }
        public CommandInfo ParentInfo { get; }
        public string Usage => UsageAttribute.GetFrom(this);
        public ulong DefaultPermissions => DefaultPermissionAttribute.GetPerm(this);
        public string PermissionKey => DefaultPermissionAttribute.GetKey(this);
        public ContextType RequiredContexts => RequireContextAttribute.GetFrom(this);
        public bool RequireOwner => RequireOwnerAttribute.GetFrom(this);
        public FlagInfo[] Flags => CallFlagAttribute.GetFrom(this);
        public string[] Subcalls => CallAttribute.GetFrom(this);
        private ArgumentInfo[] Parameters { get; }
        private int? DenseIndex => Parameters.Select((a, i) => new { a, i }).SingleOrDefault(a => a.a.IsDense)?.i;

        public CallInfo(MethodInfo call, CommandInfo parent)
        {
            ParentInfo = parent;
            Call = call;

            Parameters = ArgumentInfo.FromCallInfo(this);

            ValidateCall();
        }

        private void ValidateCall()
        {
            var denseCount = Call.GetParameters().Count(p => DenseAttribute.GetFrom(p));
            if (denseCount > 1)
                throw new InvalidOperationException($"Cannot have more than 1 {typeof(DenseAttribute).Name} per call");
        }

        public string[] GetParameters(string requiredFormat = "<{0}>", string optionalFormat = "[{0}]", string arrayFormat = "{0}...")
        {
            var pars = Call.GetParameters().TakeWhile(p => p.GetCustomAttribute<CallFlagAttribute>() == null);

            var ret = new List<string>();

            foreach (var param in pars)
            {
                var paramFormat = param.IsOptional ? optionalFormat : requiredFormat;
                paramFormat = string.Format(paramFormat, typeof(IEnumerable<>).IsAssignableFrom(param.ParameterType) || param.ParameterType.IsArray ? arrayFormat : "{0}");
                ret.Add(string.Format(paramFormat, NameAttribute.GetFrom(param)));
            }

            return ret.ToArray();
        }

        public async Task<SignatureMatchResponse> ValidateSignature(CmdContext context)
        {
            if (!CheckSubcommand(context, out string[] args))
                return SignatureMatchResponse.FromError();

            var parsedArguments = await ReadArguments(context, args);

            var flags = new FlagCollection(context, Flags, context.Flags);

            return SignatureMatchResponse.FromSuccess(parsedArguments.ToArray(), flags);
        }

        private bool CheckSubcommand(CmdContext context, out string[] arguments)
        {
            arguments = null;
            if (Subcalls.Length > context.Arguments.Length)
                return false;

            var paired = Subcalls.Zip(context.Arguments, (s, a) => new { s, a });

            if (paired.Any(p => p.s.ToLower() != p.a.ToLower()))
                return false;
            
            arguments = context.Arguments.Skip(paired.Count()).ToArray();

            return true;
        }

        private async Task<ArgumentReadResponse[]> ReadArguments(CmdContext context, string[] args)
        {
            var res = new List<ArgumentReadResponse>();
            var temp = ArgumentPossibilities().ToArray();
            foreach (var argArray in ArgumentPossibilities())
            {
                IEnumerable<string> argFormatted;
                if (DenseIndex != null && argArray[DenseIndex ?? 0] != null)
                    argFormatted = args.Squeeze(argArray.Count(a => a != null), DenseIndex ?? -1);
                else
                    argFormatted = args.Cast<string>();

                if (args.Length < argArray.Count(a => a != null))
                {
                    res.Add(ArgumentReadResponse.FromError(ArgumentReadResponse.Reason.NotEnoughArguments));
                    continue;
                }
                if (argFormatted.Count() > argArray.Count(a => a != null))
                {
                    res.Add(ArgumentReadResponse.FromError(ArgumentReadResponse.Reason.TooManyArguments));
                    continue;
                }

                var argEnumerable = argFormatted.GetEnumerator();

                var readerResults = argArray.Select(async (a, i) =>
                {
                    if (a == null)
                        return TypeReaderResponse.FromSuccess(Parameters[i].DefaultValue);
                    argEnumerable.MoveNext();
                    return await context.Readers.Read(a.ArgType, context, argEnumerable.Current);
                });

                var readResults = await Task.WhenAll(readerResults);

                if (readResults.Any(r => !r.IsSuccess))
                    res.Add(ArgumentReadResponse.FromError(ArgumentReadResponse.Reason.IncorrectArgumentType));
                else
                    res.Add(ArgumentReadResponse.FromSuccess(readResults));

            }

            return res.ToArray();
        }

        private IEnumerable<ArgumentInfo[]> ArgumentPossibilities()
        {
            var required = Parameters.TakeWhile(p => !p.Optional);
            var optional = Parameters.SkipWhile(p => !p.Optional);

            var staticMask = new BitArray(required.Count(), true).Cast<bool>();
            var counter = (int)Math.Pow(2, optional.Count()) - 1;
            while (counter >= 0)
            {
                var changingMask = new BitArray(new int[] { counter }).Cast<bool>().Take(optional.Count());
                var mask = changingMask.Concat(staticMask).Reverse().ToArray();
                yield return Parameters.Zip(mask, (a,m) => m ? a : null).ToArray();
                counter--;
            }
        }

        public bool Matches(string name)
        {
            var namePath = name.ToLower().Split('.');
            var keyPath = PermissionKey.ToLower().Split('.');
            return namePath.Length <= keyPath.Length &&
                   namePath.Zip(keyPath, (n, k) => n == k).All(a => a);
        }

        public static CallInfo[] FromCommandInfo(CommandInfo t)
        {
            var calls = t.CommandType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                     .Where(m => m.ReturnType == typeof(Task))
                                     .Where(m => m.GetCustomAttributes(typeof(CallAttribute), false).Count() > 0);

            return calls.Select(c => new CallInfo(c, t)).ToArray();
        }
    }
}

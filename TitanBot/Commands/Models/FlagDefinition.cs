using System;
using System.Reflection;
using TitanBot.Formatting;
using TitanBot.Formatting.Interfaces;

namespace TitanBot.Commands
{
    public class FlagDefinition : ILocalisable<string>
    {
        public char ShortKey { get; }
        public string LongKey { get; }
        public string Description { get; }
        public Type Type { get; }
        public object DefaultValue { get; }
        public bool RequiresInput { get; }
        public ParameterInfo Parameter { get; }
        public string ParamName { get; }

        internal FlagDefinition(CallFlagAttribute attribute, ParameterInfo param)
        {
            if (!param.HasDefaultValue)
                throw new ArgumentException($"The parameter {param} does not have a default value.");
            DefaultValue = param.DefaultValue;
            ShortKey = attribute.ShortKey;
            LongKey = attribute.LongKey;
            Description = attribute.Description;
            Type = param.ParameterType;
            RequiresInput = Type != typeof(bool);
            Parameter = param;
            ParamName = NameAttribute.GetFor(param);
        }

        object ILocalisable.Localise(ITextResourceCollection textResource)
            => Localise(textResource);
        public string Localise(ITextResourceCollection textResource)
        {
            string Format(string key)
                => $"`{key}{(RequiresInput ? $" <{ParamName}>" : "")}`";

            var text = Format($"-{ShortKey}");
            if (LongKey != null)
                text += " / " + Format($"--{LongKey}");

            return text + $" = {textResource.GetResource(Description)}";
        }
    }
}

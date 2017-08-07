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

        internal FlagDefinition(CallFlagAttribute attribute, ParameterInfo param)
        {
            if (!param.HasDefaultValue)
                throw new ArgumentException($"The parameter {param} does not have a default value.");
            DefaultValue = param.DefaultValue;
            ShortKey = attribute.ShortKey;
            LongKey = attribute.LongKey;
            Description = attribute.Description;
            Type = param.ParameterType;
            RequiresInput = !(Type == typeof(bool) &&
                             (bool)param.DefaultValue == false);
        }

        object ILocalisable.Localise(ITextResourceCollection textResource)
            => Localise(textResource);
        public string Localise(ITextResourceCollection textResource)
        {
            if (LongKey != null)
                return $"`-{ShortKey}` / `--{LongKey}` = {textResource.GetResource(Description)}";
            else
                return $"`-{ShortKey}` = {textResource.GetResource(Description)}";
        }
    }
}

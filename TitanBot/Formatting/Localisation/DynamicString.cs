using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Formatting
{
    public class DynamicString : LocalisedString
    {
        private Func<ITextResourceCollection, string> LocalisationFunc { get; }
        public DynamicString(Func<ITextResourceCollection, string> localisationFunc) : base(localisationFunc.Method.Name)
        {
            LocalisationFunc = localisationFunc;
        }

        public override string Localise(ITextResourceCollection textResource)
            => LocalisationFunc?.Invoke(textResource);
        
        public static implicit operator DynamicString(Func<ITextResourceCollection, string> localisationFunc)
            => new DynamicString(localisationFunc);
    }
}

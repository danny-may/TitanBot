using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Formatting.Interfaces
{
    public interface ILocalisable<T> : ILocalisable
    {
        new T Localise(ITextResourceCollection textResource);
    }

    public interface ILocalisable
    {
        object Localise(ITextResourceCollection textResource);

    }
}

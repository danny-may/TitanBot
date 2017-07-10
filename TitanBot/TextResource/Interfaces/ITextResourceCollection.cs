using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.TextResource
{
    public interface ITextResourceCollection
    {
        string Format(string key, params object[] items);
        string GetResource(string key);
        string this[string index] { get; }
    }
}

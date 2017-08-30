using System;
using System.Collections.Generic;
using System.Text;

namespace TitanBot.Core.Services.Formatting
{
    public interface ILocalisable
    {
        object Localise(ILocalisationCollection localeManager);
    }

    public interface ILocalisable<T> : ILocalisable
    {
        new T Localise(ILocalisationCollection localeManager);
    }
}
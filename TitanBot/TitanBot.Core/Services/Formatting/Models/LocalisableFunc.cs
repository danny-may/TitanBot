using System;
using System.Collections.Generic;
using System.Text;

namespace TitanBot.Core.Services.Formatting.Models
{
    public class LocalisableFunc : ILocalisable<string>
    {
        public Func<ILocalisationCollection, string> Localisation { get; }

        private LocalisableFunc(Func<ILocalisationCollection, string> localisationFunc)
        {
            Localisation = localisationFunc ?? throw new ArgumentNullException(nameof(localisationFunc));
        }

        public static LocalisableFunc From(Func<ILocalisationCollection, string> func)
            => new LocalisableFunc(func);

        public static implicit operator LocalisableFunc(Func<ILocalisationCollection, string> func)
            => From(func);

        object ILocalisable.Localise(ILocalisationCollection localeManager)
            => Localise(localeManager);

        public string Localise(ILocalisationCollection localeManager)
            => Localisation(localeManager);
    }
}
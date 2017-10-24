using System;

namespace TitanBot.Core.Services.Formatting.Models
{
    public class TransFunc : IDisplayable<string>
    {
        #region Statics

        public static TransFunc From(Func<ITranslationSet, IValueFormatter, string> func)
            => new TransFunc(func);

        #endregion Statics

        #region Fields

        public readonly Func<ITranslationSet, IValueFormatter, string> Localisation;

        #endregion Fields

        #region Constructors

        private TransFunc(Func<ITranslationSet, IValueFormatter, string> localisationFunc)
        {
            Localisation = localisationFunc ?? throw new ArgumentNullException(nameof(localisationFunc));
        }

        #endregion Constructors

        #region Overrides

        public override string ToString()
            => Localisation.ToString();

        #endregion Overrides

        #region Operators

        public static implicit operator TransFunc(Func<ITranslationSet, IValueFormatter, string> func)
            => From(func);

        #endregion Operators

        #region IDisplayable

        object IDisplayable.Display(ITranslationSet translations, IValueFormatter formatter)
            => Display(translations, formatter);

        public string Display(ITranslationSet translations, IValueFormatter formatter)
            => Localisation(translations, formatter);

        #endregion IDisplayable
    }
}
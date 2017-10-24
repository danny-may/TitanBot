using System;

namespace TitanBot.Core.Services.Formatting.Models
{
    public class TransEmbedable : IDisplayable<IEmbedable>
    {
        #region Statics

        public static TransEmbedable Wrap(IEmbedable embedable)
            => new TransEmbedable(embedable);

        #endregion Statics

        #region Fields

        private IEmbedable _child;

        #endregion Fields

        #region Constructors

        private TransEmbedable(IEmbedable child)
        {
            _child = child;
        }

        #endregion Constructors

        #region IDisplayable

        object IDisplayable.Display(ITranslationSet translations, IValueFormatter formatter)
            => Display(translations, formatter);

        public IEmbedable Display(ITranslationSet translations, IValueFormatter formatter)
        {
            throw new NotImplementedException();
        }

        #endregion IDisplayable
    }
}
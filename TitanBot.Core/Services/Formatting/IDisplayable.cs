namespace TitanBot.Core.Services.Formatting
{
    public interface IDisplayable
    {
        object Display(ITranslationSet translations, IValueFormatter formatter);
    }

    public interface IDisplayable<T> : IDisplayable
    {
        new T Display(ITranslationSet translations, IValueFormatter formatter);
    }
}
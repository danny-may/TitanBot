using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Titansmasher.Services.Displaying.Interfaces
{
    public delegate string BeautifyDelegate<T>(T value, IDisplayService service, DisplayOptions options = default);

    public interface IDisplayService
    {
        IDisplayService AddTranslation(string key, string text);
        IDisplayService AddTranslation(string key, Language language, string text);

        IDisplayService AddFormatter<T>(BeautifyDelegate<T> formatter);
        IDisplayService AddFormatter<T>(BeautifyDelegate<T> formatter, FormatType format);

        void Import(Language language, JObject json);
        JObject Export(Language language);

        void ReloadLanguages();

        string GetTranslation(string key, IEnumerable<object> values, DisplayOptions options = default);
        string Beautify<T>(T value, DisplayOptions options = default);
    }
}
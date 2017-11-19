using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Titansmasher.Services.Display.Interfaces
{
    public delegate string BeautifyDelegate<T>(T value, IDisplayService service, DisplayOptions options = default);

    public interface IDisplayService
    {
        #region Translation

        Language[] KnownLanguages { get; }

        void LoadTranslationsFromAssembly(Assembly assembly);
        void LoadTranslationsFromAssembly(IEnumerable<Assembly> assemblies);
        void LoadAllAssemblyTranslations();
        void Import(Language language, JObject json);
        JObject Export(Language language);

        void ReloadLanguages();

        string GetTranslation(string key, IEnumerable<object> values, DisplayOptions options = default);

        #endregion Translation

        #region Formatting

        Format[] KnownFormats { get; }

        IDisplayService AddFormatter<T>(BeautifyDelegate<T> formatter);
        IDisplayService AddFormatter<T>(Format format, BeautifyDelegate<T> formatter);
        IDisplayService AddFormatter<T>(Func<T, string> formatter);
        IDisplayService AddFormatter<T>(Format format, Func<T, string> formatter);

        string Beautify(object value, DisplayOptions options = default);
        string Beautify(object value, Type formatAs, DisplayOptions options = default);
        string Beautify<T>(T value, DisplayOptions options = default);
        string[] Beautify(IEnumerable<object> values, DisplayOptions options = default);

        #endregion Formatting
    }
}
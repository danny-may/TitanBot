using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Titansmasher.Extensions;
using Titansmasher.Services.Display.Interfaces;

namespace Titansmasher.Services.Display
{
    public class DisplayService : IDisplayService
    {
        #region Delegates

        private delegate string BeautifyDelegate(object value, IDisplayService service, DisplayOptions options = default);

        #endregion Delegates

        #region Fields

        private readonly Dictionary<Language, JObject> _translations = new Dictionary<Language, JObject>();
        private readonly Dictionary<Format, Dictionary<Type, BeautifyDelegate>> _formatters = new Dictionary<Format, Dictionary<Type, BeautifyDelegate>>();
        private readonly DirectoryInfo _translationDirectory;
        private readonly Dictionary<Language, FileInfo> _translationFiles = new Dictionary<Language, FileInfo>();

        #endregion Fields

        #region Constructors

        public DisplayService(DirectoryInfo translationsFolder = null)
        {
            _translationDirectory = translationsFolder ?? new DirectoryInfo("./Translations/");
            LoadFromAssemblies();
            DiscoverLanguages();
            ReloadLanguages();
        }

        #endregion Constructors

        #region Methods

        private void LoadFromAssemblies()
        {
            var assemblies = Assembly.GetEntryAssembly()
                                     .GetReferencedAssemblies()
                                     .Select(n => Assembly.Load(n));
            foreach (var assembly in assemblies)
            {
                var resources = assembly.GetManifestResourceNames()
                                        .Where(r => r.EndsWith(".json") &&
                                                    r.ToLower().Contains("translation"));
                foreach (var resource in resources)
                {
                    var language = Language.Get(resource.RemoveEnd(".json").Split('.').Last());
                    using (var sr = new StreamReader(assembly.GetManifestResourceStream(resource)))
                        Import(language, JsonConvert.DeserializeObject(sr.ReadToEnd()) as JObject ?? new JObject());
                }
            }
        }

        #endregion Methods

        #region IDisplayService

        #region Translation

        public Language[] KnownLanguages => _translations.Keys.ToArray();

        public void Import(Language language, JObject json)
        {
            FileInfo languageFile;
            if (_translations.TryGetValue(language, out var stored))
                languageFile = _translationFiles[language];
            else
            {
                stored = new JObject();
                languageFile = new FileInfo(Path.Combine(_translationDirectory.FullName, language + ".json"));
                _translationFiles[language] = languageFile;
                _translations[language] = stored;
            }

            stored.Merge(json);

            languageFile.EnsureDirectory();
            languageFile.WriteAllText(stored.ToString(Formatting.Indented));
            ReloadLanguages();
        }

        public void ReloadLanguages()
        {
            _translations.Clear();
            _translationDirectory.EnsureExists();

            foreach (var language in _translationFiles)
                if (language.Value.Exists)
                    _translations.Add(language.Key, (JObject)JsonConvert.DeserializeObject(language.Value.ReadAllText()) ?? new JObject());
        }

        public void DiscoverLanguages()
        {
            _translationFiles.Clear();
            _translationDirectory.EnsureExists();

            foreach (var file in _translationDirectory.GetFiles())
                if (file.Extension == ".json")
                    _translationFiles.Add(Language.Get(file.GetFileNameWithoutExtension()), file);
        }

        public JObject Export(Language language)
            => _translations.TryGetValue(language, out var export) ? export : null;

        public string GetTranslation(string key, IEnumerable<object> values, DisplayOptions options = default)
        {
            options = options ?? new DisplayOptions();
            key = key?.ToLower() ?? throw new ArgumentNullException(nameof(key));

            if (!_translations.TryGetValue(options.Language, out var language))
                language = new JObject();

            if (!_translations.TryGetValue(Language.Default, out var defaultLang))
                defaultLang = new JObject();

            var token = language.SelectToken(key) as JValue ??
                        defaultLang.SelectToken(key) as JValue;

            if (token != null)
                return string.Format(token.Value<string>(), Beautify(values ?? new object[0], options));

            return $"The translation key `{key}` is unknown";
        }

        #endregion Translation

        public Format[] KnownFormats => _formatters.Keys.ToArray();

        public IDisplayService AddFormatter<T>(Func<T, string> formatter)
            => AddFormatter(Format.Default, formatter);

        public IDisplayService AddFormatter<T>(Format format, Func<T, string> formatter)
            => AddFormatter(format, (BeautifyDelegate<T>)((v, d, o) => formatter(v)));

        public IDisplayService AddFormatter<T>(BeautifyDelegate<T> formatter)
            => AddFormatter(Format.Default, formatter);

        public IDisplayService AddFormatter<T>(Format format, BeautifyDelegate<T> formatter)
        {
            if (!_formatters.ContainsKey(format))
                _formatters.Add(format, new Dictionary<Type, BeautifyDelegate>());
            _formatters[format][typeof(T)] =
                ((v, d, o) => v is T t
                              ? formatter(t, d, o)
                              : v.ToString());
            return this;
        }

        public string Beautify(object value, Type formatAs, DisplayOptions options = default)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (formatAs == null) throw new ArgumentNullException(nameof(formatAs));
            options = options ?? new DisplayOptions();

            if (!_formatters.ContainsKey(options.Format) ||
                !_formatters[options.Format].ContainsKey(formatAs))
                return value.ToString();

            return _formatters[options.Format][formatAs](value, this, options);
        }

        public string Beautify(object value, DisplayOptions options = default)
            => Beautify(value, value?.GetType(), options);

        public string Beautify<T>(T value, DisplayOptions options = default)
            => Beautify(value, typeof(T), options);

        public string[] Beautify(IEnumerable<object> values, DisplayOptions options = default)
            => values.Select(v => v is IDisplayable d
                                    ? d.Display(this, options)
                                    : v)
                     .Select(v => Beautify(v, options))
                     .ToArray();

        #endregion IDisplayService
    }
}
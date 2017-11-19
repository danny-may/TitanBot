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
        private readonly Dictionary<Language, JObject> _embeddedTranslations = new Dictionary<Language, JObject>();
        private readonly Dictionary<Language, FileInfo> _translationFiles = new Dictionary<Language, FileInfo>();
        private readonly DirectoryInfo _translationDirectory;
        private readonly Dictionary<Format, Dictionary<Type, BeautifyDelegate>> _formatters = new Dictionary<Format, Dictionary<Type, BeautifyDelegate>>();
        private readonly JsonMergeSettings _mergeSettings = new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Union
        };

        #endregion Fields

        #region Constructors

        public DisplayService(DirectoryInfo translationsFolder = null)
        {
            _translationDirectory = translationsFolder ?? new DirectoryInfo("./Translations/");
            ReloadLanguages();
        }

        #endregion Constructors

        #region Methods

        private void DiscorverStoredTranslations()
        {
            _translationFiles.Clear();
            _translationDirectory.EnsureExists();

            foreach (var file in _translationDirectory.GetFiles())
                if (file.Extension == ".json")
                    _translationFiles.Add(Language.Get(file.GetFileNameWithoutExtension()), file);
        }

        private void LoadFromAssemby(Assembly assembly)
        {
            var resourceNames = assembly.GetManifestResourceNames();
            var translationResources = resourceNames.Where(r => r.EndsWith(".json") && r.ToLower().Contains("translation"));

            foreach (var resource in translationResources)
            {
                var language = Language.Get(resource.RemoveEnd(".json").Split('.').Last());
                JObject json;
                using (var sr = new StreamReader(assembly.GetManifestResourceStream(resource)))
                    json = JsonConvert.DeserializeObject(sr.ReadToEnd()) as JObject ?? new JObject();

                if (!_embeddedTranslations.ContainsKey(language))
                    _embeddedTranslations[language] = new JObject();

                _embeddedTranslations[language].Merge(json, _mergeSettings);
            }
        }

        #endregion Methods

        #region IDisplayService

        #region Translation

        public Language[] KnownLanguages => _translations.Keys.ToArray();

        public void LoadTranslationsFromAssembly(Assembly assembly)
        {
            LoadFromAssemby(assembly);
            ReloadLanguages();
        }

        public void LoadTranslationsFromAssembly(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
                LoadFromAssemby(assembly);
            ReloadLanguages();
        }

        public void LoadAllAssemblyTranslations()
            => LoadTranslationsFromAssembly(Assembly.GetEntryAssembly()
                                                    .GetReferencedAssemblies()
                                                    .Select(n => Assembly.Load(n))
                                                    .Concat(new[] { Assembly.GetEntryAssembly() }));

        public void Import(Language language, JObject json)
        {
            if (!_translations.ContainsKey(language))
                _translations[language] = new JObject();

            if (!_translationFiles.ContainsKey(language))
                _translationFiles[language] = new FileInfo(Path.Combine(_translationDirectory.FullName, language + ".json"));

            _translations[language].Merge(json, _mergeSettings);
            _translationFiles[language].WriteAllText(_translations[language].ToString(Formatting.Indented));
        }

        public void ReloadLanguages()
        {
            DiscorverStoredTranslations();
            _translations.Clear();

            foreach (var stored in _translationFiles)
            {
                var json = JsonConvert.DeserializeObject(stored.Value.ReadAllText()) as JObject ?? new JObject();
                if (!_translations.ContainsKey(stored.Key))
                    _translations[stored.Key] = _embeddedTranslations.TryGetValue(stored.Key, out var current)
                                                    ? current.DeepClone() as JObject
                                                    : new JObject();

                _translations[stored.Key].Merge(json, _mergeSettings);
            }

            foreach (var stored in _translationFiles)
                stored.Value.WriteAllText(_translations[stored.Key].ToString(Formatting.Indented));
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
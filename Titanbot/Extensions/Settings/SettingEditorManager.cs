using Discord.WebSocket;
using System;
using System.Collections.Generic;
using Titanbot.Extensions.Settings.Interfaces;
using Titanbot.Settings.Interfaces;

namespace Titanbot.Extensions.Settings
{
    internal class SettingEditorManager : ISettingEditorManager
    {
        #region Statics

        private static Dictionary<ISettingManager, SettingEditorManager> _editorManagers
            = new Dictionary<ISettingManager, SettingEditorManager>();

        public static SettingEditorManager GetFor(ISettingManager manager)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            return _editorManagers.TryGetValue(manager, out var editor) ? editor : new SettingEditorManager(manager);
        }

        #endregion Statics

        #region Fields

        public DiscordSocketClient Client { get; }
        public IReadOnlyList<ISettingEditor> Editors => _editors.AsReadOnly();

        private List<ISettingEditor> _editors { get; }

        #endregion Fields

        #region Constructors

        private SettingEditorManager(ISettingManager manager)
        {
            Setttings = manager ?? throw new ArgumentNullException(nameof(manager));
            _editorManagers.Add(manager, this);
        }

        #endregion Constructors

        #region ISettingEditorManager

        public ISettingManager Setttings { get; }

        public ISettingEditor<TSetting> GetOrCreateEditor<TSetting>() where TSetting : class, new()
        {
            throw new NotImplementedException();
        }

        #endregion ISettingEditorManager
    }
}
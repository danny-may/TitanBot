using System;
using System.Collections.Generic;
using System.Linq;

namespace TitanBot.Settings
{
    class EditableSettingGroup : IEditableSettingGroup
    {
        public string GroupName { get; private set; }
        public string Notes { get; private set; }
        public string Description { get; private set; }
        public List<EditableSetting> Settings { get; }
        public Type GroupType { get; }

        internal EditableSettingGroup(Type groupType, IEnumerable<EditableSetting> settings)
        {
            GroupType = groupType;
            GroupName = GroupType.Name;
            Settings = settings.ToList();
        }

        public IEditableSettingGroup WithName(string name)
        {
            GroupName = name;
            return this;
        }
        public IEditableSettingGroup WithDescription(string description)
        {
            Description = description;
            return this;
        }
        public IEditableSettingGroup WithNotes(string notes)
        {
            Notes = notes;
            return this;
        }
    }
}

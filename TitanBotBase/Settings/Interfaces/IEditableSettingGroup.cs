﻿using System;
using System.Collections.Generic;

namespace TitanBotBase.Settings
{
    public interface IEditableSettingGroup
    {
        string Description { get; }
        string GroupName { get; }
        Type GroupType { get; }
        List<EditableSetting> Settings { get; }

        IEditableSettingGroup WithName(string name);
        IEditableSettingGroup WithDescription(string description);
    }
}
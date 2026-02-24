using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using TriInspector;

namespace ThunderRoad
{
#if UNITY_EDITOR
    [EditorLoadPriority(-5)]
#endif
    [DeclareBoxGroup("Group", Title = "Group")]
    [DeclareBoxGroup("Audio", Title = "Audio")]
    [DeclareBoxGroup("Effects", Title = "Effects")]
    public class EffectData : CatalogData
    {
        [Group("Group")]
        [Dropdown(nameof(GetAllEffectGroupID))]
        public string groupId;
        [Group("Group")]
        [NonSerialized]
        public EffectGroupData effectGroupData;
        [Group("Group")]
        [ShowInInspector]
        [InlineButton(nameof(RefreshGroupParents))]
        [NonSerialized]
        public List<EffectGroupData> groupParents;

        [Group("Audio")]
        public float volumeDb = 0;

        [Group("Effects")]
        [ShowInInspector]
        [ListDrawerSettings(DontForceInline = true)]
        [ReadOnlyReferencePicker]
        [ReferencePickerScriptButton]
        public List<EffectModule> modules = new();

        public TriDropdownList<string> GetAllEffectGroupID() => Catalog.GetDropdownAllID(Category.EffectGroup);

#if UNITY_EDITOR
        public override string GetCatalogPath()
        {
            string categoryName = Catalog.GetCategory(GetType()).ToString();
            if (categoryName.ToLowerInvariant()[^1] == 's') categoryName += "e";
            string result = $"{categoryName}s/{groupId}";
            if (!groupPath.IsNullOrEmptyOrWhitespace()) result += $"/{groupPath}";
            if (result[result.Length - 1] == '/') result = result.Substring(0, result.Length - 1);
            return result;
        }
#endif

        public void RefreshGroupParents()
        {
        }

    }
}

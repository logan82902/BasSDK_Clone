using System.Collections.Generic;
using System;

using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(-10)]
#endif
    [DeclareBoxGroup("Parent", Title = "Parent")]
    [DeclareBoxGroup("Audio", Title = "Audio")]
    public class EffectGroupData : CatalogData
    {
        [Group("Parent")]
        [Dropdown(nameof(GetAllEffectGroupID))]
        public string groupId;
        [NonSerialized]
        public EffectGroupData parentGroupData;

        [Group("Audio")]
        public float globalVolumeDb = 0;
        [Group("Audio")]
        public float globalMinPitch = 1;
        [Group("Audio")]
        public float globalMaxPitch = 1;

        public TriDropdownList<string> GetAllEffectGroupID() => Catalog.GetDropdownAllID(Category.EffectGroup);

        public string GetPath(string path = "")
        {
return string.Empty;
        }

        public static void GetGroupParents(ref List<EffectGroupData> groups, EffectGroupData effectGroupData)
        {
        }

    }
}
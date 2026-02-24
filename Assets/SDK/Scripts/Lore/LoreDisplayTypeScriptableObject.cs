using System.Collections.Generic;
using UnityEngine;

using TriInspector;

namespace ThunderRoad
{
    [CreateAssetMenu(menuName = "ThunderRoad/Lore/LoreDisplayType")]
    public class LoreDisplayTypeScriptableObject : ScriptableObject
    {
        [Dropdown(nameof(GetAllTextGroupID))]
        public string groupID;
        [Dropdown(nameof(GetAllTextId))]
        public string displayName;
        public Color32 colour;
        [Tooltip("Display this pack as dalgarian in the journal?")] public bool displayInJournalAsDalgarian;
        public int factionOrderIndex;

        public string GetNameKey()
        {
            return groupID + "/" + displayName;
        }

        /// <summary>
        /// Can be deleted after we have some specified, just used to generate dummy data.
        /// See: LoreModule @ 233 (GetAllDisplayType)
        /// If it's not using dummy data this can be deleted.
        /// </summary>
        public static LoreDisplayTypeScriptableObject Create(string displayName, Color32 colour)
        {
            LoreDisplayTypeScriptableObject obj = CreateInstance<LoreDisplayTypeScriptableObject>();
            obj.groupID = "Default";
            obj.displayName = displayName;
            obj.colour = colour;
            return obj;
        }

        public TriDropdownList<string> GetAllTextGroupID() => Catalog.GetTextData().GetDropdownAllTextGroups();

        public TriDropdownList<string> GetAllTextId() => Catalog.GetTextData().GetDropdownAllTexts(groupID);
    }
}

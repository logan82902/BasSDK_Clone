using System;
using System.Collections.Generic;
using UnityEngine;


using TriInspector;

namespace ThunderRoad
{
    [CreateAssetMenu(menuName = "ThunderRoad/Lore/Lore Part pack")]
    public class LorePackScriptableObject : ScriptableObject
    {
        [Serializable]
        public class LoreItemData
        {
            [Dropdown(nameof(GetAllTextGroupID))]
            public string groupId;
            [Dropdown(nameof(GetAllTextId))]
            public string titleId;
            [Dropdown(nameof(GetAllTextId))]
            public string loreId;

            public LoreScriptableObject.LoreType loreType;

            public TriDropdownList<string> GetAllTextGroupID() => Catalog.GetTextData().GetDropdownAllTextGroups();

            public TriDropdownList<string> GetAllTextId() => Catalog.GetTextData().GetDropdownAllTexts(groupId);



#if UNITY_EDITOR
            [Dropdown(nameof(ListContentTypes))]
#endif
            public string contentType = "Bas.UI.DefaultNoteContent";

            [Tooltip("Display any sprites from the pack in the journal?")]
            public bool displayGraphicsInJournal;

#if UNITY_EDITOR
            public TriDropdownList<string> ListContentTypes() =>
                new()
                {
                    new TriDropdownItem<string>() { Text = "Factionless", Value = "Bas.UI.NoteContentFactionless" },
                    new TriDropdownItem<string>() { Text = "Outlaw", Value = "Bas.UI.NoteContentOutlaw" },
                    new TriDropdownItem<string>() { Text = "Wildfolk", Value = "Bas.UI.NoteContentWildfolk" },
                    new TriDropdownItem<string>() { Text = "Eraden", Value = "Bas.UI.NoteContentEradenCommon" },
                    new TriDropdownItem<string>() { Text = "Eraden Royal", Value = "Bas.UI.NoteContentEradenRoyal" },
                    new TriDropdownItem<string>() { Text = "The Eye", Value = "Bas.UI.NoteContentTheEyeCommon" },
                    new TriDropdownItem<string>() { Text = "The Eye Propaganda", Value = "Bas.UI.NoteContentTheEyePropaganda" },
                    new TriDropdownItem<string>() { Text = "Dalgarian Preservation Society", Value = "Bas.UI.NoteContentDalgarianPreservationSociety" },
                    new TriDropdownItem<string>() { Text = "Dalgarian Tablet", Value = "Bas.UI.NoteContentDalgarianTablet" }
                };
#endif
        }

        [Header("Localization Ids")]
        [Dropdown(nameof(GetAllTextGroupID))]
        public string groupId;
        [Dropdown(nameof(GetAllTextId))]
        public string nameId;

        [Tooltip("index use to define the position of thhe pack in the journal")]
        public int journalIndex;

        [Header("Requirement")]
        [Tooltip("lore that need to be read before this one")]
        public List<LorePackScriptableObject> loreRequirements;

        [Header("Conditions")]
        public LorePackCondition condition;

        [Dropdown(nameof(GetAllItemId))]
        public string itemId = "Note";
        public bool spawnPackAsSingleItem;

        [Header("Lore Item Data")]
        public List<LoreItemData> loreData;



        public TriDropdownList<string> GetAllItemId() =>
            new()
            {
                new TriDropdownItem<string>() { Text = "Note", Value = "Note" },
                new TriDropdownItem<string>() { Text = "DalgarianTablet", Value = "DalgTablet" },
                new TriDropdownItem<string>() { Text = "Notepad", Value = "Notepad" },
                new TriDropdownItem<string>() { Text = "NotepadOutlaw", Value = "NotepadOutlaw" },
                new TriDropdownItem<string>() { Text = "NotepadWildfolk", Value = "NotepadWildfolk" },
                new TriDropdownItem<string>() { Text = "NotepadEraden", Value = "NotepadEradenCommon" },
                new TriDropdownItem<string>() { Text = "NotepadEradenRoyal", Value = "NotepadEradenRoyal" },
                new TriDropdownItem<string>() { Text = "NotepadTheEye", Value = "NotepadTheEyeCommon" },
                new TriDropdownItem<string>() { Text = "NotepadTheEyePropaganda", Value = "NotepadTheEyePropaganda" },
                new TriDropdownItem<string>() { Text = "NotepadDalgarianSociety", Value = "NotepadDalgarianSociety" }
            };
        public TriDropdownList<string> GetAllTextGroupID() => Catalog.GetTextData().GetDropdownAllTextGroups();

        public TriDropdownList<string> GetAllTextId() => Catalog.GetTextData().GetDropdownAllTexts(groupId);



        #region Tool
#if UNITY_EDITOR
        private int noteIndex = 0;
        private bool currentlySpawning = false;
        public TriDropdownList<string> GetAllLanguages() => Catalog.GetDropdownAllID<TextData>();
        [Dropdown(nameof(GetAllLanguages))]
        [Header("Editor Tool")]
        public string language = "English";
        [Button]
        public void SpawnLore()
        {
        }


        private void OnItemSpawn(Item obj)
        {
            ILoreDisplay loreDisplay = obj.GetComponentInChildren<ILoreDisplay>();
            if (loreDisplay == null)
            {
                Debug.LogError("Can not find ILoreDisplay in the item : " + obj.itemId);
                return;
            }

            loreDisplay.EditorSetLore(loreData[noteIndex]);
            noteIndex++;
        }

        private void OnItemSpawnAllData(Item obj)
        {
            ILoreDisplay loreDisplay = obj.GetComponentInChildren<ILoreDisplay>();
            if (loreDisplay == null)
            {
                Debug.LogError("Can not find ILoreDisplay in the item : " + obj.itemId);
                return;
            }

            loreDisplay.EditorSetMultipleLore(loreData);
        }
#endif //UNITY_EDITOR
        #endregion Tool
    }
}

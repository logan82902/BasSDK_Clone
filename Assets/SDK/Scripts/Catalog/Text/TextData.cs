using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(0)]
#endif
    [DeclareTabGroup("Group")]
    public class TextData : CatalogData
    {
        public const string DEFAULT_TEXT_GROUP = "Default";

        // Used for mod.io (we could get the language code dynamically using a call to the CultureInfo with the ISO 639 but the chinese languages code is different on the mod.io API,
        // so we set the code here, instead of adding specific code to check which language is and manually change the code for some of them).
        public string languageCode;

        [Group("Group"), Tab("Texts")]
        [HideReferencePicker]
        [ListDrawerSettings(ItemsPerPage = 25)]
        public List<TextGroup> textGroups;
        [Group("Group"), Tab("Items")]
        [HideReferencePicker]
        [ListDrawerSettings(ItemsPerPage = 25)]
        public List<Item> items;
        [Group("Group"), Tab("Waves")]
        [HideReferencePicker]
        [ListDrawerSettings(ItemsPerPage = 25)]
        public List<Wave> waves;

        [Serializable]
        public class TextGroup
        {
            [JsonMergeKey]
            public string id;
            [ListDrawerSettings(ItemsPerPage = 25)]
            public List<TextID> texts;

        }

        [Serializable]
        public class TextID
        {
            [JsonMergeKey]
            public string id;
            [HideLabel]
            [TextArea(0, 20)]
            public string text;
            public string spriteAddress;
            public string videoAddress;
            [NonSerialized]
            public IResourceLocation spriteLocation;
            [NonSerialized]
            public IResourceLocation videoLocation;
        }

        [Serializable]
        [DeclareHorizontalGroup("Split")]
        [DeclareVerticalGroup("Split/vert")]
        public class Item
        {
            [LabelWidth(50f)]
            [Group("Split/vert")]
            [JsonMergeKey]
            public string id;
            [Group("Split/vert")]
            public string name;
            [Group("Split")]
            [HideLabel]
            [TextArea(0, 20)]
            public string description;
        }

        [Serializable]
        [DeclareHorizontalGroup("Split")]
        [DeclareVerticalGroup("Split/vert")]
        public class Wave
        {
            [LabelWidth(50f)]
            [Group("Split/vert")]
            [JsonMergeKey]
            public string id;
            [Group("Split/vert")]
            public string title;
            [Group("Split")]
            [HideLabel]
            [TextArea(0, 20)]
            public string description;
        }

        public override int GetCurrentVersion()
        {
            return 1;
        }

        public TriDropdownList<string> GetDropdownAllTextGroups()
        {
            TriDropdownList<string> dropdownList = new();
            foreach (TextGroup textGroup in textGroups)
            {
                dropdownList.Add(textGroup.id, textGroup.id);
            }
            return dropdownList;
        }

        public TriDropdownList<string> GetDropdownAllTexts(string groupId)
        {
            TriDropdownList<string> dropdownList = new();
            foreach (TextGroup textGroup in textGroups)
            {
                if (string.Equals(textGroup.id, groupId, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (TextID textId in textGroup.texts)
                    {
                        dropdownList.Add(textId.id, textId.id);
                    }
                    break;
                }
            }
            return dropdownList;
        }

#if UNITY_EDITOR
        [Button]
        public void CheckDuplicates() => FindAndOrRemoveDuplicates(false);

        [Button]
        public void CleanUpDuplicates() => FindAndOrRemoveDuplicates(true);

        private void FindAndOrRemoveDuplicates(bool remove)
        {
            Dictionary<string, TextGroup> groupIDs = new Dictionary<string, TextGroup>();
            HashSet<string> textIDsInGroup = new HashSet<string>();
            HashSet<string> itemIDs = new HashSet<string>();
            HashSet<string> waveIDs = new HashSet<string>();
            List<TextGroup> duplicateIDGroups = new List<TextGroup>();
            List<(TextGroup group, TextID text)> duplicateTexts = new List<(TextGroup group, TextID text)>();
            List<Item> duplicateItems = new List<Item>();
            List<Wave> duplicateWaves = new List<Wave>();
            foreach (TextGroup textGroup in textGroups)
            {
                if (!groupIDs.ContainsKey(textGroup.id))
                {
                    groupIDs[textGroup.id] = textGroup;
                }
                else
                {
                    duplicateIDGroups.Add(textGroup);
                }
            }
            foreach (TextGroup duplicateGroup in duplicateIDGroups)
            {
                Debug.Log($"Found duplicate Group ID in {id}: {duplicateGroup.id}");
                if (remove)
                {
                    groupIDs[duplicateGroup.id].texts.AddRange(duplicateGroup.texts);
                    textGroups.Remove(duplicateGroup);
                    Debug.Log($"Removed duplicate group {duplicateGroup.id} from {id}");
                }
            }
            // loop over the text groups again after potentially cleaning up duplicates
            foreach (TextGroup textGroup in textGroups)
            {
                textIDsInGroup = new HashSet<string>();
                foreach (TextID textID in textGroup.texts)
                {
                    if (!textIDsInGroup.Contains(textID.id))
                    {
                        textIDsInGroup.Add(textID.id);
                    }
                    else
                    {
                        duplicateTexts.Add((textGroup, textID));
                    }
                }
            }
            foreach (var duplicateText in duplicateTexts)
            {
                Debug.Log($"Found duplicate Text ID in {id}: textgroup.{duplicateText.group.id}.{duplicateText.text.id}");
                if (remove)
                {
                    duplicateText.group.texts.Remove(duplicateText.text);
                    Debug.Log($"Removed duplicate text {duplicateText.text.id} from {id}/{duplicateText.group.id}");
                }
            }
            foreach (Item item in items)
            {
                if (!itemIDs.Contains(item.id))
                {
                    itemIDs.Add(item.id);
                }
                else
                {
                    duplicateItems.Add(item);
                }
            }
            foreach (Item item in duplicateItems)
            {
                Debug.Log($"Found duplicate Item ID in {id}: {item.id}");
                if (remove)
                {
                    items.Remove(item);
                    Debug.Log($"Removed duplicate item {item.id} from {id}");
                }
            }
            foreach (Wave wave in waves)
            {
                if (!waveIDs.Contains(wave.id))
                {
                    waveIDs.Add(wave.id);
                }
                else
                {
                    duplicateWaves.Add(wave);
                }
            }
            foreach (Wave wave in duplicateWaves)
            {
                Debug.Log($"Found duplicate Wave ID in {id}: {wave.id}");
                if (remove)
                {
                    waves.Remove(wave);
                    Debug.Log($"Removed duplicate wave {wave.id} from {id}");
                }
            }
        }
#endif
    }
}

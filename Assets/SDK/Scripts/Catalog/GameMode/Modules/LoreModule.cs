using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.Modules
{
    public class LoreModule : GameModeModule
    {
        public class LoreState
        {
            public LoreScriptableObject.LorePack lorePack;
            public List<int> loreIdToCheckOn;
            public List<LoreScriptableObject.LoreData> lorePackDatas
            {
                get
                {
                    List<LoreScriptableObject.LoreData> lorePackDatas = new List<LoreScriptableObject.LoreData>();
                    if (lorePack == null)
                        return lorePackDatas;
                    if (lorePack.loreData == null)
                        return lorePackDatas;

                    int count = lorePack.loreData.Count;
                    for (int i = 0; i < count; i++)
                    {
                        LoreScriptableObject.LoreData tempLore = lorePack.loreData[i];
                        LoreScriptableObject.LoreData newLore = new LoreScriptableObject.LoreData()
                        {
                            groupId = tempLore.groupId,
                            loreId = tempLore.loreId,
                            itemId = tempLore.itemId,
                            loreType = tempLore.loreType,
                            contentAddress = tempLore.contentAddress,
                            displayGraphicsInJournal = tempLore.displayGraphicsInJournal,
                        };

                        lorePackDatas.Add(newLore);
                    }

                    return lorePackDatas;
                }
            }

        }

        public string[] loreAddressList;
        [Tooltip("Probability weight of a lore spawner being enabled")]
        public float loreSpawnProbabiltyWeight = 0.5f;
        [SerializeField, Tooltip("Probability weight based on the visibility of the spawner. Lower visibility should have a higher probability.")]
        public Dictionary<LorePackCondition.Visibility, float> VisibilityProbabilityWeights = new Dictionary<LorePackCondition.Visibility, float>()
        {
            {LorePackCondition.Visibility.Hidden, 1.0f },
            {LorePackCondition.Visibility.PartiallyHidden, 0.75f},
            {LorePackCondition.Visibility.Visibile, 0.5f },
            {LorePackCondition.Visibility.VeryVisibile, 0.25f },
        };

        [Tooltip("The distance the player must be within to be able to count the lore as being read")]
        public float LoreLookDistanceThreshold = 1.5f;
        [Tooltip("The duration of how long the player has to be looking at the lore to count as being read")]
        public float LoreReadDuration = 1.0f;
        [Range(0, 1), Tooltip("A measure of how direct the player must be looking at the lore to be reading it, 0 means barely looking at it and 1.0 means player must look directly at it")]
        public float LoreLookThreshold = 0.95f;
        public Color LoreOutlineColor;
        // Text
        [Dropdown(nameof(GetAllTextGroupID))]
        public string loreAquiredTextGroupId;

        [Dropdown(nameof(GetAllTextId))]
        public string textLoreAquired;


        public string loreAquiredText;

        [JsonIgnore]
        public List<LoreScriptableObject> allLoreSO;
        [JsonIgnore]
        public Dictionary<int, LoreState> loreNotRead;
        [JsonIgnore]
        public List<int> availableLore;


        public string LoreFoundSfxID = "LoreFound";
 //ProjectCore

        public TriDropdownList<string> GetAllTextGroupID() => Catalog.GetTextData().GetDropdownAllTextGroups();

        public TriDropdownList<string> GetAllTextId() => Catalog.GetTextData().GetDropdownAllTexts(loreAquiredTextGroupId);
    }
}
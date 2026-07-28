using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(-5)]
#endif
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Generation test", Title = "Generation test")]
    [DeclareBoxGroup("Population test", Title = "Population test")]
    public class CreatureTable : CatalogData
    {
        [Group("General")]
        [Multiline]
        public string description;

        [Group("General")]
        public bool linkedToGenderRatio;

        [HideReferencePicker]
        [TableList(AlwaysExpanded = true)]
        [OnValueChanged(nameof(EditorCalculateWeight))]
        public List<Drop> drops;

        private float probabilityTotalWeight;

        public override CatalogData Clone()
        {
            CreatureTable clone = MemberwiseClone() as CreatureTable;
            clone.drops = drops.Select(item => (Drop)item.Clone()).ToList();
            return clone;
        }

        public List<int> GetFactionIDs()
        {
            List<int> ids = new List<int>();
            foreach (Drop drop in drops)
            {
                ids.AddRange(drop.GetFactionIDs());
            }
            return ids;
        }

        public List<string> GetAllCreatureIDs()
        {
            List<string> results = new List<string>();
            foreach (Drop drop in drops)
            {
                if (drop.referenceID.IsNullOrEmptyOrWhitespace()) continue;
                if (drop.reference == Drop.Reference.Table)
                {
                    results.AddRange(Catalog.GetData<CreatureTable>(drop.referenceID).GetAllCreatureIDs());
                }
                else
                {
                    results.Add(drop.referenceID);
                }
            }
            return results;
        }

        [Serializable]
        [DeclareHorizontalGroup("Reference")]
        [DeclareHorizontalGroup("Override Faction", Sizes = new float[] { 15f, 0f})]
        [DeclareHorizontalGroup("Override Container", Sizes = new float[] { 15f, 0f })]
        [DeclareHorizontalGroup("Override Brain", Sizes = new float[] { 15f, 0f })]
        [DeclareHorizontalGroup("Override Ethnicity", Sizes = new float[] { 15f, 0f })]
        [DeclareHorizontalGroup("Probability Weights")]
        public class Drop
        {
            [JsonMergeKey(MergeKeyValueProvider = nameof(GetMergeKey))]
            [HideInInspector]
            public string mergeKey = "";

            [Group("Reference")]
            [HideLabel]
            [EnumToggleButtons]
            public Reference reference = Reference.Creature;

            [Group("Reference")]
            [LabelText("ID")]
            [LabelWidth(20f)]
            [Dropdown(nameof(GetAllCreatureOrTableID))]
            public string referenceID;

            [Group("Override Faction")]
            [HideLabel]
            public bool overrideFaction;

            [Group("Override Faction")]
            [ShowIf("overrideFaction", true)]
            [Dropdown(nameof(GetAllFactionID), Searchable = false)]
            [HideLabel]
            public int factionID = 0;

            [Group("Override Container")]
            [HideLabel]
            public bool overrideContainer;

            [Group("Override Container")]
            [ShowIf("overrideContainer", true)]
            [Dropdown(nameof(GetAllContainerID))]
            [HideLabel]
            public string overrideContainerID;

            [Group("Override Brain")]
            [HideLabel]
            public bool overrideBrain;

            [Group("Override Brain")]
            [ShowIf("overrideBrain", true)]
            [Dropdown(nameof(GetAllBrainID))]
            [HideLabel]
            public string overrideBrainID;

            [Group("Override Ethnicity")]
            [OnValueChanged(nameof(SetEthnicityDefault))]
            [HideLabel]
            public bool overrideEthnicity;

            [Group("Override Ethnicity")]
            [ShowIf("overrideEthnicity", true)]
            [Dropdown(nameof(GetAllEthnicityID), Searchable = false)]
            [HideLabel]
            public string overrideEthnicityID;


            [Group("Probability Weights")]
            [HideLabel]
            [ShowInInspector]
            public float probabilityWeight;

            [Group("Probability Weights")]
            [ShowInInspector]
            [ReadOnly]
            [NonSerialized]
            [LabelText("%")]
            [LabelWidth(15f)]
            public float probabilityPercent;
            [NonSerialized]
            public float probabilityRangeFrom;
            [NonSerialized]
            public float probabilityRangeTo;

            public Drop()
            {
                mergeKey = GetMergeKey();
            }

            public static string GetMergeKey()
            {
                return Guid.NewGuid().ToString().Substring(0, 8);
            }

            public void SetEthnicityDefault()
            {
                if (overrideEthnicityID.IsNullOrEmptyOrWhitespace())
                {
                    overrideEthnicityID = GetAllEthnicityID()[0].Value;
                }
            }

            public TriDropdownList<string> GetAllCreatureOrTableID()
            {
                if (reference == Reference.Creature)
                {
                    return Catalog.GetDropdownAllID(Category.Creature);
                }
                else
                {
                    return Catalog.GetDropdownAllID(Category.CreatureTable);
                }
            }

            public TriDropdownList<string> GetAllContainerID() => Catalog.GetDropdownAllID(Category.Container);

            public TriDropdownList<string> GetAllBrainID() => Catalog.GetDropdownAllID(Category.Brain);

            public TriDropdownList<string> GetAllEthnicityID()
            {
                if (reference == Reference.Table)
                {
                    TriDropdownList<string> result = new TriDropdownList<string>();
                    foreach (string creatureId in Catalog.GetData<CreatureTable>(referenceID).GetAllCreatureIDs())
                    {
                        foreach (var ethnicityOption in Catalog.GetData<CreatureData>(creatureId).GetAllEthnicityID())
                        {
                            if (result.Contains(ethnicityOption)) continue;
                            result.Add(ethnicityOption);
                        }
                    }
                    return result;
                }
                else
                {
                    return Catalog.GetData<CreatureData>(referenceID).GetAllEthnicityID();
                }
            }
            public TriDropdownList<int> GetAllFactionID() => Catalog.gameData.GetFactions();

            public enum Reference
            {
                Creature,
                Table,
            }

            public int[] GetFactionIDs()
            {

                if (overrideFaction)
                {
                    return new int[] { factionID };
                }
                else
                {
                    if (referenceID == "None" || referenceID == null) return new int[0];
                    if (reference == Reference.Table)
                    {
                        return Catalog.GetData<CreatureTable>(referenceID)?.GetFactionIDs()?.ToArray() ?? new int[0];
                    }
                    else
                    {
                        return new int[] { Catalog.GetData<CreatureData>(referenceID)?.factionId ?? 0 };
                    }
                }
            }

            public void OnCatalogRefresh()
            {
                if (mergeKey.IsNullOrEmptyOrWhitespace())
                {
                    mergeKey = GetMergeKey();
                }
                if (!string.IsNullOrEmpty(referenceID)) return;
            }

            public Drop Clone()
            {
                return MemberwiseClone() as Drop;
            }
        }



        public override int GetCurrentVersion()
        {
            return 1;
        }

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            
            drops ??= new List<Drop>();
            foreach (Drop drop in drops)
            {
                drop.OnCatalogRefresh();
            }
            CalculateWeight();
        }

        public void EditorCalculateWeight()
        {
            CalculateWeight();
        }

        /// <summary>
        /// Calculates the percentage and assigns the probabilities how many times
        /// the items can be picked. Function used also to validate data when tweaking numbers in editor.
        /// </summary>	
        public void CalculateWeight()
        {
            if (drops.IsNullOrEmpty()) return;

            float currentProbabilityWeightMaximum = 0f;
            // Sets the weight ranges of the selected items.
            int dropsCount = drops.Count;
            for (var i = 0; i < dropsCount; i++)
            {
                Drop lootDropItem = drops[i];
                if (lootDropItem.probabilityWeight < 0)
                {
                    // Prevent usage of negative weight.
                    lootDropItem.probabilityWeight = 0;
                    continue;
                }
                lootDropItem.probabilityRangeFrom = currentProbabilityWeightMaximum;
                currentProbabilityWeightMaximum += lootDropItem.probabilityWeight;
                lootDropItem.probabilityRangeTo = currentProbabilityWeightMaximum;
                
            }
            probabilityTotalWeight = currentProbabilityWeightMaximum;
            // Calculate percentage of item drop select rate.
            for (var i = 0; i < dropsCount; i++)
            {
                Drop lootDropItem = drops[i];
                lootDropItem.probabilityPercent = lootDropItem.probabilityWeight / probabilityTotalWeight * 100;
            }
        }


        protected bool TryPick(int pickCount, out CreatureData creatureData, System.Random randomGen)
        {
creatureData = null;
            return false;
        }
    }
}

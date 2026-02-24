using System;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    public class ItemModuleCrystal : ItemModule
    {
        public const float mergeEffectDelay = 2f;
        public const float glowMultiplier = 3f;
        
        [Header("skill tree")]
        [Dropdown(nameof(GetAllSkillTreeID))]
        public string treeName;
        
        [Header("Custom")]
        public bool overrideCrystalColors = false;

        [ColorPickerField(Alpha = true, HDR = true)]
        [ShowIf("overrideCrystalColors")]
        public Color baseColor;
        [ColorPickerField(Alpha = true, HDR = true)]
        [ShowIf("overrideCrystalColors")]
        public Color internalColor;
        [ColorPickerField(Alpha = true, HDR = true)]
        [ShowIf("overrideCrystalColors")]
        public Color animatedColor;
        [ColorPickerField(Alpha = true, HDR = true)]
        [ShowIf("overrideCrystalColors")]
        public Color emissionColor;
        [ColorPickerField(Alpha = true, HDR = true)]
        [ShowIf("overrideCrystalColors")]
        public Color linkVfxColor;
        [ColorPickerField(Alpha = true, HDR = true)]
        [ShowIf("overrideCrystalColors")]
        public Color mergeVfxColor;

        [Header("Merge")]
        [Dropdown(nameof(GetAllCrystalId))]
        public string higherTierCrystalId;
        [Dropdown(nameof(GetAllShardsId))]
        public string shardId;

        [Dropdown(nameof(GetAllEffectId))]
        public string mergeBeginEffectId = "CrystalMergeBegin";
        [NonSerialized]
        public EffectData mergeBeginEffectData;

        [Dropdown(nameof(GetAllEffectId))]
        public string sparkleEffectId = "CrystalSparkle";
        [NonSerialized]
        public EffectData sparkleEffectData;

        [Dropdown(nameof(GetAllEffectId))]
        public string hoverEffectId = "CrystalCoreHover";
        [NonSerialized]
        public EffectData hoverEffectData;

        [Dropdown(nameof(GetAllEffectId))]
        public string mergeEffectId = "CrystalMerge";
        [NonSerialized]
        public EffectData mergeEffectData;

        [Dropdown(nameof(GetAllEffectId))]
        public string mergeCrystalEffectAndroidId = "CrystalMergeGlow";
        [NonSerialized]
        public EffectData mergeCrystalEffectAndroidData;

        [Dropdown(nameof(GetAllEffectId))]
        public string mergeStartEffectId = "CrystalMergeStart";
        [NonSerialized]
        public EffectData mergeStartEffectData;

        [Dropdown(nameof(GetAllEffectId))]
        public string mergeCompleteEffectId = "CrystalMergeNewTier";
        [NonSerialized]
        public EffectData mergeCompleteEffectData;

        public float endMergeTime = 1;

        [Dropdown(nameof(GetAllEffectId))]
        public string activateEffectId = "CrystalActivate";
        [NonSerialized]
        public EffectData activateEffectData;

        [NonSerialized]
        public SkillTreeData skillTreeData;

        public bool allowMerge = true;




        public TriDropdownList<string> GetAllSkillTreeID() => Catalog.GetDropdownAllID(Category.SkillTree);
        public TriDropdownList<string> GetAllCrystalId()
        {
            TriDropdownList<string> dropdownList = new();
            dropdownList.Add("None", "");
            List<CatalogData> allItems = Catalog.GetDataList(Category.Item);
            for (int i = 0; i < allItems.Count; i++)
            {
                if(allItems[i] is ItemData item
                    && item.TryGetModule<ItemModuleCrystal>(out ItemModuleCrystal module)
                    && string.Equals(module.treeName, treeName))
                {
                    dropdownList.Add(item.id, item.id);
                }
            }

            return dropdownList;
        }

        public TriDropdownList<string> GetAllShardsId()
        {
            TriDropdownList<string> dropdownList = new();
            dropdownList.Add("None", "");
            foreach (string id in Catalog.GetAllID(Category.Item))
            {
                if (id.Contains("Shard"))
                {
                    dropdownList.Add(id, id);
                }
            }
            return dropdownList;
        }

        public TriDropdownList<string> GetAllEffectId() => Catalog.GetDropdownAllID(Category.Effect);
    }
}

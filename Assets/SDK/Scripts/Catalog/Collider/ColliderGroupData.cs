using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(-5)]
#endif
    public class ColliderGroupData : CatalogData
    {
        public CollisionHandling collisionHandling = CollisionHandling.ByGroup;
        public bool allowPenetration = true;

        public List<Modifier> modifiers = new List<Modifier>();

        [FormerlySerializedAs("allowedImbueEffectModules")]
        [ShowInInspector]
        [DropdownList(nameof(GetAllEffectModuleTypes))]
        public Type[] ignoredImbueEffectModules;

        public TriDropdownList<Type> GetAllEffectModuleTypes()
        {
            TriDropdownList<Type> dropdownList = new();
            foreach (Type type in typeof(EffectModule).Assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(EffectModule)))
                {
                    dropdownList.Add(type.Name, type);
                }
            }

            return dropdownList;
        }

        public bool customSpellEffects = false;
        [ShowInInspector]
        [ShowIf("customSpellEffects")]
        public List<CustomSpellEffect> customSpellEffectIDs;
        [ShowIf("customSpellEffects")]
        [JsonIgnore]
        public Dictionary<int, EffectData> customSpellEffectData;
        [ShowIf("customSpellEffects")]
        public bool blockPoolSteal = false;

        public enum ImbueType
        {
            None,
            Metal,
            Blade,
            Crystal,
            Custom,
        }

        public enum CollisionHandling
        {
            ByGroup,
            ByCollider,
        }

        [Serializable]
        [DeclareHorizontalGroup("Split")]
        [DeclareBoxGroup("Split/Filters", Title = "Filters")]
        [DeclareBoxGroup("Split/Imbue", Title = "Imbue")]
        [DeclareBoxGroup("Split/Deflect", Title = "Deflect")]
        public class Modifier
        {
            [NonSerialized]
            public ColliderGroupData colliderGroupData;

            [Group("Split/Filters")]
            [LabelWidth(150f)]
            public TierFilter tierFilter;

            [Flags]
            public enum TierFilter
            {
                Tier0 = 1,
                Tier1 = 2,
                Tier2 = 4,
                Tier3 = 8,
                Tier4 = 16,
                Tier5 = 32,
                Tier6 = 64,
            }

            protected string InfoBox => $"{Mathf.FloorToInt(imbueMax / staffSlamLoss)} slams total\n"
                                     + $"{1 / (50 / staffSlamLoss * imbueRate):F2}s to first slam\n"
                                     + $"{1 / (50 / 100f * imbueRate):F2}s to full charge";

            [InfoBox("$InfoBox")]
            [Group("Split/Imbue")]
            public ImbueType imbueType = ImbueType.None;
            [Group("Split/Imbue")]
            [HideIf("imbueType", ImbueType.None)]
            public float imbueMax = 100;
            [Group("Split/Imbue")]
            [HideIf("imbueType", ImbueType.None)]
            public float imbueRate = 1;
            [Group("Split/Imbue")]
            [HideIf("imbueType", ImbueType.None)]
            public float imbueConstantLoss = 3;
            [Group("Split/Imbue")]
            [HideIf("imbueType", ImbueType.None)]
            public float staffSlamLoss = 90;
            [Group("Split/Imbue")]
            [HideIf("imbueType", ImbueType.None)]
            public float imbueHitLoss = 30;
            [Group("Split/Imbue")]
            [HideIf("imbueType", ImbueType.None)]
            public float imbueVelocityLossPerSecond = 0f;
            [Group("Split/Imbue")]
            [HideIf("imbueType", ImbueType.None)]
            public float imbueEffectiveness = 1f;
            [Group("Split/Imbue")]
            [HideIf("imbueType", ImbueType.None)]
            public float waterLossRateMultiplier = 1f;
            [Group("Split/Imbue")]
            [HideIf("imbueType", ImbueType.None)]
            public ImbueFeature imbueFeatures = 0;

            [Group("Split/Deflect")]
            public float deflectMaxAngleCollision = 0;
            [Group("Split/Deflect")]
            [HideIf("deflectMaxAngleCollision", 0f)]
            public float deflectMaxAngleTarget = 0;
            [Group("Split/Deflect")]
            [HideIf("deflectMaxAngleCollision", 0f)]
            public float deflectSpeedMultiplier = 2.0f;
            [Group("Split/Deflect")]
            [HideIf("deflectMaxAngleCollision", 0f)]
            public float deflectSpreadHeight = 1; // z rotation
            [Group("Split/Deflect")]
            [HideIf("deflectMaxAngleCollision", 0f)]
            public float deflectSpreadWidth = 1; // y rotation
            
            public FilterLogic spellFilterLogic = FilterLogic.AnyExcept;
            [DropdownList(nameof(GetAllSpellID))]
            public List<string> spellIds;

            public TriDropdownList<string> GetAllSpellID() => Catalog.GetDropdownAllID<SpellData>();
        }

        public struct CustomSpellEffect
        {
            [Dropdown(nameof(GetAllSpellID))]
            public string spellId;

            [Dropdown(nameof(GetAllEffectID))]
            public string effectId;

            public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

            public TriDropdownList<string> GetAllSpellID() => Catalog.GetDropdownAllID<SpellCastCharge>();
        }

        [Flags]
        public enum ImbueFeature
        {
            Slam = 1,
            Steal = 2,
            Use = 4
        }
    }
}


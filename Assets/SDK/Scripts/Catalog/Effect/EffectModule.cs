using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.AddressableAssets.Settings;
#endif

using TriInspector;

namespace ThunderRoad
{
    [AutoLabelWidth(MinWidth = 0f)]
    [DeclareHorizontalGroup("Split")]
    [DeclareBoxGroup("Split/Module filters", Title = "Module filters")]
    [DeclareBoxGroup("Split/Collision filters", Title = "Collision filters")]
    [DeclareHorizontalGroup("Split2")]
    [DeclareBoxGroup("Split2/Imbues filters", Title = "Imbues filters")]
    [DeclareBoxGroup("Split2/Collider group filters", Title = "Collider group filters")]
    [DeclareBoxGroup("Split2/Damagers filters", Title = "Damagers filters")]
    [DeclareHorizontalGroup("Split3")]
    [DeclareBoxGroup("Split3/Input modifiers", Title = "Input modifiers")]
    public abstract class EffectModule
    {
        [JsonMergeKey(MergeKeyValueProvider = nameof(GetMergeKey))]
        [HideInInspector]
        public string mergeKey = "";

        [LabelWidth(150f)]
        [Group("Split/Module filters")]
        public Effect.Step step = Effect.Step.Start;
        [Group("Split/Module filters")]
        [ShowIf("step", Effect.Step.Custom)]
        public string stepCustomId;
        [NonSerialized]
        public int stepCustomIdHash;
        [Group("Split/Module filters")]
        [ShowIf("step", Effect.Step.Custom)]
        public bool loopCustomStep;

        [Group("Split/Module filters")]
        public PlatformFilter platformFilter = PlatformFilter.Windows | PlatformFilter.Android;

        [Newtonsoft.Json.JsonProperty("isGore")]
        private bool contentSetter
        {
            set
            {
                if (value)
                {
                    sensitiveContent = BuildSettings.ContentFlag.Blood;
                }
                else
                {
                    sensitiveContent = BuildSettings.ContentFlag.None;
                }
            }
        }

        [Group("Split/Module filters")]
        public BuildSettings.ContentFlag sensitiveContent = BuildSettings.ContentFlag.None;

        [Group("Split/Module filters")]
        public BuildSettings.ContentFlagBehaviour sensitiveFilterBehaviour = BuildSettings.ContentFlagBehaviour.Discard;

        [Group("Split/Collision filters")]
        public DamagerFilter damagerStateFilter = DamagerFilter.Inactive | DamagerFilter.Active;

        [Group("Split/Collision filters")]
        public DamageTypeFilter damageTypeFilter = DamageTypeFilter.Energy | DamageTypeFilter.Blunt | DamageTypeFilter.Slash | DamageTypeFilter.Pierce;

        [Group("Split/Collision filters")]
        public PenetrationFilter penetrationFilter = PenetrationFilter.None | PenetrationFilter.Hit | PenetrationFilter.Pressure;

        [Group("Split/Collision filters")]
        public bool reparentWithBreakable = true;

        [LabelWidth(150f)]
        [Group("Split2/Imbues filters")]
        public FilterLogic imbuesFilterLogic = FilterLogic.AnyExcept;

        [Group("Split2/Imbues filters")]
        [DropdownList("GetAllSpellCastChargeID")]
        public string[] imbuesFilter = new string[0];
        protected int[] imbueHashes = new int[0];

        [Group("Split2/Collider group filters")]
        public FilterLogic colliderGroupFilterLogic = FilterLogic.AnyExcept;

        [Group("Split2/Collider group filters")]
        [DropdownList("GetAllColliderGroupID")]
        public string[] colliderGroupsFilter = new string[0];
        protected int[] colliderGroupHashes = new int[0];

        [Group("Split2/Damagers filters")]
        public FilterLogic damagersFilterLogic = FilterLogic.AnyExcept;

        [Group("Split2/Damagers filters")]
        [DropdownList("GetAllDamagerID")]
        public string[] damagersFilter = new string[0];
        protected int[] damagerHashes = new int[0];
        
        [NonSerialized]
        public EffectData rootData;

        [Flags]
        public enum PlatformFilter
        {
            Windows = 1,
            Android = 2
        }

        [Flags]
        public enum PenetrationFilter
        {
            None = 1,
            Pressure = 2,
            Hit = 4,
            Skewer = 8
        }

        [Flags]
        public enum DamagerFilter
        {
            Inactive = 1,
            Active = 2
        }

        [Flags]
        public enum DamageTypeFilter
        {
            Energy = 1,
            Blunt = 2,
            Slash = 4,
            Pierce = 8
        }

        public EffectModule()
        {
            mergeKey = GetMergeKey();
        }

        public static string GetMergeKey()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        public virtual void OnCatalogRefresh(EffectData effectData, bool editorLoad = false)
        {
            rootData = effectData;
            if (mergeKey.IsNullOrEmptyOrWhitespace())
            {
                mergeKey = GetMergeKey();
            }
        }


        public TriDropdownList<string> GetAllSpellCastChargeID() => Catalog.GetDropdownAllID<SpellCastCharge>();
        public TriDropdownList<string> GetAllColliderGroupID() => Catalog.GetDropdownAllID<ColliderGroupData>();

        public TriDropdownList<string> GetAllDamagerID() => Catalog.GetDropdownAllID<DamagerData>();

        public virtual void Clean()
        { }

        public virtual void CopyHDRToNonHDR()
        { }

    }
}

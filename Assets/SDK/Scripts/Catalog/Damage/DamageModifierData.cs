using System;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(0)]
#endif
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Collision(s)", Title = "Collision(s)")]
    public class DamageModifierData : CatalogData
    {
        [Group("General")]
        public DamageType damageType = DamageType.Blunt;

        [Group("Collision(s)")]
        [HideReferencePicker]
        public List<Collision> collisions = new();

        protected Modifier defaultModifier;

        public override int GetCurrentVersion()
        {
            return 1;
        }


        [Serializable]
        [DeclareHorizontalGroup("Split")]
        public class Collision
        {
            [NonSerialized]
            public DamageModifierData damagerModifierData;

            [Group("Split")]
            [DropdownList("GetAllMaterialID")]
            public List<string> sourceMaterialIds = new();
            [NonSerialized]
            public List<MaterialData> sourceMaterials;

            [Group("Split")]
            [DropdownList("GetAllMaterialID")]
            public List<string> targetMaterialIds = new();
            [NonSerialized]
            public List<MaterialData> targetMaterials;

            [HideReferencePicker]
            public List<Modifier> modifiers = new();

            public TriDropdownList<string> GetAllMaterialID() => Catalog.GetDropdownAllID(Category.Material);

            public void OnCatalogRefresh()
            {
            }

        }

        [Serializable]
        [DeclareHorizontalGroup("Split")]
        [DeclareBoxGroup("Split/Filters", Title = "Filters")]
        [DeclareBoxGroup("Split/General", Title = "General")]
        [DeclareBoxGroup("Split/Knockout", Title = "Knockout")]
        [DeclareBoxGroup("Split/Push", Title = "Push")]
        [DeclareBoxGroup("Split/Penetration", Title = "Penetration")]
        public class Modifier
        {
            [NonSerialized]
            public Collision collision;
            [NonSerialized]
            public DamageModifierData damagerModifierData;

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


            [Group("Split/Filters")]
            public FilterLogic imbuesFilterLogic = FilterLogic.AnyExcept;
            [Group("Split/Filters")]
            [DropdownList("GetAllSpellCastChargeID")]
            public string[] imbuesFilter = new string[0];

            public TriDropdownList<string> GetAllSpellCastChargeID() => Catalog.GetDropdownAllID<SpellCastCharge>();

            [Group("Split/General")]
            [LabelWidth(150)]
            public float minVelocity = 1;
            [Group("Split/General")]
            [LabelWidth(150)]
            public float damageMultiplier = 1;
            [Group("Split/General")]
            [LabelWidth(150)]
            public bool allowKnockout = false;
            [Group("Split/General")]
            [LabelWidth(150)]
            public bool allowPenetration = false;
            [Group("Split/General")]
            [LabelWidth(150)]
            [EnableIf(nameof(allowPenetration))]
            public bool pressureAllowed = false;

            [Group("Split/Knockout")]
            [LabelWidth(210)]
            [LabelText("Allowed min velocity")]
            [EnableIf(nameof(allowKnockout))]
            public float knockoutAllowedMinVelocity = Mathf.Infinity;
            [Group("Split/Knockout")]
            [LabelWidth(210)]
            [LabelText("Forced min velocity")]
            [EnableIf(nameof(allowKnockout))]
            public float knockoutForcedMinVelocity = Mathf.Infinity;
            [Group("Split/Knockout")]
            [LabelWidth(210)]
            [LabelText("Allowed min velocity (Throw-Sliced)")]
            [EnableIf(nameof(allowKnockout))]
            public float knockoutThrowAllowedMinVelocity = Mathf.Infinity;
            [Group("Split/Knockout")]
            [LabelWidth(210)]
            [LabelText("Forced min velocity (Throw-Sliced)")]
            [EnableIf(nameof(allowKnockout))]
            public float knockoutThrowForcedMinVelocity = Mathf.Infinity;
            [Group("Split/Knockout")]
            [LabelWidth(210)]
            [LabelText("Duration multiplier")]
            [EnableIf(nameof(allowKnockout))]
            public float knockoutDurationMultiplier = 1;

            [Group("Split/Push")]
            [TableList(ShowElementLabels = true)]
            public PushLevel[] pushLevels;

            [Serializable]
            public class PushLevel
            {
                public float hitVelocity = Mathf.Infinity;
                public float throwVelocity = Mathf.Infinity;
            }

            [Group("Split/Penetration")]
            [LabelWidth(150)]
            [LabelText("Min velocity")]
            [EnableIf(nameof(allowPenetration))]
            public float penetrationMinVelocity = 1;
            [Group("Split/Penetration")]
            [LabelWidth(150)]
            [LabelText("Damper multiplier")]
            [EnableIf(nameof(allowPenetration))]
            public float penetrationDamperMultiplier = 1;
            [Group("Split/Penetration")]
            [LabelWidth(150)]
            [LabelText("Velocity max depth")]
            [EnableIf(nameof(allowPenetration))]
            public bool penetrationVelocityMaxDepth;
            [Group("Split/Penetration")]
            [LabelWidth(150)]
            [LabelText("Velocity max depth curve")]
            [EnableIf(nameof(enablePenetrationVelocityMaxDepthCurve))]
            public AnimationCurve penetrationVelocityMaxDepthCurve;
            private bool enablePenetrationVelocityMaxDepthCurve => allowPenetration && penetrationVelocityMaxDepth;
            [Group("Split/Penetration")]
            [LabelWidth(150)]
            [EnableIf(nameof(enablePressureForceMultiplier))]
            public float pressureForceMultiplier = 1;
            private bool enablePressureForceMultiplier => allowPenetration && pressureAllowed;
        }
    }
}


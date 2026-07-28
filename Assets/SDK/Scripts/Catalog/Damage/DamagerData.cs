using System;
using System.Collections.Generic;
using UnityEngine;

using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(-5)]
#endif
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareHorizontalGroup("Split")]
    [DeclareVerticalGroup("Split/Left")]
    [DeclareBoxGroup("Split/Left/Damage", Title = "Damage")]
    [DeclareBoxGroup("Split/Left/Bad angle", Title = "Bad angle")]
    [DeclareBoxGroup("Split/Left/Animation", Title = "Animation")]
    [DeclareBoxGroup("Split/Left/Force", Title = "Force")]
    [DeclareVerticalGroup("Split/Right")]
    [DeclareBoxGroup("Split/Right/Penetration", Title = "Penetration")]
    [DeclareBoxGroup("Split/Right/Penetration Temporary Modifier", Title = "Penetration Temporary Modifier")]
    [DeclareBoxGroup("Split/Right/Pressure", Title = "Pressure")]
    [DeclareBoxGroup("Split/Right/Dismemberment", Title = "Dismemberment")]
    public class DamagerData : CatalogData
    {
        [Group("General")]
        [Dropdown(nameof(GetAllDamagerModifierID))]
        public string damageModifierId;
        [NonSerialized]
        public DamageModifierData damageModifierData;

        public TriDropdownList<string> GetAllDamagerModifierID() => Catalog.GetDropdownAllID(Category.DamageModifier);

        public TriDropdownList<string> GetAllMaterialID() => Catalog.GetDropdownAllID(Category.Material);

        [TableList(ShowElementLabels = true)]
        public Tier[] tiers;

        [Serializable]
        [DeclareHorizontalGroup("Damage")]
        [DeclareHorizontalGroup("Dismemberment")]
        public class Tier
        {
            [Group("Damage")]
            [LabelText("Mult.")]
            [LabelWidth(40)]
            public float damageMultiplier = 1;
            [Group("Damage")]
            [LabelText("Player Mult.")]
            [LabelWidth(40)]
            public float playerDamageMultiplier = 1;
            [Group("Damage")]
            [LabelText("V. angle")]
            [LabelWidth(50)]
            public float maxVerticalAngle = 80f;
            [Group("Damage")]
            [LabelText("H. angle")]
            [LabelWidth(50)]
            public float maxHorizontalAngle = 60f;
            [Group("Damage")]
            [LabelText("Norm. angle")]
            [LabelWidth(70)]
            public float maxNormalAngle = 60;
            [Group("Damage")]
            [LabelText("Grip disable chance")]
            [LabelWidth(100)]
            public float gripDisableChance = 0f;

            [Group("Dismemberment")]
            [LabelText("Velocity Mult.")]
            [LabelWidth(90)]
            public float dismembermentMinVelocityMultiplier = 1f;
            [Group("Dismemberment")]
            [LabelText("V. angle")]
            [LabelWidth(50)]
            public float dismembermentMaxVerticalAngle = 30;
            [Group("Dismemberment")]
            [LabelText("H. angle")]
            [LabelWidth(50)]
            public float dismembermentMaxHorizontalAngle = 30;

        }

        public override int GetCurrentVersion()
        {
            return 1;
        }


        [Group("Split/Left/Damage")]
        public AnimationCurve velocityDamageCurve = new(new Keyframe(2, 1), new Keyframe(12, 8));
        [Group("Split/Left/Damage")]
        public float minSelfVelocity = 0;
        [Group("Split/Left/Damage")]
        public float intensityMinVelocity;
        [Group("Split/Left/Damage")]
        public float intensityMaxVelocity;
        [Group("Split/Left/Damage")]
        public float hitDelayByCollider = 0.1f;

        [Group("Split/Left/Damage")]
        public float playerMinDamage = 5;
        [Group("Split/Left/Damage")]
        public float playerMaxDamage = 15;

        [Group("Split/Left/Damage")]
        public bool selfDamage;
        [Group("Split/Left/Damage")]
        [ShowIf("selfDamage")]
        public AnimationCurve staticVelocityDamageCurve;
        [Group("Split/Left/Damage")]
        [HideIf("selfDamage")]
        public float throwedMultiplier = 2;
        [Group("Split/Left/Damage")]
        public bool handleDamager = false;

        [Group("Split/Left/Bad angle")]
        public bool badAngleBluntFallback;
        [Group("Split/Left/Bad angle")]
        [ShowIf("badAngleBluntFallback")]
        public float badAngleDamage = 1;
        [Group("Split/Left/Bad angle")]
        [Dropdown(nameof(GetAllDamagerModifierID))]
        [ShowIf("badAngleBluntFallback")]
        public string badAngleMaterialDamageId;
        [NonSerialized]
        public DamageModifierData badAngleModifierData;

        [Group("Split/Left/Animation")]
        public float dyingAnimationMaxVelocity = 8;

        [Group("Split/Left/Force")]
        public float addForce = 0;
        private bool addForceOverZero => addForce > 0;
        [Group("Split/Left/Force")]
        [ShowIf(nameof(addForceOverZero))]
        public ObjectState addForceState = ObjectState.Handled;
        [Group("Split/Left/Force")]
        [ShowIf(nameof(addForceOverZero))]
        public TargetType addForceTargetType = TargetType.All;
        [Group("Split/Left/Force")]
        [ShowIf(nameof(addForceOverZero))]
        public ForceMode addForceMode = ForceMode.Impulse;
        [Group("Split/Left/Force")]
        [ShowIf(nameof(addForceOverZero))]
        public float addForceDuration = 0;
        [Group("Split/Left/Force")]
        [ShowIf(nameof(addForceOverZero))]
        public float addForceRagdollPartMultiplier = 1;
        [Group("Split/Left/Force")]
        [ShowIf(nameof(addForceOverZero))]
        public float addForceRagdollOtherMultiplier = 0;
        [Group("Split/Left/Force")]
        [ShowIf(nameof(addForceOverZero))]
        public float addForceSlowMoMultiplier = 1;
        [Group("Split/Left/Force")]
        [ShowIf(nameof(addForceOverZero))]
        public bool addForceNormalize = false;

        [Group("Split/Right/Penetration")]
        public bool penetrationAllowed;

        [Group("Split/Right/Penetration")]
        [ShowIf("penetrationAllowed")]
        public float penetrationDeepDepthMultiplier = 1;
        [Group("Split/Right/Penetration")]
        [ShowIf("penetrationAllowed")]
        public float penetrationDamage = 20;
        [Group("Split/Right/Penetration")]
        [ShowIf("penetrationAllowed")]
        public bool penetrationEffect = true;

        [Group("Split/Right/Penetration")]
        [ShowIf("penetrationAllowed")]
        public float penetrationInitialVelocityMultiplier = 1;
        [Group("Split/Right/Penetration")]
        [ShowIf("penetrationAllowed")]
        public bool penetrationSkewerDetection = false;
        [Group("Split/Right/Penetration")]
        [ShowIf("penetrationAllowed")]
        [ShowIf("penetrationSkewerDetection")]
        public float penetrationSkewerDamage = 0;

        [Group("Split/Right/Penetration")]
        [ShowIf("penetrationAllowed")]
        public float penetrationHeldDamperIn = 800;
        [Group("Split/Right/Penetration")]
        [ShowIf("penetrationAllowed")]
        public float penetrationHeldDamperOut = 1000;
        [Group("Split/Right/Penetration")]
        [ShowIf("penetrationAllowed")]
        public float penetrationDamper = 5000;
        [Group("Split/Right/Penetration")]
        [ShowIf("penetrationAllowed")]

        public float penetrationShortDepth = 0f;
        [Group("Split/Right/Penetration")]
        [ShowIf("penetrationAllowed")]
        public float penetrationShortDepthAngle = 30;

        [Group("Split/Right/Penetration")]
        [ShowIf("penetrationAllowed")]
        public bool penetrationAllowSlide = false;
        [Group("Split/Right/Penetration")]
        [ShowIf("penetrationAllowed")]
        [ShowIf("penetrationAllowSlide")]
        public float penetrationSlideDamper = 100;

        [Group("Split/Right/Penetration Temporary Modifier")]
        [ShowIf("penetrationAllowed")]
        public PenetrationTempModifier penetrationTempModifier = PenetrationTempModifier.OnThrow;
        private bool penetrationTempMod => (int)penetrationTempModifier > 0;
        [Group("Split/Right/Penetration Temporary Modifier")]
        [ShowIf("penetrationAllowed")]
        [ShowIf(nameof(penetrationTempMod))]
        public float penetrationTempModifierDuration = 0.5f;
        [Group("Split/Right/Penetration Temporary Modifier")]
        [ShowIf("penetrationAllowed")]
        [ShowIf(nameof(penetrationTempMod))]
        public float penetrationTempModifierDamperIn = 0;
        [Group("Split/Right/Penetration Temporary Modifier")]
        [ShowIf("penetrationAllowed")]
        [ShowIf(nameof(penetrationTempMod))]
        public float penetrationTempModifierDamperOut = 5000;

        [Group("Split/Right/Pressure")]
        public bool penetrationPressureAllowed = false;
        private bool showPenetrationPressureSettings => penetrationAllowed && penetrationPressureAllowed;
        [Group("Split/Right/Pressure")]
        [ShowIf("penetrationPressureAllowed")]
        [ShowIf(nameof(showPenetrationPressureSettings))]
        public AnimationCurve penetrationPressureForceCurve = new(new Keyframe(0, 5), new Keyframe(2, 0.5f));
        [Group("Split/Right/Pressure")]
        [ShowIf("penetrationPressureAllowed")]
        [ShowIf(nameof(showPenetrationPressureSettings))]
        public float penetrationPressureMaxDot = 0.2f;

        [Group("Split/Right/Dismemberment")]
        public bool dismembermentAllowed = false;
        private bool showDismemberSettings => penetrationAllowed && dismembermentAllowed;
        [Group("Split/Right/Dismemberment")]
        [ShowIf("dismembermentAllowed")]
        [ShowIf(nameof(showDismemberSettings))]
        public float dismembermentMinVelocity = 10f;
        [Group("Split/Right/Dismemberment")]
        [ShowIf("dismembermentAllowed")]
        [ShowIf(nameof(showDismemberSettings))]
        public float dismembermentNoPenetrationDuration = 1f;

        [Flags]
        public enum PenetrationTempModifier
        {
            OnHit = 1,
            OnThrow = 2,
        }

        public enum KnockoutCondition
        {
            Disabled,
            BluntOnly,
            Always,
        }

        public enum TargetType
        {
            Object,
            Creature,
            All,
        }

        public enum ObjectState
        {
            Flying,
            Handled,
            All,
        }

    }
}


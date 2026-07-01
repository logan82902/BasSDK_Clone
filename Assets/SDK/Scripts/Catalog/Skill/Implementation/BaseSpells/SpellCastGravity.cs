using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad.Skill.SpellMerge;
using TriInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Split/Right/Misc", Title = "Misc")]
    [DeclareBoxGroup("Split/Right/Throw", Title = "Throw")]
    [DeclareBoxGroup("Split/Right/Dual Push", Title = "Dual Push")]
    [DeclareBoxGroup("Split/Right/Push Status Effect", Title = "Push Status Effect")]
    [DeclareBoxGroup("Split/Right/Imbue", Title = "Imbue")]
    [DeclareBoxGroup("Split/Right/Imbue hit", Title = "Imbue hit")]
    [DeclareBoxGroup("Split/Right/Staff Slam", Title = "Staff Slam")]
    public class SpellCastGravity : SpellCastCharge
    {
        public const string LastPushTimeVariable = "LastPushTime";
        public const string LastPushDirection = "LastPushDirection";

        [Group("Split/Right/Misc")]
        public bool enableCreatureControl = false;

        [Group("Split/Right/Throw")]
        public float halfSphereRadius = 2;
        [Group("Split/Right/Throw")]
        [MinMaxSlider(0, 4f, true)]
        public Vector2 pushLevel = new(1, 3);
        [Group("Split/Right/Throw")]
        public float pushMaxForce = 10;
        [Group("Split/Right/Throw")]
        public float pushForceOnPlayer = 0.3f;
        [Group("Split/Right/Throw")]
        public float upwardsForceLerpOnPlayer = 0.3f;
        [Group("Split/Right/Throw")]
        public float playerLocomotionDeactivate = 0.2f;
        [Group("Split/Right/Throw")]
        public bool pushUseExplosionForce = true;
        [Group("Split/Right/Throw")]
        [ShowIf("pushUseExplosionForce")]
        public float pushExplosionUpwardModifier = 1f;
        [Group("Split/Right/Throw")]
        public ForceMode pushForceMode = ForceMode.Impulse;
        [Group("Split/Right/Throw")]
        public LayerMask pushLayerMask;
        [Group("Split/Right/Throw")]
        public float minChargeToPushCreature = 0.5f;
        [Group("Split/Right/Throw")]
        [LabelText("$" + nameof(IntensityPerSkillLabel))]
        public float intensityPerSkill = 0.05f;

        [JsonIgnore]
        public string IntensityPerSkillLabel => SkillPassiveLabel("Push Force", intensityPerSkill);

        [Group("Split/Right/Throw")]
        public float pushStatusExtension = 3f;

        [Group("Split/Right/Throw")]
        public float pushStatusForce = 1.3f;

        [Group("Split/Right/Dual Push")]
        public float dualPushIntensityMultiplier = 0.4f;

        [Group("Split/Right/Dual Push")]
        public float dualPushTimeWindow = 0.3f;

        [Group("Split/Right/Dual Push")]
        public float dualPushAngleWindow = 50;

        [Group("Split/Right/Dual Push")]
        [Range(0, 3)]
        public float dualPushLevelReduction = 0.5f;
        [Group("Split/Right/Dual Push")]
        [Dropdown(nameof(GetAllEffectID))]
        public string dualPushEffectId;
        protected EffectData dualPushEffectData;

        [Group("Split/Right/Throw")]
        [Dropdown(nameof(GetAllEffectID))]
        public string pushEffectId;
        protected EffectData pushEffectData;

        [Group("Split/Right/Push Status Effect")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string pushStatusEffectId = "Floating";
        
        [NonSerialized]
        public StatusData pushStatusEffect;
        [Group("Split/Right/Push Status Effect")]
        public float pushStatusDuration = 2;
        [Group("Split/Right/Imbue")]
        public float imbueGravityRatio = 0;
        [Group("Split/Right/Imbue")]
        public float imbueDrag = -1;
        [Group("Split/Right/Imbue")]
        public float imbueAngularDrag = -1;
        [Group("Split/Right/Imbue")]
        public float imbueGravityFalloffThreshold = 0.3f;

        [Group("Split/Right/Imbue hit")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueHitEffectId;
        protected EffectData imbueHitEffectData;
        [Group("Split/Right/Imbue hit")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueHitItemEffectId;
        protected EffectData imbueHitItemEffectData;
        [Group("Split/Right/Imbue hit")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueHitRagdollEffectId;
        protected EffectData imbueHitRagdollEffectData;

        [Group("Split/Right/Imbue hit")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string imbueStatusEffectId = "Floating";
        
        [NonSerialized]
        public StatusData imbueStatusEffect;

        [Group("Split/Right/Imbue hit")]
        public float imbueHitItemZeroGravityDuration = 5;
        [Group("Split/Right/Imbue hit")]
        public float imbueHitRagdollZeroGravityDuration = 5;
        [Group("Split/Right/Imbue hit")]
        public float imbueHitGravityRatio = 0.1f;
        [Group("Split/Right/Imbue hit")]
        public float imbueHitMassRatio = 0.5f;


        [Group("Split/Right/Staff Slam")]
        public int staffSlamPushLevel = 2;
        [Group("Split/Right/Staff Slam")]
        public float staffSlamExplosionUpwardModifier = 1f;
        [Group("Split/Right/Staff Slam")]
        public float staffSlamWaveSpeed = 12f;
        [Group("Split/Right/Staff Slam")]
        public float staffSlamWaveUpdateRate = 0.05f;
        [Group("Split/Right/Staff Slam")]
        public float staffSlamMinRadius = 2f;
        [Group("Split/Right/Staff Slam")]
        public float staffSlamMaxRadius = 8f;
        [Group("Split/Right/Staff Slam")]
        public float staffSlamMinForce = 10f;
        [Group("Split/Right/Staff Slam")]
        public float staffSlamMaxForce = 15f;
        [Group("Split/Right/Staff Slam")]
        public ForceMode staffSlamForceMode = ForceMode.VelocityChange;
        [Group("Split/Right/Staff Slam")]
        public float staffSlamStatusDuration = 5;
        [Group("Split/Right/Staff Slam")]
        public EffectOnPlayer staffSlamPlayerEffect = EffectOnPlayer.PushWhenJumping;

        [NonSerialized]
        protected float lastImbueFalloffAmount;
        [NonSerialized]
        public GameObject hoverEffectObj;
        [NonSerialized]
        public float hoverIntensity;
        
        public enum EffectOnPlayer {
            None,
            PushWhenJumping,
            PushAlways
        }


        public new SpellCastGravity Clone()
        {
            return MemberwiseClone() as SpellCastGravity;
        }

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            pushEffectData = Catalog.GetData<EffectData>(pushEffectId);
            pushStatusEffect = Catalog.GetData<StatusData>(pushStatusEffectId);
            dualPushEffectData = Catalog.GetData<EffectData>(dualPushEffectId);
            imbueStatusEffect = Catalog.GetData<StatusData>(imbueStatusEffectId);
            imbueHitEffectData = Catalog.GetData<EffectData>(imbueHitEffectId);
            imbueHitItemEffectData = Catalog.GetData<EffectData>(imbueHitItemEffectId);
            imbueHitRagdollEffectData = Catalog.GetData<EffectData>(imbueHitRagdollEffectId);
        }

    }
}
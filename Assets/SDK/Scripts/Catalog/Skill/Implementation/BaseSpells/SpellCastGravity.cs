using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using ThunderRoad.Skill.SpellMerge;
using UnityEngine;
using Object = UnityEngine.Object;
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Misc", Title = "Misc")]
    [DeclareBoxGroup("Throw", Title = "Throw")]
    [DeclareBoxGroup("Dual Push", Title = "Dual Push")]
    [DeclareBoxGroup("Push Status Effect", Title = "Push Status Effect")]
    [DeclareBoxGroup("Imbue", Title = "Imbue")]
    [DeclareBoxGroup("Imbue hit", Title = "Imbue hit")]
    [DeclareBoxGroup("Staff Slam", Title = "Staff Slam")]
    [DeclareBoxGroup("Musket")]
    public class SpellCastGravity : SpellCastCharge
    {
        public const string LastPushTimeVariable = "LastPushTime";
        public const string LastPushDirection = "LastPushDirection";

        [Group("Misc")]
        public bool enableCreatureControl = false;

        [Group("Throw")]
        public float halfSphereRadius = 2;
        [Group("Throw")]
        [MinMaxSlider(0, 4f, true)]
        public Vector2 pushLevel = new(1, 3);
        [Group("Throw")]
        public float pushMaxForce = 10;
        [Group("Throw")]
        public float pushForceOnPlayer = 0.3f;
        [Group("Throw")]
        public float upwardsForceLerpOnPlayer = 0.3f;
        [Group("Throw")]
        public float playerLocomotionDeactivate = 0.2f;
        [Group("Throw")]
        public bool pushUseExplosionForce = true;
        [Group("Throw")]
        [ShowIf("pushUseExplosionForce")]
        public float pushExplosionUpwardModifier = 1f;
        [Group("Throw")]
        public ForceMode pushForceMode = ForceMode.Impulse;
        [Group("Throw")]
        public LayerMask pushLayerMask;
        [Group("Throw")]
        public float minChargeToPushCreature = 0.5f;
        [Group("Throw")]
        [LabelText("$" + nameof(IntensityPerSkillLabel))]
        public float intensityPerSkill = 0.05f;

        [JsonIgnore]
        public string IntensityPerSkillLabel => SkillPassiveLabel("Push Force", intensityPerSkill);

        [Group("Throw")]
        public float pushStatusExtension = 3f;

        [Group("Throw")]
        public float pushStatusForce = 1.3f;

        [Group("Dual Push")]
        public float dualPushIntensityMultiplier = 0.4f;

        [Group("Dual Push")]
        public float dualPushTimeWindow = 0.3f;

        [Group("Dual Push")]
        public float dualPushAngleWindow = 50;

        [Group("Dual Push")]
        [Range(0, 3)]
        public float dualPushLevelReduction = 0.5f;
        [Group("Dual Push")]
        [Dropdown(nameof(GetAllEffectID))]
        public string dualPushEffectId;
        protected EffectData dualPushEffectData;

        [Group("Throw")]
        [Dropdown(nameof(GetAllEffectID))]
        public string pushEffectId;
        protected EffectData pushEffectData;

        [Group("Push Status Effect")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string pushStatusEffectId = "Floating";
        
        [NonSerialized]
        public StatusData pushStatusEffect;
        [Group("Push Status Effect")]
        public float pushStatusDuration = 2;
        [Group("Imbue")]
        public float imbueGravityRatio = 0;
        [Group("Imbue")]
        public float imbueDrag = -1;
        [Group("Imbue")]
        public float imbueAngularDrag = -1;
        [Group("Imbue")]
        public float imbueGravityFalloffThreshold = 0.3f;

        [Group("Imbue hit")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueHitEffectId;
        protected EffectData imbueHitEffectData;
        [Group("Imbue hit")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueHitItemEffectId;
        protected EffectData imbueHitItemEffectData;
        [Group("Imbue hit")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueHitRagdollEffectId;
        protected EffectData imbueHitRagdollEffectData;

        [Group("Imbue hit")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string imbueStatusEffectId = "Floating";
        
        [NonSerialized]
        public StatusData imbueStatusEffect;

        [Group("Imbue hit")]
        public float imbueHitItemZeroGravityDuration = 5;
        [Group("Imbue hit")]
        public float imbueHitRagdollZeroGravityDuration = 5;
        [Group("Imbue hit")]
        public float imbueHitGravityRatio = 0.1f;
        [Group("Imbue hit")]
        public float imbueHitMassRatio = 0.5f;


        [Group("Staff Slam")]
        public int staffSlamPushLevel = 2;
        [Group("Staff Slam")]
        public float staffSlamExplosionUpwardModifier = 1f;
        [Group("Staff Slam")]
        public float staffSlamWaveSpeed = 12f;
        [Group("Staff Slam")]
        public float staffSlamWaveUpdateRate = 0.05f;
        [Group("Staff Slam")]
        public float staffSlamMinRadius = 2f;
        [Group("Staff Slam")]
        public float staffSlamMaxRadius = 8f;
        [Group("Staff Slam")]
        public float staffSlamMinForce = 10f;
        [Group("Staff Slam")]
        public float staffSlamMaxForce = 15f;
        [Group("Staff Slam")]
        public ForceMode staffSlamForceMode = ForceMode.VelocityChange;
        [Group("Staff Slam")]
        public float staffSlamStatusDuration = 5;
        [Group("Staff Slam")]
        public EffectOnPlayer staffSlamPlayerEffect = EffectOnPlayer.PushWhenJumping;

        [Group("Musket")]
        public float musketRadius = 2;

        [Group("Musket")]
        public LayerMask musketDecapitateLayerMask;

        [Group("Musket")]
        public float musketForce = 50;

        [Group("Musket")]
        public float musketExplosionRadius = 5;

        [Group("Musket")]
        public float musketExplosionForce = 2;

        [Group("Musket")]
        public float musketHitCrystalDamage = 60;

        [Group("Musket")]
        public float musketExplosionCrystalDamage = 15;

        [Group("Musket")]
        [Dropdown(nameof(GetAllItemID))]
        public string musketProjectileId;
        protected ItemData musketProjectileData;

        [Group("Musket")]
        [Dropdown(nameof(GetAllDamagerID))]
        public string musketProjectileDamagerId;
        protected DamagerData musketProjectileDamagerData;

        [Group("Musket")]
        public float musketShootSpeed = 40;

        [Group("Musket")]
        public float musketPlayerForce = 10f;

        [Group("Musket")]
        public string musketProjectileExplosionEffectId;
        protected EffectData musketProjectileExplosionEffectData;

        [Group("Musket")]
        public string musketProjectileEffectId;
        protected EffectData musketProjectileEffectData;

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
            musketProjectileData = Catalog.GetData<ItemData>(musketProjectileId);
            musketProjectileDamagerData = Catalog.GetData<DamagerData>(musketProjectileDamagerId);
            musketProjectileEffectData = Catalog.GetData<EffectData>(musketProjectileEffectId);
            musketProjectileExplosionEffectData = Catalog.GetData<EffectData>(musketProjectileExplosionEffectId);
        }

    }
}
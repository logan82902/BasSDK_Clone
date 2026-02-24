using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using TriInspector;
using Random = UnityEngine.Random;
using Newtonsoft.Json;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Grip Cast", Title = "Grip Cast")]
    [DeclareBoxGroup("Cast Heat Zone", Title = "Cast Heat Zone")]
    [DeclareBoxGroup("Projectile", Title = "Projectile")]
    [DeclareBoxGroup("Fire Sparks", Title = "Fire Sparks")]
    [DeclareBoxGroup("Imbue", Title = "Imbue")]
    [DeclareBoxGroup("Imbue use", Title = "Imbue use")]
    [DeclareBoxGroup("Staff Slam", Title = "Staff Slam")]
    [DeclareBoxGroup("Musket")]
    public class SpellCastProjectile : SpellCastCharge
    {
        [Group("Grip Cast")]
        public float gripCastHeatTransfer = 40;
        [Group("Cast Heat Zone")]
        public bool allowCastHeat = true;
        [Group("Cast Heat Zone")]
        [ShowIf(nameof(allowCastHeat))]
        public float spellCastHeatPerSecond = 20;

        [Group("Cast Heat Zone")]
        [ShowIf(nameof(allowCastHeat))]
        public float castHeatRadius = 0.1f;
        [Group("Projectile")]
        [LabelText("$" + nameof(IntensityPerSkillLabel))]
        public float intensityPerSkill = 0.1f;
        [JsonIgnore]
        public string IntensityPerSkillLabel => SkillPassiveLabel("Damage", intensityPerSkill);
        [Group("Projectile")]
        [LabelText("$" + nameof(DurationPerSkillLabel))]
        public float durationPerSkill = 0.1f;
        [JsonIgnore]
        public string DurationPerSkillLabel => SkillPassiveLabel("Burn Duration", durationPerSkill);
        [Group("Projectile")]
        public bool destroyInWater = false;
        [Group("Projectile")]
        [Dropdown(nameof(GetAllEffectID))]
        public string projectileEffectId;
        [NonSerialized]
        public EffectData projectileEffectData;

        [Group("Projectile")]
        [Dropdown(nameof(GetAllItemID))]
        public string projectileId;
        [NonSerialized]
        public ItemData projectileData;

        [Group("Projectile")]
        [Dropdown(nameof(GetAllDamagerID))]
        public string projectileDamagerId;
        [NonSerialized]
        public DamagerData projectileDamagerData;

        [Group("Projectile")]
        public readonly AnimationCurve damageOverTimeCurve = AnimationCurve.Linear(1, 1, 1, 1);


        [Group("Projectile")]
        public float projectileImbueEnergyTransferred = 0;
        [Group("Projectile")]
        [Range(0f, 1f)]
        public float throwHeadBias = 0.5f;
        [Group("Projectile")]
        public float projectileVelocity = 5;
        [Group("Projectile")]
        public bool projectilePlayerGuided = true;
        [Group("Projectile")]
        public float projectileGuidanceDelay = 0.5f;

        [Group("Projectile")]
        public bool projectileAllowDeflect = true;
        [Group("Projectile")]
        [Dropdown(nameof(GetAllEffectID))]
        public string projectileDeflectEffectId;
        [NonSerialized]
        public EffectData projectileDeflectEffectData;

        [Group("Fire Sparks")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string fireSparkStatusId = "Burning";
        [NonSerialized]
        public StatusData fireSparkStatusEffect;

        [Group("Fire Sparks")]
        public float fireSparkStatusDuration = 5;

        [Group("Fire Sparks")]
        public float fireSparkHeatTransfer = 10;

        [Group("Imbue")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string imbueHitStatusEffectId = "Burning";
        [NonSerialized]
        protected StatusData imbueHitStatusEffect;
        [Group("Imbue")]
        public float imbueHitStatusDuration = Mathf.Infinity;
        [Group("Imbue")]
        public float imbueHitHeat = 20;
        [Group("Imbue")]
        public float imbuePenetrateHeatPerSecond = 10;

        [Group("Imbue use")]
        public float imbueUseConsumption = 10;
        [Group("Imbue use")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueUseEffectId;
        [NonSerialized]
        public EffectData imbueUseEffectData;
        [Group("Imbue use")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueUseProjectileEffectId;
        [NonSerialized]
        public EffectData imbueUseProjectileEffectData;

        [Group("Imbue use")]
        public FireMode imbueShootFireMode = FireMode.InheritVelocity;

        [Group("Imbue use")]
        public TriggerType imbueShootTriggerMode = TriggerType.ButtonUp;

        [Group("Staff Slam")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueHitProjectileEffectId;
        [NonSerialized]
        public EffectData imbueHitProjectileEffectData;
        [Group("Staff Slam")]
        protected float staffSlamFireballSpeed = 15f;
        [Group("Staff Slam")]
        public int staffSlamNumFireballs = 6;
        [Group("Staff Slam")]
        public float slamOnCrystalDistance = 1f;

        [Group("Musket")]
        public float musketDamage = 30f;
        [Group("Musket")]
        public float musketRadius = 2f;
        [Group("Musket")]
        public LayerMask musketDecapitateLayerMask;
        [Group("Musket")]
        public int fireSparkCount = 6;
        [Group("Musket")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string musketStatusId;
        protected StatusData musketStatusData;
        [Group("Musket")]
        [Dropdown(nameof(GetAllEffectID))]
        public string musketSparkEffectId;
        protected EffectData musketSparkEffectData;
        [Group("Musket")]
        public float musketForce = 20f;
        [Group("Musket")]
        public float musketStatusDuration = Mathf.Infinity;
        [Group("Musket")]
        public float musketStatusParameterCone = 100;
        [Group("Musket")]
        public float musketStatusParameterParticle = 100;
        [Group("Musket")]
        public Vector2 musketSparkVelocity = new Vector2(15f, 25f);

        protected SkillTwinFlame twinFlameSkill;
        
        
        
        protected HashSet<Creature> creaturesHeatedThisFrame;

        public new SpellCastProjectile Clone()
        {
            return this.MemberwiseClone() as SpellCastProjectile;
        }

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();

            projectileData = Catalog.GetData<ItemData>(projectileId);
            projectileDamagerData = Catalog.GetData<DamagerData>(projectileDamagerId);
            projectileEffectData = Catalog.GetData<EffectData>(projectileEffectId);
            projectileDeflectEffectData = Catalog.GetData<EffectData>(projectileDeflectEffectId);
            imbueHitStatusEffect = Catalog.GetData<StatusData>(imbueHitStatusEffectId);
            fireSparkStatusEffect = Catalog.GetData<StatusData>(fireSparkStatusId);
            imbueUseEffectData = Catalog.GetData<EffectData>(imbueUseEffectId);
            imbueHitProjectileEffectData = Catalog.GetData<EffectData>(imbueHitProjectileEffectId);
            imbueUseProjectileEffectData = Catalog.GetData<EffectData>(imbueUseProjectileEffectId);
            musketStatusData = Catalog.GetData<StatusData>(musketStatusId);
            musketSparkEffectData = Catalog.GetData<EffectData>(musketSparkEffectId);
            twinFlameSkill = Catalog.GetData<SkillTwinFlame>("TwinFlame");

        }

        public enum FireMode
        {
            Forwards,
            InheritVelocity
        }

        public enum TriggerType
        {
            ButtonDown,
            ButtonUp
        }

    }
}

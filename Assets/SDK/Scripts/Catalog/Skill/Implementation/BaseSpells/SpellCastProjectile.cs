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
    [DeclareBoxGroup("Split/Right/Grip Cast", Title = "Grip Cast")]
    [DeclareBoxGroup("Split/Right/Cast Heat Zone", Title = "Cast Heat Zone")]
    [DeclareBoxGroup("Split/Right/Projectile", Title = "Projectile")]
    [DeclareBoxGroup("Split/Right/Fire Sparks", Title = "Fire Sparks")]
    [DeclareBoxGroup("Split/Right/Imbue", Title = "Imbue")]
    [DeclareBoxGroup("Split/Right/Imbue use", Title = "Imbue use")]
    [DeclareBoxGroup("Split/Right/Staff Slam", Title = "Staff Slam")]
    public class SpellCastProjectile : SpellCastCharge
    {
        [Group("Split/Right/Grip Cast")]
        public float gripCastHeatTransfer = 40;
        [Group("Split/Right/Cast Heat Zone")]
        public bool allowCastHeat = true;
        [Group("Split/Right/Cast Heat Zone")]
        [ShowIf(nameof(allowCastHeat))]
        public float spellCastHeatPerSecond = 20;

        [Group("Split/Right/Cast Heat Zone")]
        [ShowIf(nameof(allowCastHeat))]
        public float castHeatRadius = 0.1f;
        [Group("Split/Right/Projectile")]
        [LabelText("$" + nameof(IntensityPerSkillLabel))]
        public float intensityPerSkill = 0.1f;
        [JsonIgnore]
        public string IntensityPerSkillLabel => SkillPassiveLabel("Damage", intensityPerSkill);
        [Group("Split/Right/Projectile")]
        [LabelText("$" + nameof(DurationPerSkillLabel))]
        public float durationPerSkill = 0.1f;
        [JsonIgnore]
        public string DurationPerSkillLabel => SkillPassiveLabel("Burn Duration", durationPerSkill);
        [Group("Split/Right/Projectile")]
        public bool destroyInWater = false;
        [Group("Split/Right/Projectile")]
        [Dropdown(nameof(GetAllEffectID))]
        public string projectileEffectId;
        [NonSerialized]
        public EffectData projectileEffectData;

        [Group("Split/Right/Projectile")]
        [Dropdown(nameof(GetAllItemID))]
        public string projectileId;
        [NonSerialized]
        public ItemData projectileData;

        [Group("Split/Right/Projectile")]
        [Dropdown(nameof(GetAllDamagerID))]
        public string projectileDamagerId;
        [NonSerialized]
        public DamagerData projectileDamagerData;

        [Group("Split/Right/Projectile")]
        public readonly AnimationCurve damageOverTimeCurve = AnimationCurve.Linear(1, 1, 1, 1);


        [Group("Split/Right/Projectile")]
        public float projectileImbueEnergyTransferred = 0;
        [Group("Split/Right/Projectile")]
        [Range(0f, 1f)]
        public float throwHeadBias = 0.5f;
        [Group("Split/Right/Projectile")]
        public float projectileVelocity = 5;
        [Group("Split/Right/Projectile")]
        public bool projectilePlayerGuided = true;
        [Group("Split/Right/Projectile")]
        public float projectileGuidanceDelay = 0.5f;

        [Group("Split/Right/Projectile")]
        public bool projectileAllowDeflect = true;
        [Group("Split/Right/Projectile")]
        [Dropdown(nameof(GetAllEffectID))]
        public string projectileDeflectEffectId;
        [NonSerialized]
        public EffectData projectileDeflectEffectData;

        [Group("Split/Right/Fire Sparks")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string fireSparkStatusId = "Burning";
        [NonSerialized]
        public StatusData fireSparkStatusEffect;

        [Group("Split/Right/Fire Sparks")]
        public float fireSparkStatusDuration = 5;

        [Group("Split/Right/Fire Sparks")]
        public float fireSparkHeatTransfer = 10;

        [Group("Split/Right/Imbue")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string imbueHitStatusEffectId = "Burning";
        [NonSerialized]
        protected StatusData imbueHitStatusEffect;
        [Group("Split/Right/Imbue")]
        public float imbueHitStatusDuration = Mathf.Infinity;
        [Group("Split/Right/Imbue")]
        public float imbueHitHeat = 20;
        [Group("Split/Right/Imbue")]
        public float imbuePenetrateHeatPerSecond = 10;

        [Group("Split/Right/Imbue use")]
        public float imbueUseConsumption = 10;
        [Group("Split/Right/Imbue use")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueUseEffectId;
        [NonSerialized]
        public EffectData imbueUseEffectData;
        [Group("Split/Right/Imbue use")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueUseProjectileEffectId;
        [NonSerialized]
        public EffectData imbueUseProjectileEffectData;

        [Group("Split/Right/Imbue use")]
        public FireMode imbueShootFireMode = FireMode.InheritVelocity;

        [Group("Split/Right/Imbue use")]
        public TriggerType imbueShootTriggerMode = TriggerType.ButtonUp;

        [Group("Split/Right/Staff Slam")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueHitProjectileEffectId;
        [NonSerialized]
        public EffectData imbueHitProjectileEffectData;
        [Group("Split/Right/Staff Slam")]
        protected float staffSlamFireballSpeed = 15f;
        [Group("Split/Right/Staff Slam")]
        public int staffSlamNumFireballs = 6;
        [Group("Split/Right/Staff Slam")]
        public float slamOnCrystalDistance = 1f;



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

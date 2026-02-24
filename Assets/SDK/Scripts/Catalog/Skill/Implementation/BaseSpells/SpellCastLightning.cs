using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Imbue hit", Title = "Imbue hit")]
    [DeclareBoxGroup("Staff Slam", Title = "Staff Slam")]
    [DeclareBoxGroup("Bolts", Title = "Bolts")]
    [DeclareBoxGroup("Electrocution", Title = "Electrocution")]
    [DeclareBoxGroup("Chaining", Title = "Chaining")]
    [DeclareBoxGroup("Arcing Bolt", Title = "Arcing Bolt")]
    [DeclareBoxGroup("Musket")]
    public class SpellCastLightning : SpellCastCharge
    {
        public static readonly Type[] MUSKET_IGNORED_EFFECT_MODULES = { typeof(EffectModuleAudio) };
        
        public bool speedUpByTimeScale = false;
        public static int modifierChainRange = Animator.StringToHash("ChainRange");
        [Group("Imbue hit")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueHitEffectId;

        [NonSerialized]
        public EffectData imbueHitEffectData;

        [Group("Imbue hit")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueHitRagdollEffectId;

        [NonSerialized]
        public EffectData imbueHitRagdollEffectData;

        [Group("Staff Slam")]
        public float staffSlamRadius = 10f;

        [Group("Staff Slam")]
        public float staffSlamStunDuration = 5f;

        [Group("Staff Slam")]
        public float staffSlamExpandDuration = 1f;

        [Group("Bolts")]
        public AnimationCurve boltIntensityCurve;

        [Group("Bolts")]
        [Dropdown(nameof(GetAllEffectID))]
        public string boltEffectId;
        [NonSerialized]
        public EffectData boltEffectData;

        [Group("Bolts")]
        [Dropdown(nameof(GetAllEffectID))]
        public string boltLoopEffectId;
        [NonSerialized]
        public EffectData boltLoopEffectData;

        [Group("Bolts")]
        [Dropdown(nameof(GetAllEffectID))]
        public string boltHitEffectId;
        [NonSerialized]
        public EffectData boltHitEffectData;

        [Group("Bolts")]
        [Dropdown(nameof(GetAllEffectID))]
        public string boltHitImbueEffectId;
        [NonSerialized]
        public EffectData boltHitImbueEffectData;

        [Group("Bolts")]
        public int simultaneousBolts = 3;

        [Group("Bolts")]
        [MinMaxSlider(0.01f, 0.5f, ShowFields = true)]
        public Vector2 intervalMinRange = new(0.1f, 0.2f);

        [Group("Bolts")]
        [MinMaxSlider(0.01f, 0.5f, ShowFields = true)]
        public Vector2 intervalMaxRange = new(0.05f, 0.1f);

        public float MinDuration => Mathf.Lerp(intervalMaxRange.x, intervalMaxRange.y, 0.5f) / drainPerBolt;

        public float MaxDuration => (1 + efficiencyPerSkill * 3) * (Mathf.Lerp(intervalMaxRange.x, intervalMaxRange.y, 0.5f) / drainPerBolt);
        [JsonIgnore]
        public string DrainPerBoltLabel
            => $"Drain per Bolt (spray for ~{MinDuration:G02}s base, ~{MaxDuration:G02}s with unlocks)";
        [JsonIgnore]
        public string StunDurationPerSkillLabel => $"Stun Duration per Skill ({boltElectrocuteDuration}s base, {boltElectrocuteDuration * (1 + durationPerSkill * 3)}s with unlocks)";

        [Group("Bolts")]
        [LabelText("$" + nameof(DrainPerBoltLabel))]
        public float drainPerBolt = 0.015f;

        [Group("Bolts")]
        [LabelText("$" + nameof(EfficiencyPerSkillLabel))]
        public float efficiencyPerSkill = 0.05f;
        [Group("Bolts")]
        [LabelText("$" + nameof(StunDurationPerSkillLabel))]
        public float durationPerSkill = 0.1f;

        [JsonIgnore]
        public string EfficiencyPerSkillLabel => SkillPassiveLabel("Efficiency", efficiencyPerSkill);

        [Group("Bolts")]
        public float coneAngle = 30;

        [Group("Bolts")]
        public float boltMaxRange = 5;

        [Group("Bolts")]
        public float fireDirectionAngle = 70;

        [Group("Bolts")]
        public float boltDamage = 1;

        [Group("Bolts")]
        public int pushLevel = 1;

        [Group("Bolts")]
        public float boltHaptic = 0.2f;

        [Group("Bolts")]
        public int boltElectrocuteDuration = 2;
        [Group("Bolts")]
        public int boltElectrocuteDurationMetal = 6;

        [Group("Bolts")]
        public float boltPushForce = 5f;

        [Group("Bolts")]
        public float headToFireMaxAngle = 45;
        [Group("Bolts")]
        public float boltLoopFadoutDelay = 0.5f;

        [Group("Bolts")]
        public LayerMask rayMaskPlayer;
        [Group("Bolts")]
        public LayerMask rayMaskNpc;

        [Group("Bolts")]
        public float hitLookupMaxDiffRange = 0.25f;
        [Group("Bolts")]
        public float boltEnergyTransfer = 2f;
        [Group("Bolts")]
        public float boltElectrocuteChance = 0.3f;

        [Group("Electrocution")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string electrocuteStatusId = "Electrocute";
        [NonSerialized]
        public StatusData electrocuteStatusData;

        [Group("Chaining")]
        public float minChainIntensity = 0.6f;
        [Group("Chaining")]
        public float minChainLength = 0.4f;
        [Group("Chaining")]
        public float maxChainLength = 1.2f;
        [Group("Chaining")]
        public float chainIntensityMultiplier = 0.75f;
        [Group("Chaining")]
        public int maxChainsPerBolt = 3;
        [Group("Chaining")]
        public float chainChance = 0.5f;
        
        [NonSerialized]
        public bool allowChainEnemies;
        [NonSerialized]
        public bool allowChainItems;


        [Group("Arcing Bolt")]
        [ShowIf("allowThrow")]
        public float arcDuration = 10f;
        [Group("Arcing Bolt")]
        [ShowIf("allowThrow")]
        public float arcJumpRadius = 5f;
        [Group("Arcing Bolt")]
        [ShowIf("allowThrow")]
        public float arcThrowAngle = 30f;
        [Group("Arcing Bolt")]
        [ShowIf("allowThrow")]
        public float arcShockDuration = 1.5f;
        [Group("Arcing Bolt")]
        [ShowIf("allowThrow")]
        public float arcJumpDelay = 1.5f;

        [Group("Musket")]
        [Dropdown(nameof(GetAllSkillID))]
        public string musketThunderboltId;
        [JsonIgnore]
        public SkillThunderbolt musketThunderboltData;

        [Group("Musket")]
        [Dropdown(nameof(GetAllEffectID))]
        public string musketProjectileEffectId;
        [JsonIgnore]
        public EffectData musketProjectileEffectData;

        [Group("Musket")]
        [Dropdown(nameof(GetAllItemID))]
        public string musketProjectileId;
        [JsonIgnore]
        public ItemData musketProjectileData;

        [Group("Musket")]
        [Dropdown(nameof(GetAllDamagerID))]
        public string musketProjectileDamagerId;
        [JsonIgnore]
        protected DamagerData musketProjectileDamagerData;

        [Group("Musket")]
        public int musketProjectileCount = 6;

        [Group("Musket")]
        public float musketProjectileSpeed = 30;

        [Group("Musket")]
        public float musketTargetRadius = 3;

        [Group("Musket")]
        public float musketProjectileAngle = 35;
        
        public delegate void SprayEvent(SpellCastLightning spell);

        public event SprayEvent OnSprayLoopEvent;
        public event SprayEvent OnSprayStopEvent;

        public delegate void ChainEvent(ColliderGroup source, ColliderGroup target);
        public event ChainEvent OnChainEvent;

        public delegate void BoltHitColliderGroupEvent(
            SpellCastLightning spell,
            ColliderGroup colliderGroup,
            Vector3 position,
            Vector3 normal,
            Vector3 velocity,
            float intensity,
            ColliderGroup source,
            HashSet<ThunderEntity> seenEntities);
        public event BoltHitColliderGroupEvent OnBoltHitColliderGroupEvent;
        public delegate void BoltHitEvent(SpellCastLightning spell, BoltHit hit);
        public event BoltHitEvent OnBoltHitEvent;

        public delegate void ChargeSappingEvent(SpellCastLightning spell, SkillChargeSapping skill, EventTime time, SpellCastCharge other);

        public event ChargeSappingEvent OnChargeSappingEvent;

        public delegate void LightningEvent(SpellCastLightning spell);

        public event LightningEvent OnChargeSappingResetEvent;

        [NonSerialized]
        public Gradient boltColorOverride;

        public void ResetBoltColor()
        {
            boltColorOverride = null;
        }
        public void OverrideBoltColor(Gradient gradient)
        {
            boltColorOverride = gradient;
        }
        
        [Serializable]
        public struct BoltHit
        {
            public BoltHit(Collider collider, Vector3 point, float distance, float angle, Vector3 normal, Vector3 direction)
            {
                this.collider = collider;
                this.closestPoint = point;
                this.distance = distance;
                this.angle = angle;
                this.normal = normal;
                this.direction = direction;
            }
            public Collider collider;
            public Vector3 closestPoint;
            public float distance;
            public float angle;
            public Vector3 normal;
            public Vector3 direction;
        }

        public new SpellCastLightning Clone()
        {
            return this.MemberwiseClone() as SpellCastLightning;
        }

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();

        }

    }
}

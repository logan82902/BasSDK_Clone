using System;
using System.Collections.Generic;
using UnityEngine;

using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [Serializable]
    [DeclareBoxGroup("Skill UI", Title = "Skill UI")]
    [DeclareBoxGroup("Charge", Title = "Charge")]
    [DeclareBoxGroup("Imbue", Title = "Imbue")]
    [DeclareBoxGroup("Hand force", Title = "Hand force")]
    [DeclareBoxGroup("Throw", Title = "Throw")]
    [DeclareBoxGroup("Spray", Title = "Spray")]
    [DeclareBoxGroup("Grip Cast", Title = "Grip Cast")]
    [DeclareBoxGroup("Staff Slam", Title = "Staff Slam")]
    [DeclareBoxGroup("Musket")]
    public class SpellCastCharge : SpellCastData
    {
        [Group("Skill UI")]
        public string castDescription;
        [Group("Skill UI")]
        public string imbueDescription;
        [Group("Skill UI")]
        public string slamDescription;

        [Group("Charge")]
        [Dropdown(nameof(GetAllEffectID))]
        public string chargeEffectId;

        [NonSerialized]
        public EffectData chargeEffectData;

        [Group("Charge")]
        [Dropdown(nameof(GetAllEffectID))]
        public string readyEffectId;

        [NonSerialized]
        public EffectData readyEffectData;

        [Group("Charge")]
        [Dropdown(nameof(GetAllEffectID))]
        public string readyMinorEffectId = "SpellFlourishMinor";

        [NonSerialized]
        public EffectData readyMinorEffectData;


        [Group("Charge")]
        [Dropdown(nameof(GetAllEffectID))]
        public string fingerEffectId;
        protected EffectData fingerEffectData;

        [Group("Charge")]
        [Dropdown(nameof(GetAllHandPoseID))]
        public string closeHandPoseId;
        [NonSerialized]
        public HandPoseData closeHandPoseData;

        [Group("Charge")]
        [Dropdown(nameof(GetAllHandPoseID))]
        public string openHandPoseId;
        [NonSerialized]
        public HandPoseData openHandPoseData;

        [Group("Charge")]
        public bool doReadyHaptic = true;
        [Group("Charge")]
        public AnimationCurve chargeEffectCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Group("Charge")]
        public float orbVariationSpeed = 10f;
        [Group("Charge")]
        public float orbVariationAmount = 0.2f;
        [Group("Charge")]
        public float chargeSpeed = 0.5f;
        [Group("Charge")]
        public bool allowStaffBuff = false;
        [Group("Charge")]
        [ShowIf("allowStaffBuff")]
        [ShowInInspector]
        public Dictionary<Modifier, float> heldStaffModifiers;
        [Group("Charge")]
        [LabelText("$" + nameof(ChargeSpeedPerSkillLabel))]
        public float chargeSpeedPerSkill = 0.05f;
        [JsonIgnore]
        public string ChargeSpeedPerSkillLabel => SkillPassiveLabel("Charge Speed", chargeSpeedPerSkill);
        [Group("Charge")]
        public float grabbedFireMaxCharge = 0.5f;
        [Group("Charge")]
        public bool allowUnderwater;
        [Group("Charge")]
        public bool endOnGrip = true;
        [Group("Charge")]
        public bool allowCharge = true;

        public bool ShouldSerializeallowUnderwater()
        {
            return false;
        }

        [Group("Imbue")]
        public float chargeMinHaptic = 0.05f;
        [Group("Imbue")]
        public float chargeMaxHaptic = 0.3f;

        [Group("Hand force")]
        public float handSpringMultiplier = 0.8f;
        [Group("Hand force")]
        public float handLocomotionVelocityCorrectionMultiplier = 1f;

        [Group("Throw")]
        public bool allowThrow = true;
        [Group("Throw")]
        [Dropdown(nameof(GetAllEffectID))]
        [ShowIf("allowThrow")]
        public string throwEffectId;
        [NonSerialized]
        public EffectData throwEffectData;
        [Group("Throw")]
        [ShowIf("allowThrow")]
        public float throwMinCharge = 0.9f;

        [Group("Spray")]
        public bool allowSpray;
        [Group("Spray")]
        [Dropdown(nameof(GetAllHandPoseID))]
        [ShowIf("allowSpray")]
        public string sprayHandPoseId;
        [NonSerialized]
        public HandPoseData sprayHandPoseData;
        [Group("Spray")]
        [ShowIf("allowSpray")]
        public Vector3 sprayMagicOffset;
        [Group("Spray")]
        [ShowIf("allowSpray")]
        public float sprayHeadToFireMaxAngle = 45;
        [Group("Spray")]
        [ShowIf("allowSpray")]
        public float sprayStopMinCharge = 0.2f;
        [Group("Spray")]
        [ShowIf("allowSpray")]
        public float sprayStartMinCharge = 0.5f;

        [Group("Grip Cast")]
        [Dropdown(nameof(GetAllEffectID))]
        public string gripCastEffectId;
        [NonSerialized]
        public EffectData gripCastEffectData;
        [Group("Grip Cast")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string gripCastStatusEffectId;
        [Group("Grip Cast")]
        public float gripCastStatusDuration;
        
        [NonSerialized]
        public bool allowGripCast;

        [NonSerialized]
        protected float lastGripCastTime;
        [NonSerialized]
        public float gripCastDamageInterval;
        [NonSerialized]
        public float gripCastDamageAmount;
        
        [NonSerialized]
        public StatusData gripCastStatusEffect;

        [NonSerialized]
        public bool isGripCasting;

        [NonSerialized]
        public bool isSpraying;

        // [NonSerialized]
        // protected float lastCharge;
        
        [NonSerialized]
        public float currentCharge;

        [Group("Imbue")]
        public bool imbueEnabled = true;
        [Group("Imbue")]
        public bool imbueAllowMetal = false;
        [Group("Imbue")]
        public float imbueRate = 1;
        [Group("Imbue")]
        public float imbueLossMultiplier = 1;
        [Group("Imbue")]
        public float imbueRadius = 0.2f;
        [Group("Imbue")]
        public bool imbueHitUseDamager = false;
        [Group("Imbue")]
        [HideIf("imbueHitUseDamager")]
        public float imbueHitMinVelocity = 4f;
        [Group("Imbue")]
        public float imbueWhooshMinSpeed = 4f;
        [Group("Imbue")]
        public float imbueWhooshMaxSpeed = 12f;
        [Group("Imbue")]
        public float imbueWhooshHapticMultiplier = 1f;

        [Group("Imbue")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueMetalEffectId;
        [NonSerialized]
        public EffectData imbueMetalEffectData;

        [Group("Imbue")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueBladeEffectId;
        [NonSerialized]
        public EffectData imbueBladeEffectData;

        [Group("Imbue")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueCrystalEffectId;
        [NonSerialized]
        public EffectData imbueCrystalEffectData;

        [Group("Charge")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueNaaEffectId;

        [NonSerialized]
        public EffectData imbueNaaEffectData;

        [Group("Imbue")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueTransferEffectId;
        [NonSerialized]
        public EffectData imbueTransferEffectData;

        [Group("Imbue")]
        public float imbueHitEnergySteal = 1;


        [Group("Staff Slam")]
        public float staffSlamRechargeDelay = 2f;
        [Group("Staff Slam")]
        public float staffSlamMinVelocity = 4f;
        [Group("Staff Slam")]
        public float staffSlamConsumptionMult = 1f;
        [Group("Staff Slam")]
        public float staffSlamMaxVelocity = 15f;

        [Group("Staff Slam")]
        [Dropdown(nameof(GetAllEffectID))]
        public string staffSlamTipEffectId;
        [NonSerialized]
        public EffectData staffSlamTipEffectData;

        [Group("Staff Slam")]
        [Dropdown(nameof(GetAllEffectID))]
        public string staffSlamCollisionEffectId;

        [NonSerialized]
        public EffectData staffSlamCollisionEffectData;
        [Group("Staff Slam")]
        public bool allowEnemySelfSlam = false;
        [Group("Musket")]
        [Dropdown(nameof(GetAllEffectID))]
        public string musketHammerEffectId;
        [NonSerialized]
        [JsonIgnore]
        public EffectData musketHammerEffectData;
        [Group("Musket")]
        [Dropdown(nameof(GetAllEffectID))]
        public string musketShootEffectId;
        [NonSerialized]
        [JsonIgnore]
        public EffectData musketShootEffectData;
        [Group("Musket")]
        [Dropdown(nameof(GetAllEffectID))]
        public string musketChargeEffectId;
        [NonSerialized]
        [JsonIgnore]
        public EffectData musketChargeEffectData;



        public new SpellCastCharge Clone()
        {
            return this.MemberwiseClone() as SpellCastCharge;
        }

        public override int GetCurrentVersion()
        {
            return 0;
        }

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            heldStaffModifiers ??= new Dictionary<Modifier, float>
            {
                { Modifier.ChargeSpeed, 1.5f }
            };

            chargeEffectData = Catalog.GetData<EffectData>(chargeEffectId);
            readyEffectData = Catalog.GetData<EffectData>(readyEffectId);
            readyMinorEffectData = Catalog.GetData<EffectData>(readyMinorEffectId);
            fingerEffectData = Catalog.GetData<EffectData>(fingerEffectId);
            gripCastEffectData = Catalog.GetData<EffectData>(gripCastEffectId);
            closeHandPoseData = Catalog.GetData<HandPoseData>(closeHandPoseId);
            openHandPoseData = Catalog.GetData<HandPoseData>(openHandPoseId);
            throwEffectData = Catalog.GetData<EffectData>(throwEffectId);
            sprayHandPoseData = Catalog.GetData<HandPoseData>(sprayHandPoseId);

            imbueMetalEffectData = Catalog.GetData<EffectData>(imbueMetalEffectId);
            imbueBladeEffectData = Catalog.GetData<EffectData>(imbueBladeEffectId);
            imbueTransferEffectData = Catalog.GetData<EffectData>(imbueTransferEffectId);
            imbueCrystalEffectData = Catalog.GetData<EffectData>(imbueCrystalEffectId);
            imbueNaaEffectData = Catalog.GetData<EffectData>(imbueNaaEffectId);
            musketShootEffectData = Catalog.GetData<EffectData>(musketShootEffectId);
            musketHammerEffectData = Catalog.GetData<EffectData>(musketHammerEffectId);
            musketChargeEffectData = Catalog.GetData<EffectData>(musketChargeEffectId);

            staffSlamCollisionEffectData = Catalog.GetData<EffectData>(staffSlamCollisionEffectId);
            staffSlamTipEffectData = Catalog.GetData<EffectData>(staffSlamTipEffectId);
            gripCastStatusEffect = Catalog.GetData<StatusData>(gripCastStatusEffectId);
        }

        public void InvokeOnGripEndEvent()
        {
        }
    }

    public enum Modifier
    {
        Range,
        Duration,
        Intensity,
        Efficiency,
        Speed,
        ChargeSpeed
    }
}

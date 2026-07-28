using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [DeclareBoxGroup("Split/Right/Skill UI", Title = "Skill UI")]
    [DeclareBoxGroup("Split/Right/Charge", Title = "Charge")]
    [DeclareVerticalGroup("Split/Right/Charge/StaffBuffs")]
    [DeclareBoxGroup("Split/Right/Imbue", Title = "Imbue")]
    [DeclareBoxGroup("Split/Right/Hand force", Title = "Hand force")]
    [DeclareBoxGroup("Split/Right/Throw", Title = "Throw")]
    [DeclareBoxGroup("Split/Right/Spray", Title = "Spray")]
    [DeclareBoxGroup("Split/Right/Grip Cast", Title = "Grip Cast")]
    [DeclareBoxGroup("Split/Right/Staff Slam", Title = "Staff Slam")]
    [DeclareBoxGroup("Split/Right/Modules", Title = "Modules")]
#endif
    public class SpellCastCharge : SpellCastData
    {
        [Group("Split/Right/Skill UI")]
        public string castDescription;
        [Group("Split/Right/Skill UI")]
        public string imbueDescription;
        [Group("Split/Right/Skill UI")]
        public string slamDescription;

        [Group("Split/Right/Modules")]
        [SerializeReference]
        public List<SpellModule> modules;

        [Group("Split/Right/Charge")]
        [Dropdown(nameof(GetAllEffectID))]
        public string chargeEffectId;

        [NonSerialized]
        public EffectData chargeEffectData;

        [Group("Split/Right/Charge")]
        [Dropdown(nameof(GetAllEffectID))]
        public string readyEffectId;

        [NonSerialized]
        public EffectData readyEffectData;

        [Group("Split/Right/Charge")]
        [Dropdown(nameof(GetAllEffectID))]
        public string readyMinorEffectId = "SpellFlourishMinor";

        [NonSerialized]
        public EffectData readyMinorEffectData;


        [Group("Split/Right/Charge")]
        [Dropdown(nameof(GetAllEffectID))]
        public string fingerEffectId;
        protected EffectData fingerEffectData;

        [Group("Split/Right/Charge")]
        [Dropdown(nameof(GetAllHandPoseID))]
        public string closeHandPoseId;
        [NonSerialized]
        public HandPoseData closeHandPoseData;

        [Group("Split/Right/Charge")]
        [Dropdown(nameof(GetAllHandPoseID))]
        public string openHandPoseId;
        [NonSerialized]
        public HandPoseData openHandPoseData;

        [Group("Split/Right/Charge")]
        public bool doReadyHaptic = true;
        [Group("Split/Right/Charge")]
        public AnimationCurve chargeEffectCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Group("Split/Right/Charge")]
        public float orbVariationSpeed = 10f;
        [Group("Split/Right/Charge")]
        public float orbVariationAmount = 0.2f;
        [Group("Split/Right/Charge")]
        public float chargeSpeed = 0.5f;
        [Group("Split/Right/Charge/StaffBuffs")]
        public bool allowStaffBuff = false;

        [JsonProperty("heldStaffModifiers")]
        private Dictionary<Modifier, float> heldStaffModifiersProp
        {
            get
            {
                return heldStaffModifiers;
            }
            set
            {
                heldStaffModifiers = new SpellModifierDictionary();
                if (value != null)
                {
                    foreach (KeyValuePair<Modifier, float> pair in value)
                    {
                        heldStaffModifiers[pair.Key] = pair.Value;
                    }
                }
            }
        }

        [JsonIgnore]
        [ShowInInspector]
        [Group("Split/Right/Charge/StaffBuffs")]
        [ShowIf("allowStaffBuff")]
        [LabelWidth(0.0001f)]
        public SpellModifierDictionary heldStaffModifiers;
        [Group("Split/Right/Charge")]
        [LabelText("$" + nameof(ChargeSpeedPerSkillLabel))]
        public float chargeSpeedPerSkill = 0.05f;
        [JsonIgnore]
        public string ChargeSpeedPerSkillLabel => SkillPassiveLabel("Charge Speed", chargeSpeedPerSkill);
        [Group("Split/Right/Charge")]
        public float grabbedFireMaxCharge = 0.5f;
        [Group("Split/Right/Charge")]
        public bool allowUnderwater;
        [Group("Split/Right/Charge")]
        public bool endOnGrip = true;
        [Group("Split/Right/Charge")]
        public bool allowCharge = true;

        public bool ShouldSerializeallowUnderwater()
        {
            return false;
        }

        [Group("Split/Right/Imbue")]
        public float chargeMinHaptic = 0.05f;
        [Group("Split/Right/Imbue")]
        public float chargeMaxHaptic = 0.3f;

        [Group("Split/Right/Hand force")]
        public float handSpringMultiplier = 0.8f;
        [Group("Split/Right/Hand force")]
        public float handLocomotionVelocityCorrectionMultiplier = 1f;

        [Group("Split/Right/Throw")]
        public bool allowThrow = true;
        [Group("Split/Right/Throw")]
        [Dropdown(nameof(GetAllEffectID))]
        [ShowIf("allowThrow")]
        public string throwEffectId;
        [NonSerialized]
        public EffectData throwEffectData;
        [Group("Split/Right/Throw")]
        [ShowIf("allowThrow")]
        public float throwMinCharge = 0.9f;

        [Group("Split/Right/Spray")]
        public bool allowSpray;
        [Group("Split/Right/Spray")]
        [Dropdown(nameof(GetAllHandPoseID))]
        [ShowIf("allowSpray")]
        public string sprayHandPoseId;
        [NonSerialized]
        public HandPoseData sprayHandPoseData;
        [Group("Split/Right/Spray")]
        [ShowIf("allowSpray")]
        public Vector3 sprayMagicOffset;
        [Group("Split/Right/Spray")]
        [ShowIf("allowSpray")]
        public float sprayHeadToFireMaxAngle = 45;
        [Group("Split/Right/Spray")]
        [ShowIf("allowSpray")]
        public float sprayStopMinCharge = 0.2f;
        [Group("Split/Right/Spray")]
        [ShowIf("allowSpray")]
        public float sprayStartMinCharge = 0.5f;

        [Group("Split/Right/Grip Cast")]
        [Dropdown(nameof(GetAllEffectID))]
        public string gripCastEffectId;
        [NonSerialized]
        public EffectData gripCastEffectData;
        [Group("Split/Right/Grip Cast")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string gripCastStatusEffectId;
        [Group("Split/Right/Grip Cast")]
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

        [Group("Split/Right/Imbue")]
        public bool imbueEnabled = true;
        [Group("Split/Right/Imbue")]
        public bool imbueAllowMetal = false;
        [Group("Split/Right/Imbue")]
        public float imbueRate = 1;
        [Group("Split/Right/Imbue")]
        public float imbueLossMultiplier = 1;
        [Group("Split/Right/Imbue")]
        public float imbueRadius = 0.2f;
        [Group("Split/Right/Imbue")]
        public bool imbueHitUseDamager = false;
        [Group("Split/Right/Imbue")]
        [HideIf("imbueHitUseDamager")]
        public float imbueHitMinVelocity = 4f;
        [Group("Split/Right/Imbue")]
        public float imbueWhooshMinSpeed = 4f;
        [Group("Split/Right/Imbue")]
        public float imbueWhooshMaxSpeed = 12f;
        [Group("Split/Right/Imbue")]
        public float imbueWhooshHapticMultiplier = 1f;

        [Group("Split/Right/Imbue")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueMetalEffectId;
        [NonSerialized]
        public EffectData imbueMetalEffectData;

        [Group("Split/Right/Imbue")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueBladeEffectId;
        [NonSerialized]
        public EffectData imbueBladeEffectData;

        [Group("Split/Right/Imbue")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueCrystalEffectId;
        [NonSerialized]
        public EffectData imbueCrystalEffectData;

        [Group("Split/Right/Charge")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueNaaEffectId;

        [NonSerialized]
        public EffectData imbueNaaEffectData;

        [Group("Split/Right/Imbue")]
        [Dropdown(nameof(GetAllEffectID))]
        public string imbueTransferEffectId;
        [NonSerialized]
        public EffectData imbueTransferEffectData;

        [Group("Split/Right/Imbue")]
        public float imbueHitEnergySteal = 1;


        [Group("Split/Right/Staff Slam")]
        public float staffSlamRechargeDelay = 2f;
        [Group("Split/Right/Staff Slam")]
        public float staffSlamMinVelocity = 4f;
        [Group("Split/Right/Staff Slam")]
        public float staffSlamConsumptionMult = 1f;
        [Group("Split/Right/Staff Slam")]
        public float staffSlamMaxVelocity = 15f;

        [Group("Split/Right/Staff Slam")]
        [Dropdown(nameof(GetAllEffectID))]
        public string staffSlamTipEffectId;
        [NonSerialized]
        public EffectData staffSlamTipEffectData;

        [Group("Split/Right/Staff Slam")]
        [Dropdown(nameof(GetAllEffectID))]
        public string staffSlamCollisionEffectId;

        [NonSerialized]
        public EffectData staffSlamCollisionEffectData;
        [Group("Split/Right/Staff Slam")]
        public bool allowEnemySelfSlam = false;


        [Serializable]
        public class SpellModifierDictionary : UnitySerializedDictionary<Modifier, float> { }


        public new SpellCastCharge Clone()
        {
            if (MemberwiseClone() is not SpellCastCharge spell)
            {
                Exception exception = new Exception($"Failed to clone spell ({id})");
                Debug.LogException(exception);
                throw exception;
            }
            spell.modules = new List<SpellModule>();
            if (!modules.IsNullOrEmpty())
            {
                //copy the new modules 
                for (var i = 0; i < modules.Count; i++)
                {
                    var module = modules[i].Clone();
                    spell.modules.Add(module);
                }
            }
            return spell;
        }

        public override int GetCurrentVersion()
        {
            return 0;
        }

        public bool TryGetSpellModule<T>(out T module) where T : SpellModule
        {
            module = default;
            if (modules == null) return false;
            for (var i = 0; i < modules.Count; i++)
            {
                if (modules[i] is not T foundModule) continue;
                module = foundModule;
                return true;
            }
            return false;
        }

        public int TryGetSpellModules<T>(out List<T> outModules) where T : SpellModule
        {
            outModules = new List<T>();
            if (modules == null) return 0;
            for (var i = 0; i < modules.Count; i++)
            {
                if (modules[i] is not T module) continue;
                outModules.Add(module);
            }

            return outModules.Count;
        }

        public int ForEachModule<T>(Action<T> action) where T: SpellModule
        {
            if (modules == null) return 0;
            int count = 0;
            for (var i = 0; i < modules.Count; i++)
            {
                if (modules[i] is not T foundModule) continue;
                action?.Invoke(foundModule);
                count++;
            }

            return count;
        }

        public bool HasModule<T>() where T : SpellModule => TryGetSpellModule<T>(out _);
        
        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            if (heldStaffModifiers == null)
            {
                heldStaffModifiers = new SpellModifierDictionary()
                {
                    { Modifier.ChargeSpeed, 1.5f }
                };
            }

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

            staffSlamCollisionEffectData = Catalog.GetData<EffectData>(staffSlamCollisionEffectId);
            staffSlamTipEffectData = Catalog.GetData<EffectData>(staffSlamTipEffectId);
            gripCastStatusEffect = Catalog.GetData<StatusData>(gripCastStatusEffectId);

            if (Application.isPlaying)
            {
                if (!HasModule<SpellModuleMusket>() && needsCompatibilityModule)
                {
                    SpellModuleMusketCompatibility compatibilityModule = new SpellModuleMusketCompatibility();
                    compatibilityModule.SetIdsFromSpell(this);
                    modules ??= new List<SpellModule>();
                    modules.Add(compatibilityModule);
                }

            }

            ForEachModule<SpellModule>(module =>
            {
                try
                {
                    module.OnCatalogRefresh();
                }
                catch (Exception e)
                {
                    Debug.LogError("Caught following exception during module.OnCatalogRefresh()");
                    Debug.LogException(e);
                }
            });
        }

        public override IEnumerator OnCatalogRefreshCoroutine()
        {
            yield return base.OnCatalogRefreshCoroutine();
            if (modules == null) yield break;
            for (var i = 0; i < modules.Count; i++)
            {
                yield return modules[i].OnCatalogRefreshCoroutine().WrapSafely(e =>
                {
                    Debug.LogError("Caught following exception during module.OnCatalogRefreshCoroutine()");
                    Debug.LogException(e);
                }); 
            }
        }

 // ProjectCore

        #region Backwards compatibility
        private bool needsCompatibilityModule => !musketHammerEffectId.IsNullOrEmptyOrWhitespace()
            || !musketShootEffectId.IsNullOrEmptyOrWhitespace()
            || !musketChargeEffectId.IsNullOrEmptyOrWhitespace();
        [Obsolete("Musket fields in SpellCastCharge and similar are obsolete. Reference fields inside SpellModuleMusket instead.")]
        [HideInInspector]
        public string musketHammerEffectId;
        [NonSerialized]
        [JsonIgnore]
        [Obsolete("Musket fields in SpellCastCharge and similar are obsolete. Reference fields inside SpellModuleMusket instead.")]
        public EffectData musketHammerEffectData;
        private bool ShouldSerializemusketHammerEffectId() => false;
        [Obsolete("Musket fields in SpellCastCharge and similar are obsolete. Reference fields inside SpellModuleMusket instead.")]
        [HideInInspector]
        public string musketShootEffectId;
        [NonSerialized]
        [JsonIgnore]
        [Obsolete("Musket fields in SpellCastCharge and similar are obsolete. Reference fields inside SpellModuleMusket instead.")]
        public EffectData musketShootEffectData;
        private bool ShouldSerializemusketShootEffectId() => false;
        [Obsolete("Musket fields in SpellCastCharge and similar are obsolete. Reference fields inside SpellModuleMusket instead.")]
        [HideInInspector]
        public string musketChargeEffectId;
        [NonSerialized]
        [JsonIgnore]
        [Obsolete("Musket fields in SpellCastCharge and similar are obsolete. Reference fields inside SpellModuleMusket instead.")]
        public EffectData musketChargeEffectData;
        private bool ShouldSerializemusketChargeEffectId() => false;

        [Obsolete("OnMusketShoot is obsolete. Create a module that inherits from SpellModuleMusket for your spell.")]
        public virtual void OnMusketShoot(bool playEffect = true) { }
        #endregion

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

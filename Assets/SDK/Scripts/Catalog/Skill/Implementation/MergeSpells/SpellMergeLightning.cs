using System;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad.Skill.Spell;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


using TriInspector;

namespace ThunderRoad.Skill.SpellMerge
{
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Beam", Title = "Beam")]
    [DeclareBoxGroup("Chaining", Title = "Chaining")]
    [DeclareBoxGroup("Electrocution", Title = "Electrocution")]
    [DeclareBoxGroup("Hook", Title = "Hook")]
    [DeclareBoxGroup("Charge", Title = "Charge")]
    public class SpellMergeLightning : SpellMergeData
    {
        [Group("General")]
        public float movementSpeedMult = 0.8f;

        [Group("Beam")]
        [Dropdown(nameof(GetAllEffectID))]
        public string beamEffectId;
        [NonSerialized]
        protected EffectData beamEffectData;


        [Group("Beam")]
        public LayerMask beamMask;

        [Group("Beam")]
        public float beamForce = 3f;

        [Group("Beam")]
        [Dropdown(nameof(GetAllSpellID))]
        public string imbueSpellId;
        protected SpellCastCharge imbueSpell;

        [Group("Beam")]
        public float imbueAmount = 10;

        [Group("Beam")]
        public float damageDelay = 0.5f;

        [Group("Beam")]
        public float damageAmount = 10f;

        [Group("Beam")]
        public AnimationCurve beamForceCurve = new(new Keyframe(0f, 10f), new Keyframe(0.05f, 25f), new Keyframe(0.1f, 10f));

        [Group("Beam")]
        public float beamHandPositionSpringMultiplier = 1f;

        [Group("Beam")]
        public float beamHandPositionDamperMultiplier = 1f;

        [Group("Beam")]
        public float beamHandRotationSpringMultiplier = 0.2f;

        [Group("Beam")]
        public float beamHandRotationDamperMultiplier = 0.6f;

        [Group("Beam")]
        public float beamHandLocomotionVelocityCorrectionMultiplier = 1;

        [Group("Beam")]
        public float beamLocomotionPushForce = 4f;

        [Group("Beam")]
        public float beamCastMinHandAngle = 20f;

        [Group("Beam")]
        [Dropdown(nameof(GetAllEffectID))]
        public string beamImpactEffectId;
        protected EffectData beamImpactEffectData;

        [Group("Beam")]
        public bool instantBreakBreakables = true;

        [Group("Chaining")]
        public float chainRadius = 4;

        [Group("Chaining")]
        public float chainDelay = 1f;

        [Group("Chaining")]
        [Dropdown(nameof(GetAllEffectID))]
        public string chainEffectId;
        protected EffectData chainEffectData;

        [Group("Electrocution")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusEffectId = "Electrocute";
        [NonSerialized]
        public StatusData statusData;

        [NonSerialized]
        public bool beamActive;
        [NonSerialized]
        public Ray beamRay;
        [NonSerialized]
        public Transform beamStart;
        [NonSerialized]
        public Transform beamHitPoint;
        protected float lastDamageTick;
        protected float lastChainTick;
        protected Collider[] collidersHit;
        protected HashSet<Creature> creaturesHit;
        [Group("Hook")]
        public float beamHookDamper = 150;
        [Group("Hook")]
        public float beamHookSpring = 1000;
        [Group("Hook")]
        public float beamHookSpeed = 20;
        [Group("Hook")]
        public float beamHookMaxAngle = 30;
        [Group("Charge")]
        public float zapInterval = 0.7f;

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            beamEffectData = Catalog.GetData<EffectData>(beamEffectId);
            imbueSpell = Catalog.GetData<SpellCastCharge>(imbueSpellId);
            chainEffectData = Catalog.GetData<EffectData>(chainEffectId);
            statusData = Catalog.GetData<StatusData>(statusEffectId);
            beamImpactEffectData = Catalog.GetData<EffectData>(beamImpactEffectId);
            collidersHit = new Collider[20];
            beamForceCurve.postWrapMode = WrapMode.Loop;
            creaturesHit = new HashSet<Creature>();
        }

    }
}

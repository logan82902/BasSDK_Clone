using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad.Skill.Spell;

using TriInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ThunderRoad.Skill.SpellMerge
{
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Beam", Title = "Beam")]
    [DeclareBoxGroup("Trail", Title = "Trail")]
    [DeclareBoxGroup("Status", Title = "Status")]
    public class SpellMergeMoltenBeam : SpellMergeData
    {
        [Group("General")]
        public float movementSpeedMult = 0.8f;

        [Group("Beam")]
        [Dropdown(nameof(GetAllEffectID))]
        public string beamEffectId;
        [NonSerialized]
        protected EffectData beamEffectData;

        [Group("Beam")]
        public float beamHitHeatTransferPerSecond = 70;

        [Group("Beam")]
        public LayerMask layerMask;

        [Group("Trail")]
        [Dropdown(nameof(GetAllEffectID))]
        public string trailEffectId;

        [NonSerialized]
        public EffectData trailEffectData;


        [Group("Trail")]
        public float trailMinDelay = 0.4f;

        [Group("Trail")]
        public float trailMaxDelay = 4f;

        [Group("Trail")]
        public float trailMinDistance = 1f;

        [Group("Trail")]
        public float trailMaxDistance = 1.5f;

        [Group("Trail")]
        public float trailDuration = 5;

        [Group("Trail")]
        public float trailRadius = 2;

        [Group("Trail")]
        public AnimationCurve trailIntensityCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Group("Trail")]
        [MinMaxSlider(0, 5, showFields: true)]
        public Vector2Int minMaxFireSpark = new(1, 4);


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
        public bool instantBreakBreakables = true;

        [Group("Status")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusEffectId = "Burning";
        [NonSerialized]
        public StatusData statusData;

        [Group("Status")]
        public float statusDuration = 5;

        [Group("Status")]
        public float statusHeatTransfer = 100;
    }
}
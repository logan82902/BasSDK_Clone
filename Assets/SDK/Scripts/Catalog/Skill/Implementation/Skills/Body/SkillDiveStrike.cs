using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.Skill
{
    [DeclareBoxGroup("Detection", Title = "Detection")]
    [DeclareBoxGroup("Dive", Title = "Dive")]
    [DeclareBoxGroup("Misc", Title = "Misc")]
    public class SkillDiveStrike : SkillData
    {
        public delegate void DiveEvent(Creature creature, EventTime eventTime);
        public static event DiveEvent OnDive;

        [Header("General")]
        [Group("Detection")]
        public float downwardAngle = 5f;

        [Group("Detection")]
        public float minimumVelocity = 3f;

        [Group("Detection")]
        public float grabWait = 0.5f;

        [Group("Detection")]
        public float headAngleRange = 15f;

        [Header("Jab")]
        [Group("Detection")]
        public float decelerationRatioThresholdStab = 0.1f;

        [Group("Detection")]
        public bool allowPommelDive = false;
        [Header("Swing")]
        [Group("Detection")]
        public bool swingDetectionEnabled = false;
        [Group("Detection")]
        public float decelerationRatioThresholdSlash = 0.1f;
        [Group("Detection")]
        public float maxSwingPointDistance = 0.5f;

        [Group("Detection")]
        public float minAirborneTimeForPunch = 0.6f;

        [Group("Detection")]
        public float minAirborneTimeForSlash = 0.5f;

        [Group("Detection")]
        public float slashLookBiasAmount = 0.75f;

        [Group("Dive")]
        public bool aimAssist = false;

        [Group("Dive")]
        [Dropdown(nameof(GetAllEffectID))]
        public string effectId;

        protected EffectData effectData;
        [Group("Dive")]
        public float diveSpeedScaleAimAssist = 1.4f;
        [Group("Dive")]
        public float currentSpeedScale = 0.75f;
        [Group("Dive")]
        public float diveSpeedScale = 2f;
        [Group("Dive")]
        public float handVelocityMult = 2;
        [Group("Dive")]
        public float maxVelocity = 8f;
        [Group("Dive")]
        public float damageMultiplier = 3f;
        [Group("Dive")]
        public float groundEndDiveAllowance = 0.5f;
        [Group("Misc")]
        public bool endOnHit = false;
        [Group("Misc")]
        public float fallDamageScale = 0;

    }

    public enum DiveType
    {
        None,
        Hand,
        Swing,
        Jab,
        FlyDirRef
    }

}

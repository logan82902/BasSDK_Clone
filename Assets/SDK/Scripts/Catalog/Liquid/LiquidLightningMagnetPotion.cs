using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad.Skill.Spell;

using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [Serializable]
    public class LiquidLightningMagnetPotion : LiquidData
    {
        protected const string attractorObjectVarName = "LightningAttractor";

        public string lightningBodyEffectId;
        public string lightningZapEffectId;
        public bool imbue = true;
        public string lightningSpellId;
        public float imbueAmountPerZap = 33f;
        public float zapJoltVelocity = 0.2f;
        public bool disarmNpcs = true;
        public float disarmChance = 0.5f;
        public bool pullNpcOnDisarm = true;
        public float pullDisarmForce = 5f;
        public bool pullArmoredNpcs = true;
        public float ragdollArmoredChance = 0.5f;
        public string electrocuteStatusId;
        public string magnetRadiusEffectId;
        public AnimationCurve itemMagnetForceByDistance;
        public AnimationCurve creatureMagnetForceByDistance;
        public ForceMode magnetForceMode = ForceMode.Acceleration;
        public AnimationCurve repulsionForceByDistance;
        public ForceMode repulusionForceMode = ForceMode.VelocityChange;
        public bool resistiveForce = true;
        public float resistiveForceMinSpeed = 5f;
        public float resistiveForceAcceleration = 5f;
        public float endEjectThrowSpeed = 15f;
        public float inheritedSourceVelocity = 0.9f;
        public bool throwOnEject = true;
        public bool zapOnEject = true;
        public Vector2 minMaxZapFequency = new Vector2(0.6f, 1.2f);
        public AnimationCurve pullRadiusByConsumptionAmount = AnimationCurve.Linear(40f, 2.5f, 100f, 5f);
        public float consumptionPerDrink = 20f;
        public float consumptionDecayPerSecond = 10f;
        public float decayDelay = 1f;

        public class MagnetRadiusIndicator : LiquidData.Indicator
        {
            public override string GetName() => "(Lightning Potion) Magnet Radius Display";

        }

    }

}

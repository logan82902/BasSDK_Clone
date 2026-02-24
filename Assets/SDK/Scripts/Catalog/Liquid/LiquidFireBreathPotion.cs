using UnityEngine;
using System;
using ThunderRoad.Skill.Spell;
using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [Serializable]
    public class LiquidFireBreathPotion : LiquidData
    {
        protected const string headEffectName = "LingeringHeadEffect";
        protected const string flamethrowerStartTimeVarName = "FlamethrowerStartTime";
        protected const string flamethrowerDurationVarName = "FlamethrowerDuration";

        public string heatStatusId;
        public float heatPerReception = 10f;
        public float baseDuration = 3f;
        public float durationPerReception = 3f;
        public float activationDelay = 1f;
        public string flamethrowerSkillId;
        public float mouthOpenRatio = 1f;
        public string lingeringEffectId;
        public float headEffectExtraDuration = 6f;

        public class ChargePercentIndicator : LiquidData.Indicator
        {
            public override string GetName() => "(Fire Potion) Fire Breath Charge";

        }

    }
}

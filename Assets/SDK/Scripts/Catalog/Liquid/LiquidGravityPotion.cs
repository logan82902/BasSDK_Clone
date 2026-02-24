using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThunderRoad
{
    [Serializable]
    public class LiquidGravityPotion : LiquidData
    {
        protected const string effectVarName = "FloatingEffect";
        protected const string floatStartTimeVarName = "GravityRumFloatStart";

        public float floatingTimePerReception = 2f;
        public float gravityModifier = 0f;
        public float dragModifier = 1.5f;
        public string effectId;
        public string loopingSoundEffectId;
        public int countdownSoundCount = 5;
        public float countdownSoundDelay = 1f;
        public string countdownSoundEffectId;
        public float swimmingForce = -1f;

        public class FloatingTimeIndicator : LiquidData.Indicator
        {
            public override string GetName() => "(Gravity Potion) Floating Time Meter";

        }

    }
}

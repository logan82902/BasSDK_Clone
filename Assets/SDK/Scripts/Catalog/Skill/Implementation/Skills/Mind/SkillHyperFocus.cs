using System.Collections;
using System.Collections.Generic;
using Shadowood;
using ThunderRoad.Skill.SpellPower;
using UnityEngine;
using TriInspector;
using System;
using Newtonsoft.Json;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Time Scale", Title = "Time Scale")]
    [DeclareBoxGroup("Focus Consumption", Title = "Focus Consumption")]
    [DeclareBoxGroup("Activation", Title = "Activation")]
    [DeclareBoxGroup("Multipliers", Title = "Multipliers")]
    [DeclareBoxGroup("Effects", Title = "Effects")]
    public class SkillHyperFocus : SkillData
    {
        [Group("Time Scale")]
        public float timeScale = 0.1f;
        [Group("Time Scale")]
        public float timeScaleAndroid = 0.15f;

        [JsonIgnore]
        public float TimeScale => Common.IsAndroid ? timeScaleAndroid : timeScale;

        [Group("Focus Consumption")]
        public float focusConsumptionStart = 10f;
        [Group("Focus Consumption")]
        public float focusConsumption = 5f;
        [Group("Focus Consumption")]
        public float minFocus = 20f;
        [Group("Activation")]
        public float buttonHoldThreshold = 0.3f;
        [Group("Multipliers")]
        public float jointPosMultiplier = 10;
        [Group("Multipliers")]
        public float jointRotMultiplier = 10;
        [Group("Multipliers")]
        public float speedMultiplier = 10;
        [Group("Multipliers")]
        public float dragMult = 5f;
        [Group("Multipliers")]
        public float maxSpeedCap = 20;
        [Group("Multipliers")]
        public float mixerMultiplier = 1;
        [Group("Multipliers")]
        public float chargeSpeedMultiplier = 3f;

        [Group("Effects")]
        [Dropdown(nameof(GetAllEffectID))]
        public string effectId;

        [NonSerialized]
        [JsonIgnore]
        public EffectData effectData;

        [Group("Effects")]
        [Dropdown(nameof(GetAllEffectID))]
        public string handEffectId;
        protected EffectData handEffectData;

        [NonSerialized]
        [JsonIgnore]
        public HashSet<object> handlers;

    }
}

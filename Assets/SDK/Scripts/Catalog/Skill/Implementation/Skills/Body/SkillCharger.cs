using System.Collections.Generic;
using UnityEngine;

using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad.Skill
{
    [DeclareBoxGroup("Force", Title = "Force")]
    [DeclareBoxGroup("Footstep", Title = "Footstep")]
    [DeclareBoxGroup("Player Modifiers", Title = "Player Modifiers")]
    public class SkillCharger : SkillData
    {
        [Group("Force")]
        public float sprintDelay = 0.5f;

        [Group("Force")]
        public float force = 1f;

        [Group("Force")]
        [Dropdown(nameof(GetAllEffectID))]
        public string linesEffectId;
        [JsonIgnore]
        public EffectData linesEffectData;

        [Group("Force")]
        [Dropdown(nameof(GetAllEffectID))]
        public string shoveEffectId;
        [JsonIgnore]
        public EffectData shoveEffectData;

        [Group("Footstep")]
        [Dropdown(nameof(GetAllEffectID))]
        public string stepEffectId;
        [JsonIgnore]
        protected EffectData stepEffectData;

        [Group("Player Modifiers")]
        public float damageReduction = 0.3f;

        [Group("Player Modifiers")]
        public float speedMultiplier = 1.3f;

    }

}
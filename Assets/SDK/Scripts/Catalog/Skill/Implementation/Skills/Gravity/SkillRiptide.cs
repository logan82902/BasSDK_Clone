using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Grab", Title = "Grab")]
    [DeclareBoxGroup("Throw", Title = "Throw")]
    [DeclareBoxGroup("Status", Title = "Status")]
    public class SkillRiptide : SpellSkillData
    {
        [Group("Grab")]
        public float initialDelay = 0.1f;

        [Group("Grab")]
        public bool includeItems = false;

        [Group("Grab")]
        public float gripWindow = 0.5f;

        [Group("Grab")]
        [Dropdown(nameof(GetAllEffectID))]
        public string tetherEffectId;

        protected EffectData tetherEffectData;

        [Group("Grab")]
        [Dropdown(nameof(GetAllEffectID))]
        public string grabEffectId;

        protected EffectData grabEffectData;

        [Group("Throw")]
        public float throwMultiplier = 3f;

        [Group("Throw")]
        [Dropdown(nameof(GetAllEffectID))]
        public string throwEffectId;

        protected EffectData throwEffectData;

        [Group("Status")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusId = "Floating";

        protected StatusData statusData;

    }
}
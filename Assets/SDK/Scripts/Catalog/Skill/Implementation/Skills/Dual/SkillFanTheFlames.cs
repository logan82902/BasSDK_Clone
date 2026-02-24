using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Effect", Title = "Effect")]
    [DeclareBoxGroup("Status Effect", Title = "Status Effect")]
    public class SkillFanTheFlames : SpellSkillData
    {
        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string effectId;
        [Group("Status Effect")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusEffectId = "Burning";
        [Group("Status Effect")]
        public float duration = 3f;
        [Group("Status Effect")]
        public float heatTransferRatio = 0.4f;
        [Group("Status Effect")]
        public float radius = 3f;
        [NonSerialized]
        public EffectData effectData;
        protected StatusData status;

    }
}

using System;
using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Conditions", Title = "Conditions")]
    [DeclareBoxGroup("Ignition", Title = "Ignition")]
    public class SkillRevengeIgnition : SkillData
    {
        public static bool loaded = false;

        [Group("Conditions")]
        public bool ignoreMagic = true;
        [Group("Conditions")]
        public bool meleeOnly = true;
        [Group("Conditions")]
        public float triggerMinDamage = 0f;
        [Group("Ignition")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string burnEffectID = "Burning";

        public StatusData burnEffect;
        [Group("Ignition")]
        public float burnTime = 3f;
        [Group("Ignition")]
        public float heatAmount = 20f;
        [Group("Ignition")]
        public float burstDamage = 5f;
        [Group("Ignition")]
        [Dropdown(nameof(GetAllEffectID))]
        public string burstEffectID;

        [NonSerialized]
        public EffectData burstEffectData;
    }
}

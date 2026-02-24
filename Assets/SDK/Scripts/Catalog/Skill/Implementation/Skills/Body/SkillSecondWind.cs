using System;
using System.Collections;
using UnityEngine;
using ThunderRoad.Skill.SpellPower;

using TriInspector;

namespace ThunderRoad.Skill
{
    [DeclareBoxGroup("Charges", Title = "Charges")]
    [DeclareBoxGroup("Healing", Title = "Healing")]
    [DeclareBoxGroup("Effect", Title = "Effect")]
    [DeclareBoxGroup("Knockback", Title = "Knockback")]
    [DeclareBoxGroup("Rage", Title = "Rage")]
    public class SkillSecondWind : SkillData
    {
        [Group("Charges")]
        public int chargesPerLevel = 1;

        [Group("Healing")]
        public float healRatio = 0.5f;

        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string startEffectId;

        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string loopEffectId;

        [Group("Knockback")]
        public float knockbackUpwardsForce = 1;
        [Group("Knockback")]
        public float knockbackRadius = 4;
        [Group("Knockback")]
        public float knockbackForce = 10;

        [Group("Rage")]
        public float rageDamageReduction = 0.1f;

        [Group("Rage")]
        public float rageDuration = 10f;

        [Group("Rage")]
        public float rageStrengthMultiplier = 10;

        [Group("Rage")]
        public float rageSpeedMultiplier = 1.3f;

        public delegate void SecondWindEvent(SkillSecondWind skill, EventTime time);
        public event SecondWindEvent OnSecondWindEvent;

    }
}

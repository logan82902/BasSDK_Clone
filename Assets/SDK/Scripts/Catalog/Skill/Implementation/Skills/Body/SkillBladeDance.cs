using System;
using UnityEngine;

using TriInspector;

namespace ThunderRoad.Skill
{
    [DeclareBoxGroup("Effect", Title = "Effect")]
    [DeclareBoxGroup("Momentum", Title = "Momentum")]
    [DeclareBoxGroup("Modifiers", Title = "Modifiers")]
    public class SkillBladeDance : SpellSkillData
    {
        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string swingBladeEffectId;

        [NonSerialized]
        public EffectData swingBladeEffectData;

        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string swingBluntEffectId;

        [NonSerialized]
        public EffectData swingBluntEffectData;

        [Group("Momentum")]
        public float minVelocity = 2.5f;
        [Group("Momentum")]
        public float swingMomentumGain = 2;
        [Group("Momentum")]
        public float lingerDelay = 0.5f;
        [Group("Momentum")]
        public float velocityOffset = 0.3f;

        [Group("Modifiers")]
        [MinMaxSlider(0, 3, true)]
        public Vector2 sliceAngle = new(0, 2.5f);
        [Group("Modifiers")]
        public float force = 1;
        [Group("Modifiers")]
        public int pushLevel = 2;

    }

}

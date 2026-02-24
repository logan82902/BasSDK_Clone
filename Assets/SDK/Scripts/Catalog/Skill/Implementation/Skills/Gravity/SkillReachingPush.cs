using UnityEngine;
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Spell Modifiers", Title = "Spell Modifiers")]
    public class SkillReachingPush : SpellSkillData
    {
        [Group("Spell Modifiers")]
        public float rangeMultiplier = 2;
        [Group("Spell Modifiers")]
        public float forceMultiplier = 1.5f;

        public string effectId = "SpellGravityPushReach";
        public EffectData effectData;

    }
}

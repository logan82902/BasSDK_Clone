using UnityEngine;

using TriInspector;

namespace ThunderRoad.Skill
{
    [DeclareBoxGroup("Kick", Title = "Kick")]
    [DeclareBoxGroup("Force", Title = "Force")]
    public class SkillMightyKick : SkillData
    {
        [Group("Kick")]
        [Dropdown(nameof(GetAllEffectID))]
        public string kickEffectId;

        [Group("Force")]
        public float forceMultiplierRagdolls = 1.3f;

        [Group("Force")]
        public float forceMultiplierItems = 3f;

    }
}
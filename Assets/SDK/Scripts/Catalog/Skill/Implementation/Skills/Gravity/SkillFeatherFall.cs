using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Hover", Title = "Hover")]
    public class SkillFeatherFall : SpellSkillData
    {
        [Group("Hover")]
        public float hoverVelocityThreshold = -0.5f;
        [Group("Hover")]
        public float hoverForce = 0.46f;
        [Group("Hover")]
        [Dropdown(nameof(GetAllEffectID))]
        public string hoverEffectId;
        [Group("Hover")]
        public float groundEffectDistance = 3f;
        [Group("Hover")]
        public float groundEffectMinDistance = 1.2f;
        [Group("Hover")]
        protected EffectData hoverEffectData;

    }
}

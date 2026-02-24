using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Jump", Title = "Jump")]
    public class SkillAirDash : SkillData
    {
        [Group("Jump")]
        [Dropdown(nameof(GetAllEffectID))]
        public string effectId;
        protected EffectData effectData;
        [Group("Jump")]
        public float dashForce = 10;

    }
}

using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Bolt", Title = "Bolt")]
    public class SkillThunderPunch : SkillSpellPunch
    {
        [Group("Bolt")]
        [Dropdown(nameof(GetAllEffectID))]
        public string boltEffectId;

        protected EffectData boltEffect;

        [Group("Bolt")]
        public float boltRadius = 3;

        [Group("Bolt")]
        public float damageShareRatio = 0.5f;
    }
}
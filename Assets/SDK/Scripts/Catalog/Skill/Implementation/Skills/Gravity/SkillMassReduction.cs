using System.Collections.Generic;
using TriInspector;


namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Mass Reduction", Title = "Mass Reduction")]
    [DeclareBoxGroup("Joint Modifier", Title = "Joint Modifier")]
    public class SkillMassReduction : SpellSkillData
    {
        [Group("Mass Reduction")]
        public float massMultiplier = 0.25f;
        [Group("Joint Modifier")]
        public float positionSpring = 1.5f;
        [Group("Joint Modifier")]
        public float positionDamper = 1.5f;
        [Group("Joint Modifier")]
        public float rotationSpring = 1.5f;
        [Group("Joint Modifier")]
        public float rotationDamper = 1.5f;
    }
}

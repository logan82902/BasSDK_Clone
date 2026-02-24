using System.Collections.Generic;

using UnityEngine;
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Bolts", Title = "Bolts")]
    public class SkillStaticDisarm : SpellSkillData
    {
        [Group("Bolts")]
        public float disarmChanceNPC = 0.8f;
        [Group("Bolts")]
        public float disarmChancePlayer = 0.3f;

    }
}

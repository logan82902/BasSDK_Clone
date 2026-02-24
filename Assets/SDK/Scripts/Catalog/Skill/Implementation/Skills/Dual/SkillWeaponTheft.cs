using System.Collections;
using TriInspector;
using ThunderRoad.Skill.SpellPower;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Steal", Title = "Steal")]
    [DeclareBoxGroup("Bolts", Title = "Bolts")]
    public class SkillWeaponTheft : SkillData
    {
        [Group("Steal")]
        public float stealDelay;

        [Group("Bolts")]
        public Vector2 boltDelay = new(0.5f, 0.1f);

        [Group("Bolts")]
        [Dropdown(nameof(GetAllEffectID))]
        public string boltEffectId;
        protected EffectData boltEffectData;

    }
}
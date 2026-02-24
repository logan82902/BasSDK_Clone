using System;
using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Imbue", Title = "Imbue")]
    [DeclareBoxGroup("Bolts", Title = "Bolts")]
    [DeclareBoxGroup("Status Effect", Title = "Status Effect")]
    public class SkillSapStatusApplier : SpellSkillData
    {
        [Group("Imbue")]
        [Dropdown(nameof(GetAllSpellID))]
        public string spellId;

        [Group("Imbue")]
        public Gradient sapGradient;

        [Group("Bolts")]
        public Gradient boltGradient;

        [Group("Status Effect")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusEffectId;

        [NonSerialized]
        public StatusData statusData;

        [Group("Status Effect")]
        public float statusDuration = 2f;

        protected int spellHashId;
       
    }
}
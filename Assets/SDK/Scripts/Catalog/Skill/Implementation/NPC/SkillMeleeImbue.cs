using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace ThunderRoad.Skill
{
    [DeclareBoxGroup("Imbue", Title = "Imbue")]
    [DeclareBoxGroup("Spell", Title = "Spell")]
    public class SkillMeleeImbue : AISkillData
    {
        [Group("Imbue")]
        public float autoImbueTime = 1f;
        [Group("Imbue")]
        public bool recharge = true;
        [Group("Imbue")]
        [FormerlySerializedAs("autoImbueDelay")]
        public float rechargeDelay = 5f;

        [Group("Spell")]
        [Dropdown(nameof(GetAllSpellID))]
        public string spellId;

        [Group("Spell")]
        [DropdownList(nameof(GetAllSpellID))]
        public List<string> randomSpells;

        protected string chosenSpell;
    }
}

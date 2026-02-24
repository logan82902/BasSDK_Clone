using System.Collections;
using System.Collections.Generic;
using TriInspector;
using ThunderRoad.Skill.Spell;
using UnityEngine;

namespace ThunderRoad.Skill
{
    [DeclareBoxGroup("Imbue", Title = "Imbue")]
    [DeclareBoxGroup("Spell", Title = "Spell")]
    public class SkillArcherImbue : AISkillData
    {
        [Group("Imbue")]
        public float autoImbueTime = 1f;

        [Group("Spell")]
        [Dropdown(nameof(GetAllSpellID))]
        public string spellId;

        [Group("Spell")]
        [DropdownList(nameof(GetAllSpellID))]
        public List<string> randomSpells;

    }
}

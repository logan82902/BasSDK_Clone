using System;
using TriInspector;
using System.Collections.Generic;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Status Effect", Title = "Status Effect")]
    public class SkillBurning : SpellSkillData
    {
        [Group("Status Effect")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusEffectId = "Burning";

        [NonSerialized]
        public StatusData statusEffect;
        [Group("Status Effect")]
        public float heatTransfer = 5;

        [Group("Status Effect")]
        [DropdownList(nameof(GetAllMaterialID))]
        public List<string> ignitionBlockedMaterials;
        

    }
}
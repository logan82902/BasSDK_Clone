using System;
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Status Effect", Title = "Status Effect")]
    public class SkillShockParry : SkillData
    {
        [Group("Status Effect")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusId = "Electrocute";

        [Group("Status Effect")]
        public float duration = 3f;

        [NonSerialized]
        public StatusData statusData;


    }

}

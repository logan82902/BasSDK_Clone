using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Status", Title = "Status")]
    public class SkillZapStatusExtender : SpellSkillData
    {
        [Group("Status")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusId;

        [Group("Status")]
        public float duration = 3f;

        protected StatusData statusData;
    }
}

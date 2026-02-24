using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Shove Detection", Title = "Shove Detection")]
    [DeclareBoxGroup("Status", Title = "Status")]
    public class SkillWeightlessShove : SpellSkillData
    {
        [Group("Shove Detection")]
        public float maxDelay = 0.1f;
        [Group("Status")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusId;
        [Group("Status")]
        public float duration = 2f;
        
        protected StatusData statusData;
        protected float lastPush = 0;
        protected Vector3 lastPushDir;

    }
}

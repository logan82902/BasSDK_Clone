using System;
using System.Collections;
using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Status Effect", Title = "Status Effect")]
    [DeclareBoxGroup("Boost", Title = "Boost")]
    public class SkillWeightlessBoost : BoostSkillData
    {
        [Group("Status Effect")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusId;

        [NonSerialized]
        public StatusData statusData;

        [Group("Status Effect")]
        public float duration = 0.3f;

        [Group("Boost")]
        public float maxAngle = 20f;

        [Group("Boost")]
        public float boostWindow = 1;

    }
}

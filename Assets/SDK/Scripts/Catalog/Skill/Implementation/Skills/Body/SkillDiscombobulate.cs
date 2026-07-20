using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TriInspector;

namespace ThunderRoad.Skill
{
    [DeclareBoxGroup("Impact Modifiers", Title = "Impact Modifiers")]
    [DeclareBoxGroup("Bonk Effect", Title = "Bonk Effect")]
    public class SkillDiscombobulate : SkillData
    {

        [Group("Impact Modifiers")]
        public float impactVelocity = 6f;
        [Group("Impact Modifiers")]
        public float headDamageMultiplier = 1f;
        [Group("Impact Modifiers")]
        public float stunTime = 6f;
        [Group("Impact Modifiers")]
        public bool ragdollDuringAttack = false;

        [Group("Bonk Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string bonkEffectId;

        protected EffectData bonkEffectData;

        [Group("Bonk Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string bonkAltEffectId;

        protected EffectData bonkAltEffectData;

    }
}

using TriInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Aim Assist", Title = "Aim Assist")]
    [DeclareBoxGroup("Ricochet", Title = "Ricochet")]
    public class SkillFireballRicochet : SpellSkillData
    {
        [Group("General")]
        public string spellId = "Fire";
        protected int spellHashId;
        [Group("Aim Assist")]
        public bool aimAssist = true;
        [Group("Aim Assist")]
        [ShowIf("aimAssist")]
        public float maxDistance = 10;
        [Group("Aim Assist")]
        [ShowIf("aimAssist")]
        public float maxAngle = 40;
        [Group("Aim Assist")]
        [ShowIf("aimAssist")]
        public float targetMoveBias = 1f;

        [Group("Ricochet")]
        [Dropdown(nameof(GetAllEffectID))]
        public string effectId = "SpellFireballRicochet";
        protected EffectData effectData;
    }
}

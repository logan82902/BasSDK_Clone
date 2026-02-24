using TriInspector;
using ThunderRoad.Skill.SpellPower;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Status Effect", Title = "Status Effect")]
    [DeclareBoxGroup("Telekinesis", Title = "Telekinesis")]
    public class SkillWeightlessKinesis : TkSkillData
    {
        [Group("Status Effect")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusId;
        protected StatusData statusData;
        [Group("Status Effect")]
        [ShowInInspector]
        public FloatingParams itemParams = FloatingParams.Identity;
        [Group("Status Effect")]
        [ShowInInspector]
        public FloatingParams creatureParams = FloatingParams.Identity;
        [Group("Status Effect")]
        public float throwEffectDuration = 2f;
        [Group("Telekinesis")]
        public float strengthMult = 2f;

        [Group("Telekinesis")]
        public bool allowCreature = false;

    }
}

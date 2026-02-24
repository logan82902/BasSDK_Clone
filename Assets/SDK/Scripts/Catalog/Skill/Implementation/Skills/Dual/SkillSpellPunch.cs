using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Punch", Title = "Punch")]
    [DeclareBoxGroup("Spell", Title = "Spell")]
    [DeclareBoxGroup("Status", Title = "Status")]
    [DeclareBoxGroup("Effects", Title = "Effects")]
    public class SkillSpellPunch : SpellSkillData
    {
        [Group("Punch")]
        [Dropdown(nameof(GetAllEffectID))]
        public string baseImpactEffectId;

        protected EffectData baseImpactEffectData;
        [Group("Punch")]
        [Dropdown(nameof(GetAllEffectID))]
        public string spellImpactEffectId;

        protected EffectData spellImpactEffectData;

        [Group("Punch")]
        [Dropdown(nameof(GetAllEffectID))]
        public string flourishEffectId;

        protected EffectData flourishEffectData;

        [Group("Punch")]
        public float minPunchVelocity = 3;
        [Group("Punch")]
        public float cooldown = 0.3f;
        [Group("Spell")]
        [Dropdown(nameof(GetAllSpellID))]
        public string spellId;
        protected int spellHashId;

        [Group("Status")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusId;

        [Group("Status")]
        public float duration = 0;

        [Group("Status")]
        public float statusFloatParam;
        protected StatusData statusData;

        [Group("Effects")]
        public int pushLevel = 0;
        [Group("Effects")]
        public float damage = 0;

        [Group("Effects")]
        public float extraForce = 0;

        [Group("Effects")]
        public bool playerKnockbackAirOnly = true;
        [Group("Effects")]
        public float playerKnockbackMultiplier = 0;
        [Group("Effects")]
        public float playerKnockbackUpMultiplier = 0;
        [Group("Effects")]
        public float maxPlayerKnockback = 10;

        [Group("Effects")]
        public float playerDashForce = 0;

        protected float lastHit;
    }
}

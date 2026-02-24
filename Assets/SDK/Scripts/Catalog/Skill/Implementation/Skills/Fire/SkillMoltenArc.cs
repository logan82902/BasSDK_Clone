using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Effect", Title = "Effect")]
    [DeclareBoxGroup("Damage", Title = "Damage")]
    [DeclareBoxGroup("Status", Title = "Status")]
    public class SkillMoltenArc : SpellSkillData
    {
        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string moltenEffectId;
        protected EffectData moltenEffectData;

        [Group("Effect")]
        [MinMaxSlider(0f, 20f, true)]
        public Vector2 minMaxVelocity = new(5, 16);

        [Group("Effect")]
        [Range(0, 1)]
        public float maxIntensityEnemy = 1;

        [Group("Effect")]
        [Range(0, 1)]
        public float maxIntensityPlayer = 1;

        [Group("Damage")]
        public float damage = 3;

        [Group("Status")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusId = "Burning";

        protected StatusData statusData;

        [Group("Status")]
        public float statusDuration = 3;

        [Group("Status")]
        public float sparkHeatTransferEnemy = 3f;

        [Group("Status")]
        public float sparkHeatTransferPlayer = 5f;
    }
}

using UnityEngine;
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Imbue Hit", Title = "Imbue Hit")]
    public class SkillImpactPulse : SpellSkillData
    {
        [Group("Imbue Hit")]
        public float minMomentum = 8f;
        [Group("Imbue Hit")]
        public float minVelocity = 5f;
        [Group("Imbue Hit")]
        [MinMaxSlider(0, 3, true)]
        public Vector2 extraForce;
        [Group("Imbue Hit")]
        public float maxExtraForce = 10f;
        [Group("Imbue Hit")]
        public ForceMode forceMode = ForceMode.Impulse;

        [Group("Imbue Hit")]
        public float playerMultiplier = 3;

        [Group("Imbue Hit")]
        public float imbueEnergyDrainOnHit = 5f;

        [Group("Imbue Hit")]
        public int pushLevel = 2;

        [Group("Imbue Hit")]
        [Dropdown(nameof(GetAllEffectID))]
        public string effectId;

        protected EffectData effectData;
    }
}

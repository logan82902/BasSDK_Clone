using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Disarm", Title = "Disarm")]
    [DeclareBoxGroup("Impact Force", Title = "Impact Force")]
    [DeclareBoxGroup("Effect", Title = "Effect")]
    public class SkillHeavyFirepower : SpellSkillData
    {
        [Group("Disarm")]
        public bool disarmPlayer = true;
        [Group("Disarm")]
        public bool disarmEnemies = true;
        [Group("Impact Force")]
        public float impactForceItem = 6;
        [Group("Impact Force")]
        public float impactForceCreature = 30;
        [Group("Impact Force")]
        public float impactForcePlayer = 30;
        [Group("Impact Force")]
        public ForceMode impactForceMode = ForceMode.VelocityChange;

        [Group("Impact Force")]
        public int pushLevel = 2;

        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string effectId;

        protected EffectData effectData;
    }
}

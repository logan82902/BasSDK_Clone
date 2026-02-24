using UnityEngine;
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Spread", Title = "Spread")]
    public class SkillConflagration : SpellSkillData
    {
        [Group("Spread")]
        public float radius = 3;
        [Group("Spread")]
        public float heatTransfer = 0.3f;
        [Group("Spread")]
        public float heatTransferToDeadEnemy = 0.1f;
        [Group("Spread")]
        public float heatTransferToCharredEnemy = 0f;
        [Group("Spread")]
        public AnimationCurve rangeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    }
}

using System.Collections;

using UnityEngine;
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Boost", Title = "Boost")]
    [DeclareBoxGroup("Vortex", Title = "Vortex")]
    public class SkillVortexJump : BoostSkillData
    {
        [Group("Boost")]
        public float maxAngle = 20f;
        [Group("Vortex")]
        public float radius = 2f;
        [Group("Vortex")]
        public float duration = 1f;
        [Group("Vortex")]
        public AnimationCurve forceCurve;

    }
}

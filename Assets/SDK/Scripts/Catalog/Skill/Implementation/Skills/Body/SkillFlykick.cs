using UnityEngine;
using TriInspector;

namespace ThunderRoad.Skill
{
    [DeclareBoxGroup("Acceleration", Title = "Acceleration")]
    public class SkillFlykick : SkillData
    {
        [Group("Acceleration")]
        public float speed = 3f;

        [Group("Acceleration")]
        public float downwardVelocityCorrection = 0f;

    }
}

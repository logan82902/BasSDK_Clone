
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Slam", Title = "Slam")]
    public class SkillHoverSlam : SkillData
    {
        [Group("Slam")]
        public float slamGravityRatio = 5;
        [Group("Slam")]
        public float slamDuration = 2f;

    }
}

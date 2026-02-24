
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Focus", Title = "Focus")]
    public class SkillCombatFocus : SkillData
    {
        [Group("Focus")]
        public float killRegen = 15;
        [Group("Focus")]
        public float sliceRegen = 30;
        [Group("Focus")]
        public float parryRegen = 10;
        [Group("Focus")]
        public float deflectRegen = 15;
        [Group("Focus")]
        public float rangedHeadshotRegen = 30;
        [Group("Focus")]
        public float disarmRegen = 15;

    }
}

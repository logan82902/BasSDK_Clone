using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Status", Title = "Status")]
    public class SkillSlowingParry : SkillData
    {
        [Group("Status")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusId;

        protected StatusData statusData;

        [Group("Status")]
        public float duration = 1f;
        [Group("Status")]
        public float extraSlowMult = 0.3f;
    }
}

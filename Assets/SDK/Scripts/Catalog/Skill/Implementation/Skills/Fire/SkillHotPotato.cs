using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Status", Title = "Status")]
    public class SkillHotPotato : SkillHeldImbue
    {
        [Group("Status")]
        public float speedMultiplier = 1.5f;
        [Group("Status")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusEffectId;
        public StatusData statusEffect;

    }
}

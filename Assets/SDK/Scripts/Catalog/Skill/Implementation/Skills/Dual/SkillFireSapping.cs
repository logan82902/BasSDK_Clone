
using TriInspector;
namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Status Effect", Title = "Status Effect")]
    public class SkillFireSapping : SkillSapStatusApplier
    {
        [Group("Status Effect")]
        public float heatTransferPerBolt = 10f;
    }
}

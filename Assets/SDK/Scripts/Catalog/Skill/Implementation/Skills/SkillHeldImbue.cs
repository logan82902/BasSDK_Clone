using System.Collections.Generic;

using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Spell", Title = "Spell")]
    public class SkillHeldImbue : SkillData
    {
        [Group("Spell")]
        [Dropdown(nameof(GetAllSpellID))]
        public string spellId;


        public virtual void Activate(Creature creature)
        {
        }

        public virtual void Deactivate(Creature creature)
        {
        }
    }
}

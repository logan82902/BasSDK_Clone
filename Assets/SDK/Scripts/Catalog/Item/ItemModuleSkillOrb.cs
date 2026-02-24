using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad
{
    public class ItemModuleSkillOrb : ItemModule
    {
        [Dropdown(nameof(GetAllSkillID))]
        public string skillId;

        protected SkillData skillData;
        public TriDropdownList<string> GetAllSkillID() => Catalog.GetDropdownAllID(Category.Skill);

    }
}

using System;
using System.Collections;
using TriInspector;

namespace ThunderRoad
{
    public class SpellModule
    {
        public virtual void OnCatalogRefresh()
        {
        }

        public virtual IEnumerator OnCatalogRefreshCoroutine()
        {
            yield return 0;
        }

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);
        public TriDropdownList<string> GetAllDamagerID() => Catalog.GetDropdownAllID(Category.Damager);

        public TriDropdownList<string> GetAllSkillID() => Catalog.GetDropdownAllID(Category.Skill);

        public TriDropdownList<string> GetAllItemID() => Catalog.GetDropdownAllID(Category.Item);

        public TriDropdownList<string> GetAllStatusEffectID() => Catalog.GetDropdownAllID(Category.Status);

        public virtual SpellModule Clone()
        {
            return this.MemberwiseClone() as SpellModule;
        }

    }
}
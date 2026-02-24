using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad
{
    [SkipInTypeDropdown]
    public class EntityData : CatalogData
    {
        [ShowInInspector]
        [DropdownList(nameof(GetAllEntityModuleIDs))]
        public List<string> entityModules;

        public TriDropdownList<string> GetAllEntityModuleIDs() => Catalog.GetDropdownAllID(Category.EntityModule);

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            entityModules ??= new List<string>();
        }

        public void Load(ThunderEntity entity)
        {
            for (var i = 0; i < entityModules.Count; i++)
            {
                var module = Catalog.GetData<EntityModule>(entityModules[i]).Clone() as EntityModule;
                if (module is not EntityModule) continue;
                module.Load(entity);
                entity.entityModules.Add(module);
            }
        }
    }
}
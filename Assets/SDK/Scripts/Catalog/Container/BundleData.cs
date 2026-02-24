using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    public class BundleData : CatalogData, IContainerLoadable<BundleData>, ILootTableDrop
    {
        public string localizationGroup = "Bundles";
        public string displayName;
        public string description;
        public string altDescriptionTitle = "ContentsTitle";
        public bool isArmorBundle;

        public override string SortKey()
        {
            return groupPath + id;
        }

        public TriDropdownList<string> GetAllTextId()
        {
            return Catalog.GetTextData().GetDropdownAllTexts(localizationGroup);
        }


        [Dropdown(nameof(GetAllItemID))]
        public string bundleItemId = "Bundle";

        protected ItemData bundleItemData;
        [DropdownList(nameof(GetAllItemID))]
        public List<string> items = new();
        [DropdownList(nameof(GetAllSkillID))]
        public List<string> skills = new();
        [DropdownList(nameof(GetAllLootTableID))]
        public List<string> lootTables = new();

        public string spriteAddress = "Ui.Icon.Unknown";

        public TriDropdownList<string> GetAllItemID() => Catalog.GetDropdownAllID(Category.Item);

        public TriDropdownList<string> GetAllSkillID() => Catalog.GetDropdownAllID(Category.Skill);

        public TriDropdownList<string> GetAllLootTableID() => Catalog.GetDropdownAllID(Category.LootTable);

        public void OnLoadedFromContainer(Container container)
        { }

        public ContainerContent InstanceContent()
        {
            return new BundleContent(this);
        }

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            bundleItemData = Catalog.GetData<ItemData>(bundleItemId);
        }

        public bool ContainsItemID(string id) => items.Contains(id);

        public bool ContainsItemID(ItemData item) => ContainsItemID(item.id);


        public void ValidateIDs()
        {
            // Validate all catalog ID references
            foreach (var itemId in items)
                if (!Catalog.TryGetData<ItemData>(itemId, out _))
                    Debug.LogWarning($"Bundle {id} item {itemId} does not exist in catalog");
            foreach (var skillId in skills)
                if (!Catalog.TryGetData<SkillData>(skillId, out _))
                    Debug.LogWarning($"Bundle {id} skill {skillId} does not exist in catalog");
            foreach (var tableId in lootTables)
                if (!Catalog.TryGetData<LootTable>(tableId, out _))
                    Debug.LogWarning($"Bundle {id} loot table {tableId} does not exist in catalog");
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(5)]
#endif
    [DeclareHorizontalGroup("Trade")]
    [DeclareHorizontalGroup("Restocking")]
    public class ShopData : CatalogData
    {
        [Dropdown(nameof(GetAllCreatureIDs))]
        public string shopKeeperCreatureID;
        [Group("Trade")]
        public string tradeCurrency = "Gold";
        [Group("Trade")]
        public float defaultUsedValueMultiplier = 0f;
        [Group("Trade")]
        public RoundMethod roundingMethod;
        [Group("Restocking")]
        public int restockDayInterval = 2;
        [Group("Restocking")]
        public DisplaySorting displaySorting;

        [ListDrawerSettings(AlwaysExpanded = true)]
        [HideReferencePicker]
        public List<ShopStockCategory> shopStockCategories = new();

        public TriDropdownList<string> GetAllCreatureIDs() => Catalog.GetDropdownAllID(Category.Creature);

        public TriDropdownList<string> GetAllItemIDs() => Catalog.GetDropdownAllID(Category.Item);

        public enum DisplaySorting
        {
            MostValuableWithoutDiscount,
            MostValuableWithDiscount,
            Oldest,
            Newest,
            Random,
        }

        public enum RoundMethod
        {
            RoundNormal,
            RoundUp,
            RoundDown,
        }

        [Serializable]
        [DeclareBoxGroup("Box", Title = "$" + nameof(stockCategory))]
        [DeclareHorizontalGroup("Box/Top")]
        [DeclareHorizontalGroup("Box/Horiz", Sizes = new float[] { 350f, 0f })]
        [DeclareBoxGroup("Box/Horiz/Trade", Title = "Trade")]
        [DeclareHorizontalGroup("Box/Horiz/Trade/Horiz1", Sizes = new float[] { 0f, 130f })]
        [DeclareBoxGroup("Box/Horiz/Stocking", Title = "Stocking")]
        [DeclareHorizontalGroup("Box/Horiz/Stocking/Horiz2", Sizes = new float[] { 0f, 165f, 75f })]
        [DeclareHorizontalGroup("Box/Debug")]
        public class ShopStockCategory
        {
            [JsonMergeKey]
            [Group("Box/Top")]
            [LabelText("Category")]
            [LabelWidth(65)]
            public string stockCategory;

            [Group("Box/Top")]
            [LabelText("Upgrade Checkpoints")]
            [LabelWidth(130)]
            public string upgradeCheckpoints;

            [Group("Box/Horiz/Trade/Horiz1")]
            [LabelText("\"Used\" Value Mult")]
            [LabelWidth(110)]
            [Range(0f, 1f)]
            public float usedValueMultiplier = 1f;

            [Group("Box/Horiz/Trade/Horiz1")]
            [LabelText("Despawn On Trade")]
            [LabelWidth(115)]
            public bool despawnOnTrade = false; // Whether to remove this item when traded

            [JsonIgnore]
            [ShowInInspector]
            [Group("Box/Horiz/Stocking/Horiz2")]
            [LabelWidth(50)]
            [Dropdown(nameof(GetAllLootTableIDs))]
            public string tableId
            {
                get => restockTableID;
                set => restockTableID = value;
            }

            [HideInInspector]
            public string restockTableID = "None";

            [JsonIgnore]
            [ShowInInspector]
            [Group("Box/Horiz/Stocking/Horiz2")]
            [LabelText("Stock Quantity: Min")]
            [LabelWidth(115f)]
            public int min
            {
                get => minMaxStockQuantity.x;
                set => minMaxStockQuantity.x = Mathf.Min(value, max);
            }

            [JsonIgnore]
            [ShowInInspector]
            [Group("Box/Horiz/Stocking/Horiz2")]
            [LabelWidth(25f)]
            public int max
            {
                get => minMaxStockQuantity.y;
                set => minMaxStockQuantity.y = Mathf.Max(min, value);
            }

            [HideInInspector]
            public Vector2Int minMaxStockQuantity = new();

            [NonSerialized]
            public LootTableBase restockTable;

            [NonSerialized]
            public ShopData containingData;

            public TriDropdownList<string> GetAllLootTableIDs() => Catalog.GetDropdownAllID(Category.LootTable);

            [NonSerialized]
            [ShowInInspector]
            [ReadOnly]
            [Group("Box/Debug")]
            [InlineButton(nameof(LoadAcceptedIDs), LeftSide = true)]
            public List<string> allAcceptedIDs;

            public void LoadAcceptedIDs()
            {
                restockTable = Catalog.GetData<LootTableBase>(restockTableID);
                if (restockTable == null)
                {
                    Debug.LogError($"Missing restock table for category {stockCategory}! The shop doesn't know which items belong in this category and they will use the default used value multiplier for buying/selling!");
                    return;
                }
                allAcceptedIDs = new List<string>();
                foreach (ItemData itemData in restockTable.GetAll())
                {
                    if (itemData == null) continue;
                    if (allAcceptedIDs.Contains(itemData.id)) continue;
                    allAcceptedIDs.Add(itemData.id);
                }
            }
        }

        [NonSerialized]
        public Dictionary<string, ShopStockCategory> shopStockDictionary;

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            shopStockDictionary = new Dictionary<string, ShopStockCategory>();
            for (int i = 0; i < shopStockCategories.Count; i++)
            {
                ShopStockCategory shopCategory = shopStockCategories[i];
                shopCategory.containingData = this;
                if (string.IsNullOrEmpty(shopCategory.stockCategory))
                {
                    Debug.LogError($"Shop [{id}] category element #{i} is missing a category name!");
                    continue;
                }
                shopStockDictionary.Add(shopCategory.stockCategory, shopCategory);
                shopCategory.LoadAcceptedIDs();
            }
        }
    }
}

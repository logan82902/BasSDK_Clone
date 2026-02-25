using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    public class LootTable : LootTableBase, IContainerLoadable<LootTable>
    {

        [LabelText("Drop Levels")]
        [HideReferencePicker]
        [ListDrawerSettings(Draggable = false, HideAddButton = true, HideRemoveButton = true)]
        public List<DropLevel> levelledDrops = new List<DropLevel>();
        private const int maxPickCount = 20;

        

        [Serializable]
        [DeclareBoxGroup("$elementName", Title = "$elementName")]
        [DeclareVerticalGroup("$elementName/Top")]
        [DeclareHorizontalGroup("$elementName/Top/Row")]
        [DeclareVerticalGroup("$elementName/Bottom")]
        public class DropLevel
        {
            [JsonMergeKey]
            [HideInInspector]
            public int dropLevel = -1;
#if UNITY_EDITOR
            [Button]
            [Group("$elementName/Top/Row")]
            [PropertyOrder(0)]
            public void RemoveThisLevel()
            {
                lootTable.levelledDrops.Remove(this);
                // Update the drop levels
                for (int i = 0; i < lootTable.levelledDrops.Count; i++)
                {
                    lootTable.levelledDrops[i].dropLevel = i;
                }
                //reorder the drop levels based on their drop level
                lootTable.levelledDrops = lootTable.levelledDrops.OrderBy(x => x.dropLevel).ToList();
            }

            [Button]
            [Group("$elementName/Top/Row")]
            [PropertyOrder(0)]
            public void MoveDown()
            {
                //move down the drop level
                int index = lootTable.levelledDrops.IndexOf(this);
                if (index >= 0 && index < lootTable.levelledDrops.Count)
                {
                    if (index - 1 < 0) return;
                    lootTable.levelledDrops[index] = lootTable.levelledDrops[index - 1];
                    lootTable.levelledDrops[index - 1] = this;
                    lootTable.levelledDrops[index].dropLevel = index;
                    dropLevel = index - 1;
                }
            }
            [Button]
            [Group("$elementName/Top/Row")]
            [PropertyOrder(0)]
            public void MoveUp()
            {
                //move up the drop level
                int index = lootTable.levelledDrops.IndexOf(this);
                if (index >= 0 && index < lootTable.levelledDrops.Count)
                {
                    if (index + 1 == lootTable.levelledDrops.Count) return;
                    lootTable.levelledDrops[index] = lootTable.levelledDrops[index + 1];
                    lootTable.levelledDrops[index + 1] = this;
                    lootTable.levelledDrops[index].dropLevel = index;
                    dropLevel = index + 1;
                }
            }
#endif

            [LabelWidth(120f)]
            [Group("$elementName/Top/Row")]
            [NonSerialized]
            public ItemData.Type itemCategory;
            [Space]
            [TableList(AlwaysExpanded = true)]
            [HideReferencePicker]
            [Group("$elementName/Bottom")]
            [PropertyOrder(4)]
            public List<Drop> drops;

            [JsonIgnore]
            public string elementName => $"Drop level {dropLevel}";

            [NonSerialized]
            public LootTable lootTable;
            [NonSerialized]
            public float probabilityTotalWeight;
            [NonSerialized]
            public float minMoneyValue;
            [NonSerialized]
            public float maxMoneyValue;
            [NonSerialized]
            public float minRewardValue;
            [NonSerialized]
            public float maxRewardValue;

            private void AddEntryForEachInCategory()
            {
                foreach (ItemData itemData in Catalog.GetDataList<ItemData>())
                {
                    if (itemData.type == itemCategory)
                    {
                        drops.Add(new Drop()
                        {
                            reference = Drop.Reference.Item,
                            referenceID = itemData.id,
                            probabilityWeight = 1f
                        });
                    }
                }
            }

            public DropLevel Clone()
            {
                DropLevel clone = new DropLevel();
                clone.drops = clone.drops.Select(item => (Drop)item.Clone()).ToList();
                return clone;
            }

        }

        [Serializable]
        public class Drop
        {
            public enum RandMode
            {
                ItemCount,
                MoneyValue,
                RewardValue,
            }

            [JsonMergeKey(MergeKeyValueProvider = nameof(GetMergeKey))]
            [HideInInspector]
            public string mergeKey = "";

            [Dropdown(nameof(GetAllItemOrLootTableID))]
            [PropertyOrder(1)]
            public string referenceID;

            [PropertyOrder(0)]
            [EnumToggleButtons]
            public Reference reference = Reference.Item;

            [NonSerialized]
            public ItemData itemData;
            [NonSerialized]
            public LootTable lootTable;
            [NonSerialized]
            public BundleData bundleData;
            [NonSerialized]
            public SkillData skillData;

            [PropertyOrder(1)]
            [LabelText("Drop Mode")]
            public RandMode randMode = RandMode.ItemCount;
            [PropertyOrder(2)]
            [LabelText("Drop Min/Max")]
            public Vector2 minMaxRand = Vector2.one;

            [PropertyOrder(3)]
            public float probabilityWeight;

            [PropertyOrder(4)]
            [ShowInInspector]
            [ReadOnly]
            //[ProgressBar(0, 100)]
            [NonSerialized]
            public float probabilityPercent;

            [NonSerialized]
            public float probabilityRangeFrom;
            [NonSerialized]
            public float probabilityRangeTo;

            public static string GetMergeKey()
            {
                return Guid.NewGuid().ToString().Substring(0, 8);
            }

            public TriDropdownList<string> GetAllItemOrLootTableID()
            {
                return reference switch
                {
                    Reference.Item => Catalog.GetDropdownAllID(Category.Item),
                    Reference.Bundle => Catalog.GetDropdownAllID(Category.Bundle),
                    Reference.Skill => Catalog.GetDropdownAllID(Category.Skill),
                    Reference.Table => Catalog.GetDropdownAllID(Category.LootTable)
                };
            }

            public enum Reference
            {
                Item,
                Table,
                Skill,
                Bundle
            }

            public Drop Clone()
            {
                return MemberwiseClone() as Drop;
            }

            public BuildSettings.ContentFlag sensitiveContent = BuildSettings.ContentFlag.None;
            public BuildSettings.ContentFlagBehaviour sensitiveFilterBehaviour = BuildSettings.ContentFlagBehaviour.Discard;

        }

        public override int GetCurrentVersion()
        {
            return 1;
        }
        public void OnLoadedFromContainer(Container container)
        { }

        public ContainerContent InstanceContent()
        { return null; }
    }
}

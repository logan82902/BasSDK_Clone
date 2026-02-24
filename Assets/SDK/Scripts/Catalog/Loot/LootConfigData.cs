using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(5)]
#endif
    [DeclareBoxGroup("Data", Title = "Data")]
    public class LootConfigData : CatalogData
    {
        [Group("Data")]
        [Dropdown(nameof(GetAllLootTableID))]
        public string sideRoomLootTableID;

        [Group("Data")]
        [Dropdown(nameof(GetAllLootTableID))]
        public string enemyDropLootTableID;

        [Group("Data")]
        [Dropdown(nameof(GetAllLootTableID))]
        public string treasureLootTableID;

        [Group("Data")]
        [Dropdown(nameof(GetAllLootTableID))]
        public string rewardLootTableID;

        [Group("Data")]
        [Dropdown(nameof(GetAllLootTableID))]
        public string altSideRoomLootTableID;

        [Group("Data")]
        [Dropdown(nameof(GetAllLootTableID))]
        public string altTreasureLootTableID;


        public TriDropdownList<string> GetAllLootTableID()
        {
            TriDropdownList<string> dropdownList = new() { new TriDropdownItem<string>() { Text = "None", Value = "" } };
            List<string> creatureTableIds = Catalog.GetDataList(Category.LootTable).OfType<LootTableBase>().Select(x => x.id).ToList();
            foreach (string id in creatureTableIds)
            {
                dropdownList.Add(id, id);
            }
            return dropdownList;
        }
    }
}
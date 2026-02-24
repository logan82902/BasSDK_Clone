using System;
using TriInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ThunderRoad.Modules
{
    /// <summary>
    /// Handles creating and updating the level instances for the Crystal Hunt game mode
    /// </summary>
    [Serializable]
    [DeclareBoxGroup("MapPins", Title = "Map Pins")]
    [DeclareHorizontalGroup("MapPins/Horiz")]
    [DeclareBoxGroup("Shop & Persistent Levels")]
    [DeclareBoxGroup("Tutorial", Title = "Tutorial Dungeon")]
    [DeclareHorizontalGroup("Tutorial/Horiz")]
    [DeclareHorizontalGroup("LevelsHoriz")]
    [DeclareBoxGroup("LevelsHoriz/Outposts", Title = "Outpost Dungeons")]
    [DeclareHorizontalGroup("LevelsHoriz/Outposts/Horiz")]
    [DeclareBoxGroup("LevelsHoriz/Outposts/Horiz/EnemyConfigs", Title = "Enemy Configs")]
    [DeclareBoxGroup("LevelsHoriz/Outposts/Horiz/LootConfigs", Title = "Loot Configs")]
    [DeclareBoxGroup("LevelsHoriz/Arenas", Title = "Arenas")]
    [DeclareHorizontalGroup("LevelsHoriz/Arenas/Horiz")]
    [DeclareBoxGroup("LevelsHoriz/Dalgarian", Title = "Dalgarian Dungeon")]
    [DeclareHorizontalGroup("LevelsHoriz/Dalgarian/Horiz")]
    [DeclareBoxGroup("LevelsHoriz/Dalgarian/Horiz/EnemyConfigs", Title = "Enemy Configs")]
    [DeclareBoxGroup("LevelsHoriz/Dalgarian/Horiz/LootConfigs", Title = "Loot Configs")]
    [DeclareBoxGroup("Balancing", Title = "Balancing Data")]
    public class CrystalHuntLevelInstancesModule : LevelInstancesModule
    {
        //this dungeon type describes if this is a loot heavy or shard heavy dungeon
        // this enum will be used to get the correct multiplier for the end reward
        public enum LootType
        {
            Loot,
            Shard,
            Arena,
            Dalgarian,
        }

        [Group("MapPins")]
        public string mapInfoPrefabAddress;

        [Group("MapPins")]
        public string difficultyIcon;
        [Group("MapPins")]
        public Color difficultyIconColor;

        [Group("MapPins/Horiz")]
        public int minPinsToGeneratePerDay = 2;
        [Group("MapPins/Horiz")]
        public int maxPinsToGeneratePerDay = 4;

        [Group("Shop & Persistent Levels")]
        public string shopID;
        [Group("Shop & Persistent Levels")]
        [HideReferencePicker]
        [ListDrawerSettings(AlwaysExpanded = true)]
        public List<LevelInfo> persistentLevels;

        [Group("Tutorial")]
        [InlineProperty]
        [HideReferencePicker]
        [NullValueFix]
        public LevelInfo tutorialDungeonLevelInfo;

        [Group("Tutorial/Horiz")]
        [Dropdown(nameof(GetAllEnemyConfigs))]
        public string tutorialEnemyConfig;

        [Group("Tutorial/Horiz")]
        [Dropdown(nameof(GetAllLootConfigs))]
        public string tutorialLootConfig;

        [Group("LevelsHoriz/Outposts")]
        public int outpostMapLocationRandomNearest = 5;

        [Group("LevelsHoriz/Outposts")]
        [InlineProperty]
        [HideReferencePicker]
        [NullValueFix]
        public LevelInfo outpostDungeonLevelInfo;

        [Group("LevelsHoriz/Outposts/Horiz/EnemyConfigs")]
        [InlineProperty]
        [HideReferencePicker]
        [NullValueFix]
        public EnemyConfigTiers outpostEnemyConfigs;

        [Group("LevelsHoriz/Outposts/Horiz/LootConfigs")]
        [InlineProperty]
        [HideReferencePicker]
        [NullValueFix]
        public LootConfigTiers outpostLootConfigs;

        [Group("LevelsHoriz/Arenas/Horiz")]
        public int minNumberOfArenasAllowedPerDay = 1;
        [Group("LevelsHoriz/Arenas/Horiz")]
        public int maxNumberOfArenasAllowedPerDay = 1;
        [Group("LevelsHoriz/Arenas")]
        [HideReferencePicker]
        [ListDrawerSettings(AlwaysExpanded = true)]
        public List<LevelInfo> arenasLevelInfo;

        [Group("LevelsHoriz/Dalgarian")]
        public int dalgarianMapLocationRandomNearest = 3;

        [Group("LevelsHoriz/Dalgarian")]
        [InlineProperty]
        [HideReferencePicker]
        [NullValueFix]
        public LevelInfo dalgarianDungeonLevelInfo;

        [Group("LevelsHoriz/Dalgarian/Horiz/EnemyConfigs")]
        [InlineProperty]
        [HideReferencePicker]
        [NullValueFix]
        public EnemyConfigTiers dalgarianEnemyConfigs;

        [Group("LevelsHoriz/Dalgarian/Horiz/LootConfigs")]
        [InlineProperty]
        [HideReferencePicker]
        [NullValueFix]
        public LootConfigTiers dalgarianLootConfigs;

 // ProjectCore

        [System.Serializable]
        public class EnemyConfigTiers : ConfigTiers<EnemyConfig> { }

        [System.Serializable]
        public class LootConfigTiers : ConfigTiers<LootConfigData> { }

        [AutoLabelWidth(MinWidth = 0f)]
        public abstract class ConfigTiers<T> where T : CatalogData
        {
            public List<string> GetTier(int tier)
            {
                return tier switch
                {
                    0 => Tier0,
                    1 => Tier1,
                    2 => Tier2,
                    3 => Tier3,
                    _ => Tier0
                };
            }

            public TriDropdownList<string> GetAllOptionIDs()
            {
                return Catalog.GetDropdownAllID<T>();
            }

            public TriDropdownList<string> GetAllEnemyConfigs() => Catalog.GetDropdownAllID<EnemyConfig>();

            public TriDropdownList<string> GetAllLootConfigs() => Catalog.GetDropdownAllID<LootConfigData>();

            [ListDrawerSettings(AlwaysExpanded = true)]
            [DropdownList(nameof(GetAllOptionIDs))]
            public List<string> Tier0;
            [ListDrawerSettings(AlwaysExpanded = true)]
            [DropdownList(nameof(GetAllOptionIDs))]
            public List<string> Tier1;
            [ListDrawerSettings(AlwaysExpanded = true)]
            [DropdownList(nameof(GetAllOptionIDs))]
            public List<string> Tier2;
            [ListDrawerSettings(AlwaysExpanded = true)]
            [DropdownList(nameof(GetAllOptionIDs))]
            public List<string> Tier3;
        }

        [AutoLabelWidth(MinWidth = 0f)]
        [System.Serializable]
        [DeclareBoxGroup("Box", HideTitle = true)]
        [DeclareHorizontalGroup("Box/HorizTop")]
        [DeclareHorizontalGroup("Box/HorizBottom")]
        public class LevelInfo
        {
            [Group("Box/HorizTop")]
            [Dropdown(nameof(GetAllLevelId))]
            public string levelId;
            [Group("Box/HorizTop")]
            [Dropdown(nameof(GetAllLevelMode))]
            public string modeId;

            [Group("Box/HorizBottom")]
            [Dropdown(nameof(GetAllEnemyConfigs))]
            public string staticEnemyConfigDataId;

            [Group("Box/HorizBottom")]
            [FormerlySerializedAs("lootConfigDataId")]
            [Dropdown(nameof(GetAllLootConfigs))]
            public string staticLootConfigDataId;
            
            [Newtonsoft.Json.JsonProperty("modId")]
            private string modeIdSetter
            {
                set
                {
                    modeId = value;
                }
            }

 // ProjectCore
            public TriDropdownList<string> GetAllEnemyConfigs() => Catalog.GetDropdownAllID<EnemyConfig>();

            public TriDropdownList<string> GetAllLootConfigs() => Catalog.GetDropdownAllID<LootConfigData>();

            public TriDropdownList<string> GetAllLevelId() => Catalog.GetDropdownAllID<LevelData>();

            public TriDropdownList<string> GetAllLevelMode()
            {
                TriDropdownList<string> result = new();
                LevelData level = Catalog.GetData<LevelData>(levelId);
                if (level == null)
                {
                    result.Add("None", null);
                    return result;
                }

                List<LevelData.Mode> levelModes = level.modes;
                int count = levelModes.Count;
                for (int i = 0; i < count; i++)
                {
                    result.Add(levelModes[i].name, levelModes[i].name);
                }
                return result;
            }
        }


        #region ScriptableObjects

        [Group("Balancing")]
        public string endRewardBalanceAddress;
        [Group("Balancing")]
        public string dungeonLengthBalanceAddress;
        [Group("Balancing")]
        public string dungeonLootMultiplierBalanceAddress;
        [Group("Balancing")]
        public string dungeonTypeBalanceAddress;
        [Group("Balancing")]
        public string outpostFactionTierBalanceAddress;
        [Group("Balancing")]
        public string dalgarianFactionTierBalanceAddress;
 // ProjectCore
        #endregion


        public TriDropdownList<string> GetAllEnemyConfigs() => Catalog.GetDropdownAllID<EnemyConfig>();

        public TriDropdownList<string> GetAllLootConfigs() => Catalog.GetDropdownAllID<LootConfigData>();
    }
}

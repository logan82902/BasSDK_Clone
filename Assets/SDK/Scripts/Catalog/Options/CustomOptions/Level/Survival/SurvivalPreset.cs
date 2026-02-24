using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    [Serializable]
    public struct SurvivalPreset
    {
        [Dropdown(nameof(GetAllTextId))]
        public string displayName;
        [Dropdown(nameof(GetAllTextId))]
        public string description;
        
        public LevelModuleSurvival.SurvivalModeLoopBehavior loopBehavior;
        public bool rouletteMode;
        public bool forbidEnemyWeapons;
        public bool spawnWeapons;
        public bool spawnCombatSkills;
        public bool spawnUtilitySkills;
        public bool spawnPotions;
        public bool spawnArmor;
        public bool spawnBundles;
        public bool removePlayerArmor;
        public bool removePlayerSkills;
        public bool removePlayerWeapons;
        
        public TriDropdownList<string> GetAllTextId()
        {
            return Catalog.GetTextData().GetDropdownAllTexts("Survival");
        }

        public string LocalizedName()
        {
            return displayName;
        }

        public string LocalizedDesc()
        {
            return description;
        }

    }
}
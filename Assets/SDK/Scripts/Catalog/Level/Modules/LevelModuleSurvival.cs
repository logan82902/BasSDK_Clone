using TriInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ThunderRoad.Skill.Spell;
using ThunderRoad.Skill.SpellPower;
using UnityEngine;
using Object = UnityEngine.Object;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ThunderRoad
{
    public class LevelModuleSurvival : LevelModule, ISurvivalModule
    {
        #region Classes

        [Serializable]
        public class Waves
        {
            public TriDropdownList<string> GetAllWaveID()
            {
                return Catalog.GetDropdownAllID(Category.Wave);
            }

            [Dropdown(nameof(GetAllWaveID))]
            public string waveID;
            public TriDropdownList<string> GetAllLootTableID()
            {
                return Catalog.GetDropdownAllID(Category.LootTable);
            }

            public string GetTableID(RewardTypes type) => type switch
            {
                RewardTypes.Weapons => weaponTable,
                RewardTypes.Potions => potionTable,
                RewardTypes.Armor => armorTable,
                RewardTypes.Bundles => bundleTable,
                _ => null
            };

            [Dropdown(nameof(GetAllLootTableID))]
            public string weaponTable;
            [Dropdown(nameof(GetAllLootTableID))]
            public string armorTable;
            [Dropdown(nameof(GetAllLootTableID))]
            public string potionTable;
            [Dropdown(nameof(GetAllLootTableID))]
            public string bundleTable;
        }

        public enum SurvivalModeLoopBehavior
        {
            NoLoop,
            Loop,
            RepeatLastWave
        }

        [Flags]
        public enum EquipmentSettings
        {
            None = 0,
            ForbidEnemyWeapons = 1,
            RouletteMode = 2,
            RemovePlayerArmor = 4,
            RemovePlayerSkills = 8,
            RemovePlayerWeapons = 16,
        }

        [Flags]
        public enum RewardTypes
        {
            None = 0,
            Weapons = 1,
            OffensiveSkills = 2,
            MiscSkills = 4,
            Potions = 8,
            Armor = 16,
            Bundles = 32,
        }
        #endregion

        [Title("General")]
        public bool trueRandomInProgressive = false;
        public static Queue<CatalogData> forceSpawnDataQueue = new();

        [Title("Skill Weighting")]
        [LabelText("Dual Tree Extra Weight")]
        public float dualTreeWeight = 0.4f;
        [LabelText("Non-Core Skill Extra Weight")]
        public float subSkillWeight = 0.6f;
        [InfoBox("Skill spawning weight multiplier based on tier as wave number increases. "
                 + "First element is for Tier 1, second is for Tier 2 etc. ")]
        public List<AnimationCurve> skillTierWeightPerWave;
        // = new() { AnimationCurve.Linear(0, 1, 20, 0.5f), AnimationCurve.Linear(0, 0.5f, 20, 1f), AnimationCurve.Linear(0, 0.1f, 20, 0.5f), };

        [Title("Reward Weightings")]
        public float weaponWeight = 4;
        public float offensiveSkillWeight = 3;
        public float miscSkillWeight = 1;
        public float potionWeight = 1;
        public float armorWeight = 1;
        public float bundleWeight = 1;

        private string GetWeightPercentageString(RewardTypes type) => $"{Mathf.RoundToInt(GetRewardWeight(type) * 100)}%";
        public float GetRewardWeight(RewardTypes type)
        {
            float total = weaponWeight
                          + offensiveSkillWeight
                          + miscSkillWeight
                          + potionWeight
                          + armorWeight
                          + bundleWeight;

            if (total == 0) return 0;
            return type switch
            {
                RewardTypes.Weapons => weaponWeight,
                RewardTypes.OffensiveSkills => offensiveSkillWeight,
                RewardTypes.MiscSkills => miscSkillWeight,
                RewardTypes.Potions => potionWeight,
                RewardTypes.Armor => armorWeight,
                RewardTypes.Bundles => bundleWeight
            } / total;
        }
        
        // Options
        [JsonProperty]
        public bool forbidEnemyWeapons
        {
            set => equipmentSettings |= EquipmentSettings.ForbidEnemyWeapons;
        }

        [Title("Default Level Options")]
        public EquipmentSettings equipmentSettings = EquipmentSettings.None;
        public RewardTypes rewardTypes;
        
        [Dropdown(nameof(GetAllContainerID))]
        public string startingContainerId = "SurvivalStartingContainer";
        protected ContainerData startingContainer;

        [Title("Reward Pillars")]
        public string rewardPillarAddress;

        public string raiseSoundAudioContainerAddress = "Bas.AudioGroup.Survival.Pillar.Raise.Stone";
        public string lowerSoundAudioContainerAddress = "Bas.AudioGroup.Survival.Pillar.Lower.Stone";

        [NonSerialized]
        public AudioContainer raiseSound;
        [NonSerialized]
        public AudioContainer lowerSound;

        [JsonIgnore]
        [ShowInInspector]
        public bool hasPillarZone
        {
            get
            {
                return pillarZone.HasValue;
            }
            set
            {
                if (value)
                {
                    pillarZone = new Bounds(Vector3.zero, Vector3.one);
                }
                else
                {
                    pillarZone = null;
                }
            }
        }
        [JsonIgnore]
        [ShowInInspector]
        [HideLabel]
        [ShowIf(nameof(hasPillarZone))]
        public Bounds pillarZoneBounds
        {
            get
            {
                return pillarZone.GetValueOrDefault();
            }
            set
            {
                pillarZone = value;
            }
        }

        public bool useCustomTeleportPoint = false;
        public Vector3 customTeleportPoint = Vector3.zero;

        public bool TryGetTeleportPosition(out Vector3 position)
        {
            position = customTeleportPoint;
            return useCustomTeleportPoint;
        }

        public bool TryGetPillarSounds(out AudioContainer raise, out AudioContainer lower)
        {
            raise = raiseSound;
            lower = lowerSound;
            return true;
        }

        [HideInInspector]
        public Bounds? pillarZone;
        public int rewardsToSpawn = 3;
        public bool normalizeItemScale = false;
        [Title("Catalog Data")]
        [Dropdown(nameof(GetAllEffectID))]
        public string getRewardEffectId = "GetReward";
        protected EffectData getRewardEffect;
        [Dropdown(nameof(GetAllEffectID))]
        public string victoryEffectId = "OutpostChestOpen";
        protected EffectData victoryEffectData;
        [Dropdown(nameof(GetAllEffectID))]
        public string lightningZapEffectId;
        [Dropdown(nameof(GetAllEffectID))]
        public string dropItemEffectId = "SurvivalDropItem";

        [NullValueFix]
        public Gradient shockGradient = Utils.CreateGradient(new Color(0.8f, 0.8f, 0.8f), new Color(0.6f, 0.6f, 0.6f));
        
        [Dropdown(nameof(GetAllEffectID))]
        public string rewardEffectId = "SurvivalRewardHighlight";
        [Dropdown(nameof(GetAllItemID))]
        public string skillItemId = "SkillOrbItem";
        [NonSerialized]
        public ItemData skillItemData;

        [Title("Waves")]
        public float startDelay = 10;
        public SurvivalModeLoopBehavior loop = SurvivalModeLoopBehavior.Loop;
        public int wavesNumberForReward = 1;
        public List<Waves> waves;

        public delegate void WaveComplete(LevelModuleSurvival module);
        public event WaveComplete waveComplete;
        
        public delegate void SurvivalEvent();
        public delegate void RewardEvent(object reward);
        public event RewardEvent RewardSpawned;
        public event RewardEvent RewardChosen;
        public event SurvivalEvent AllRewardsSpawned;
        
        [Title("Texts")]
        [Dropdown(nameof(GetAllTextGroupID))]
        public string textGroupId;

        [Dropdown(nameof(GetAllTextId))]
        public string textNextWaveId;

        [Dropdown(nameof(GetAllTextId))]
        public string textFightId;

        [Dropdown(nameof(GetAllTextId))]
        public string textWaveId;

        public TriDropdownList<string> GetAllTextGroupID() => Catalog.GetTextData().GetDropdownAllTextGroups();

        public TriDropdownList<string> GetAllTextId() => Catalog.GetTextData().GetDropdownAllTexts(textGroupId);

        public TriDropdownList<string> GetAllContainerID() => Catalog.GetDropdownAllID(Category.Container);

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

        public TriDropdownList<string> GetAllItemID() => Catalog.GetDropdownAllID(Category.Item);

    }
}
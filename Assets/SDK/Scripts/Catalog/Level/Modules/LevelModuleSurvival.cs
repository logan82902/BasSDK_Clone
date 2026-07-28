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
    [DeclareBoxGroup("General")]
    [DeclareBoxGroup("Weights", Title = "Weighting")]
    [DeclareHorizontalGroup("Weights/Horiz")]
    [DeclareVerticalGroup("Weights/Horiz/Skills")]
    [DeclareVerticalGroup("Weights/Horiz/Rewards")]
    [DeclareBoxGroup("Catalog", Title = "Catalog Config")]
    [DeclareBoxGroup("Pillars", Title = "Reward Pillars")]
    [DeclareVerticalGroup("Pillars/Zone")]
    [DeclareHorizontalGroup("Pillars/Zone/AutoConfig")]
    [DeclareHorizontalGroup("Pillars/Zone/Teleport")]
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

        [Group("General")]
        public bool trueRandomInProgressive = false;
        public static Queue<CatalogData> forceSpawnDataQueue = new();

        [NullValueFix]
        [Group("General")]
        public Gradient shockGradient = Utils.CreateGradient(new Color(0.8f, 0.8f, 0.8f), new Color(0.6f, 0.6f, 0.6f));

        [Group("General")]
        [DropdownList(nameof(GetAllItemID))]
        public List<string> forceAllowedGrabbables = new List<string>();

        [Group("Weights/Horiz/Skills")]
        [Title("Skills")]
        [LabelText("Dual Tree Extra Weight")]
        public float dualTreeWeight = 0.4f;
        [Group("Weights/Horiz/Skills")]
        [LabelText("Non-Core Skill Extra Weight")]
        public float subSkillWeight = 0.6f;
        [Group("Weights/Horiz/Skills")]
        [InfoBox("Skill spawning weight multiplier based on tier as wave number increases. "
                 + "First element is for Tier 1, second is for Tier 2 etc. ")]
        public List<AnimationCurve> skillTierWeightPerWave;
        // = new() { AnimationCurve.Linear(0, 1, 20, 0.5f), AnimationCurve.Linear(0, 0.5f, 20, 1f), AnimationCurve.Linear(0, 0.1f, 20, 0.5f), };

        [Group("Weights/Horiz/Rewards")]
        [Title("Rewards")]
        public float weaponWeight = 4;
        [Group("Weights/Horiz/Rewards")]
        public float offensiveSkillWeight = 3;
        [Group("Weights/Horiz/Rewards")]
        public float miscSkillWeight = 1;
        [Group("Weights/Horiz/Rewards")]
        public float potionWeight = 1;
        [Group("Weights/Horiz/Rewards")]
        public float armorWeight = 1;
        [Group("Weights/Horiz/Rewards")]
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

        [Group("General")]
        [Title("Default Level Options")]
        public EquipmentSettings equipmentSettings = EquipmentSettings.None;
        [Group("General")]
        public RewardTypes rewardTypes;

        [Group("General")]
        [Dropdown(nameof(GetAllContainerID))]
        public string startingContainerId = "SurvivalStartingContainer";
        protected ContainerData startingContainer;

        [Group("Pillars")]
        public string rewardPillarAddress;

        [Group("Pillars")]
        public string raiseSoundAudioContainerAddress = "Bas.AudioGroup.Survival.Pillar.Raise.Stone";
        [Group("Pillars")]
        public string lowerSoundAudioContainerAddress = "Bas.AudioGroup.Survival.Pillar.Lower.Stone";

        [NonSerialized]
        public AudioContainer raiseSound;
        [NonSerialized]
        public AudioContainer lowerSound;

        [Group("Pillars")]
        public int mainPillarIndex = 0;
        [Group("Pillars/Zone/AutoConfig")]
        public bool autoConfigureTeleportZone = false;
        [Group("Pillars/Zone/AutoConfig")]
        public float autoConfigureTeleportBoundsSize = 10f;

        [JsonIgnore]
        [ShowInInspector]
        [Group("Pillars/Zone")]
        public bool hasPillarZone
        {
            get => pillarZone.HasValue;
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
        [Group("Pillars")]
        [Indent(1)]
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

        [Group("Pillars/Zone/Teleport")]
        public bool useCustomTeleportPoint = false;
        [Group("Pillars/Zone/Teleport")]
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
        [Group("Pillars")]
        public int rewardsToSpawn = 3;
        [Group("Pillars")]
        public bool normalizeItemScale = false;
        [Group("Catalog")]
        [Dropdown(nameof(GetAllEffectID))]
        public string getRewardEffectId = "GetReward";
        protected EffectData getRewardEffect;
        [Group("Catalog")]
        [Dropdown(nameof(GetAllEffectID))]
        public string victoryEffectId = "OutpostChestOpen";
        protected EffectData victoryEffectData;
        [Group("Catalog")]
        [Dropdown(nameof(GetAllEffectID))]
        public string lightningZapEffectId;
        [Group("Catalog")]
        [Dropdown(nameof(GetAllEffectID))]
        public string dropItemEffectId = "SurvivalDropItem";

        [Group("Catalog")]
        [Dropdown(nameof(GetAllEffectID))]
        public string rewardEffectId = "SurvivalRewardHighlight";
        [Group("Catalog")]
        [Dropdown(nameof(GetAllItemID))]
        public string skillItemId = "SkillOrbItem";
        [NonSerialized]
        public ItemData skillItemData;

        [Group("General")]
        [Title("Waves")]
        public float startDelay = 10;
        [Group("General")]
        public SurvivalModeLoopBehavior loop = SurvivalModeLoopBehavior.Loop;
        [Group("General")]
        public int wavesNumberForReward = 1;
        [Group("General")]
        public List<Waves> waves;

        public delegate void WaveComplete(LevelModuleSurvival module);
        public event WaveComplete waveComplete;
        
        public delegate void SurvivalEvent();
        public delegate void RewardEvent(object reward);
        public event RewardEvent RewardSpawned;
        public event RewardEvent RewardChosen;
        public event SurvivalEvent AllRewardsSpawned;

        [Group("Catalog")]
        [Title("Texts")]
        [Dropdown(nameof(GetAllTextGroupID))]
        public string textGroupId;

        [Group("Catalog")]
        [Dropdown(nameof(GetAllTextId))]
        public string textNextWaveId;

        [Group("Catalog")]
        [Dropdown(nameof(GetAllTextId))]
        public string textFightId;

        [Group("Catalog")]
        [Dropdown(nameof(GetAllTextId))]
        public string textWaveId;

        public TriDropdownList<string> GetAllTextGroupID() => Catalog.GetTextData().GetDropdownAllTextGroups();

        public TriDropdownList<string> GetAllTextId() => Catalog.GetTextData().GetDropdownAllTexts(textGroupId);

        public TriDropdownList<string> GetAllContainerID() => Catalog.GetDropdownAllID(Category.Container);

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

        public TriDropdownList<string> GetAllItemID() => Catalog.GetDropdownAllID(Category.Item);


        public override void DrawGizmos()
        {
            if (pillarZone.Value is Bounds bounds)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }
    }
}
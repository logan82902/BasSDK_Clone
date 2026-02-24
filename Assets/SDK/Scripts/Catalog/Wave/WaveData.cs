using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections;

using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(0)]
#endif
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Info", Title = "Info")]
    public class WaveData : CatalogData
    {
        [Group("General")]
        public string category = "Misc";

        [Group("General")]
        public string localizationId;

        [Group("General")]
        public string title = "Unknown";

        [Group("General")]
        [Multiline]
        public string description;

        public enum LoopBehavior
        {
            NoLoop,
            LoopSeamless,
            Loop,
        }

        [Group("General")]
        public LoopBehavior loopBehavior;

        [Group("General")]
        public int totalMaxAlive = 5;

        [Group("General")]
        public bool alwaysAvailable;

        [Group("General")]
        [ListDrawerSettings(AlwaysExpanded = true)]
        public List<string> waveSelectors;

        [Group("Info")]
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public int minTotalCount;
        [Group("Info")]
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public float maxTotalCount;
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, Draggable = false)]
        [HideReferencePicker]
        public List<WaveFaction> factions = new();

        [HideReferencePicker]
        [OnValueChanged(nameof(RefreshInfo))]
        public List<Group> groups;

        public List<WaveModule> modules;

        public override int GetCurrentVersion()
        {
            return 2;
        }

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            if (Application.isPlaying) ClearNullOrEmptyRefGroups();
            RefreshInfo();
            if (modules is { Count: > 0 })
            {
                for (var i = 0; i < modules.Count; i++)
                {
                    modules[i].OnCatalogRefresh();
                }
            }
        }

        public void ClearNullOrEmptyRefGroups()
        {
            for (int i = groups.Count - 1; i >= 0; i--)
            {
                if (groups[i].referenceID.IsNullOrEmptyOrWhitespace())
                {
                    Debug.LogWarning($"Null group ({groups[i].mergeKey}) ID found in wave data {id}, removing from wave data");
                    groups.RemoveAt(i);
                }
            }
        }

        public override IEnumerator OnCatalogRefreshCoroutine()
        {
            yield return base.OnCatalogRefreshCoroutine();
            if (modules is { Count: > 0 })
            {
                for (var i = 0; i < modules.Count; i++)
                {
                    yield return modules[i].OnCatalogRefreshCoroutine();
                }
            }
        }

        private void RefreshInfo()
        {
            minTotalCount = 0;
            maxTotalCount = 0;
            Dictionary<int, WaveFaction> foundFactions = new Dictionary<int, WaveFaction>();
            foreach (WaveFaction faction in factions)
            {
                faction.factionName = Catalog.gameData.GetFaction(faction.factionID).name;
                if (!foundFactions.TryAdd(faction.factionID, faction))
                {
                    Debug.LogError($"Duplicate faction {faction.factionID} in wave {id}");
                }
            }
            List<int> factionsInWave = new List<int>();
            List<WaveFaction> newFactions = new List<WaveFaction>();
            if (groups != null)
            {
                for (var i = 0; i < groups.Count; i++)
                {
                    groups[i].waveData = this;
                    minTotalCount += groups[i].minMaxCount.x;
                    maxTotalCount += groups[i].minMaxCount.y;
                    groups[i].OnCatalogRefresh();
                    groups[i].GetFactions(factionsInWave);
                }
            }
            factionsInWave.Sort();
            if (factionsInWave.Count > 0)
            {
                foreach (int factionID in factionsInWave)
                {
                    if (foundFactions.TryGetValue(factionID, out WaveFaction existingFaction))
                    {
                        newFactions.Add(existingFaction);
                        foundFactions.Remove(factionID);
                    }
                    else
                    {
                        newFactions.Add(new WaveFaction(Catalog.gameData.GetFaction(factionID), Mathf.RoundToInt(totalMaxAlive / factionsInWave.Count)));
                    }
                }
            }
            factions = newFactions;
        }

        public int GetMaxAlive()
        {
            return Mathf.Min(totalMaxAlive, Catalog.gameData.platformParameters.maxWaveAlive);
        }


        [Serializable]
        [DeclareHorizontalGroup("Box/Split")]
        [DeclareBoxGroup("Box/Split/Faction", Title = "Faction")]
        [DeclareHorizontalGroup("Box/Split/Faction/Horiz")]
        [DeclareBoxGroup("Box/Split/Faction health multiplier", Title = "Faction health multiplier")]
        [DeclareBoxGroup("Box/Split/Faction max alive", Title = "Faction max alive")]
        public class WaveFaction
        {
            [Group("Box/Split/Faction/Horiz")]
            [ReadOnly]
            [HideLabel]
            public int factionID;
            [Group("Box/Split/Faction/Horiz")]
            [NonSerialized]
            [ShowInInspector]
            [ReadOnly]
            [HideLabel]
            public string factionName;
            
            [Group("Box/Split/Faction max alive")]
            [HideLabel]
            public int factionMaxAlive = 5;

            public WaveFaction() { }

            public WaveFaction(GameData.Faction faction, int factionDefaultMax)
            {
                factionID = faction.id;
                factionName = faction.name;
                factionMaxAlive = factionDefaultMax;
            }
        }

        public class SpawnData
        {
            public CreatureData data;
            public Group spawnGroup;

        }

        [Serializable]
        [DeclareFoldoutGroup("Box", Title = "$" + nameof(label), Expanded = true)]
        [DeclareHorizontalGroup("Box/Split", Sizes = new float[] { 300f, 0f, 0f, 0f, 0f, 0f, 200f, 0f, 350f })]
        [DeclareBoxGroup("Box/Split/Reference", Title = "Reference")]
        [DeclareHorizontalGroup("Box/Split/Reference/Horiz")]
        [DeclareBoxGroup("Box/Split/OverrideFaction", Title = "Override Faction")]
        [DeclareHorizontalGroup("Box/Split/OverrideFaction/Horiz", Sizes = new float[] { 15f, 0f })]
        [DeclareBoxGroup("Box/Split/OverrideContainer", Title = "Override Container")]
        [DeclareHorizontalGroup("Box/Split/OverrideContainer/Horiz", Sizes = new float[] { 15f, 0f })]
        [DeclareBoxGroup("Box/Split/OverrideBrain", Title = "Override Brain")]
        [DeclareHorizontalGroup("Box/Split/OverrideBrain/Horiz", Sizes = new float[] { 15f, 0f })]
        [DeclareBoxGroup("Box/Split/MaxMelee", Title = "Override Max Melee")]
        [DeclareHorizontalGroup("Box/Split/MaxMelee/Horiz", Sizes = new float[] { 15f, 0f })]
        [DeclareBoxGroup("Box/Split/HealthMult", Title = "Group health mult")]
        [DeclareBoxGroup("Box/Split/Spawning", Title = "Spawning Min/Max")]
        [DeclareBoxGroup("Box/Split/SpawnPoint", Title = "Spawn Point")]
        [DeclareHorizontalGroup("Box/Split/SpawnPoint/Horiz", Sizes = new float[] { 15f, 0f })]
        [DeclareBoxGroup("Box/Split/PrereqGroup", Title = "Prereq Group")]
        [DeclareHorizontalGroup("Box/Split/PrereqGroup/Horiz", Sizes = new float[] { 15f, 120f, 0f })]
        public class Group
        {
            [NonSerialized]
            [JsonIgnore]
            public WaveData waveData;
            [JsonIgnore]
            public string label => $"Group Index {waveData?.groups.IndexOf(this)}";

            [JsonMergeKey(MergeKeyValueProvider = nameof(GetMergeKey))]
            [HideInInspector]
            public string mergeKey = "";

            [Group("Box/Split/Reference/Horiz")]
            [HideLabel]
            [EnumToggleButtons]
            [PropertyOrder(0)]
            public Reference reference = Reference.Creature;
            [Group("Box/Split/Reference/Horiz")]
            [Dropdown(nameof(GetAllCreatureOrTableID))]
            [HideLabel]
            [OnValueChanged(nameof(RefreshInfo))]
            [PropertyOrder(1)]
            public string referenceID;

            [Group("Box/Split/OverrideFaction/Horiz")]
            [HideLabel]
            [OnValueChanged(nameof(RefreshInfo))]
            [PropertyOrder(2)]
            public bool overrideFaction;
            [Group("Box/Split/OverrideFaction/Horiz")]
            [EnableIf(nameof(overrideFaction), true)]
            [Dropdown(nameof(GetAllFactionID), Searchable = false)]
            [HideLabel]
            [OnValueChanged(nameof(RefreshInfo))]
            [PropertyOrder(3)]
            public int factionID = 0;

            [Group("Box/Split/OverrideContainer/Horiz")]
            [HideLabel]
            [PropertyOrder(4)]
            public bool overrideContainer;
            [Group("Box/Split/OverrideContainer/Horiz")]
            [EnableIf(nameof(overrideContainer), true)]
            [Dropdown(nameof(GetAllContainerID))]
            [HideLabel]
            [PropertyOrder(5)]
            public string overrideContainerID;

            [Group("Box/Split/OverrideBrain/Horiz")]
            [HideLabel]
            [PropertyOrder(6)]
            public bool overrideBrain;
            [Group("Box/Split/OverrideBrain/Horiz")]
            [EnableIf(nameof(overrideBrain), true)]
            [Dropdown(nameof(GetAllBrainID))]
            [HideLabel]
            [PropertyOrder(7)]
            public string overrideBrainID;

            [Group("Box/Split/MaxMelee/Horiz")]
            [HideLabel]
            [PropertyOrder(8)]
            public bool overrideMaxMelee;
            [Group("Box/Split/MaxMelee/Horiz")]
            [HideLabel]
            [EnableIf(nameof(overrideMaxMelee), true)]
            [PropertyOrder(9)]
            public int overrideMaxMeleeCount;

            [Group("Box/Split/HealthMult")]
            [HideLabel]
            [PropertyOrder(10)]
            public float groupHealthMultiplier = 1f;

            [Group("Box/Split/Spawning")]
            [HideLabel]
            [MinMaxSlider(1, 10, true)]
            [OnValueChanged(nameof(RefreshInfo))]
            [PropertyOrder(11)]
            public Vector2Int minMaxCount = new(1, 1);

            [Group("Box/Split/SpawnPoint/Horiz")]
            [HideLabel]
            [ShowInInspector]
            [NonSerialized]
            [JsonIgnore]
#if UNITY_EDITOR
            [OnValueChanged(nameof(SetRandSpawn))]
#endif
            [PropertyOrder(12)]
            public bool set = false;
            [Group("Box/Split/SpawnPoint/Horiz")]
            [HideLabel]
            [EnableIf(nameof(set))]
            [PropertyOrder(13)]
            public int spawnPointIndex = -1;

            [Group("Box/Split/PrereqGroup/Horiz")]
            [HideLabel]
            [ShowInInspector]
            [NonSerialized]
            [JsonIgnore]
#if UNITY_EDITOR
            [OnValueChanged(nameof(SetPrereqGroupIndex))]
#endif
            [PropertyOrder(14)]
            public bool hasPrereq = false;
            [Group("Box/Split/PrereqGroup/Horiz")]
            [EnableIf(nameof(hasPrereq))]
            [LabelText("Group Index")]
            [AutoLabelWidth]
            [PropertyOrder(15)]
            public int prereqGroupIndex = -1;
            [Group("Box/Split/PrereqGroup/Horiz")]
            [EnableIf(nameof(hasPrereq))]
            [LabelText("Max Alive Remaining")]
            [AutoLabelWidth]
            [PropertyOrder(16)]
            public int prereqMaxRemainingAlive = 0;

            [NonSerialized]
            [JsonIgnore]
            public Group prereqGroup;

            public enum Reference
            {
                Creature,
                Table,
            }

            public static string GetMergeKey()
            {
                return Guid.NewGuid().ToString().Substring(0, 8);
            }

            public int GetMaxThreshold()
            {
                if (waveData != null)
                {
                    Group conditionStep = waveData.groups.ElementAtOrDefault(prereqGroupIndex);
                    if (conditionStep != null)
                    {
                        return conditionStep.minMaxCount.x - 1;
                    }
                }
                return 4;
            }

#if UNITY_EDITOR
            public void SetPrereqGroupIndex()
            {
                if (!hasPrereq)
                {
                    prereqGroupIndex = -1;
                    prereqMaxRemainingAlive = 0;
                }
                else
                {
                    if (prereqGroupIndex == -1)
                    {
                        prereqGroupIndex = -1;
                    }
                }
            }

            public void SetRandSpawn()
            {
                if (!set) spawnPointIndex = -1;
                else
                {
                    if (spawnPointIndex == -1) spawnPointIndex = 0;
                }
            }
#endif

            private void RefreshInfo()
            {
                waveData?.RefreshInfo();
            }

            public void GetFactions(List<int> existing)
            {
                if (overrideFaction)
                {
                    if (!existing.Contains(factionID)) existing.Add(factionID);
                }
                else
                {
                    if (referenceID == "None") return;
                    if (reference == Reference.Creature)
                    {
                        CreatureData creatureData = Catalog.GetData<CreatureData>(referenceID);
                        if (!existing.Contains(creatureData.factionId)) existing.Add(creatureData.factionId);
                    }
                    else
                    {
                        if (Catalog.TryGetData<CreatureTable>(referenceID, out var creatureTable))
                        {
                            foreach (int factionID in creatureTable.GetFactionIDs())
                            {
                                if (!existing.Contains(factionID)) existing.Add(factionID);
                            }
                        } else
                        {
                            Debug.LogError($"Could not find creature table {referenceID} for wave {waveData.id}");
                        }
                    }
                }
            }


            public TriDropdownList<string> GetAllCreatureOrTableID() => Catalog.GetDropdownAllID(reference == Reference.Creature ? Category.Creature : Category.CreatureTable);

            public TriDropdownList<string> GetAllContainerID() => Catalog.GetDropdownAllID(Category.Container);

            public TriDropdownList<string> GetAllBrainID() => Catalog.GetDropdownAllID(Category.Brain);

            public TriDropdownList<int> GetAllFactionID() => Catalog.gameData.GetFactions();

            public void OnCatalogRefresh()
            {
                if (mergeKey.IsNullOrEmptyOrWhitespace())
                {
                    mergeKey = GetMergeKey();
                }
            }
        }

        public void OnWaveStart(WaveSpawner waveSpawner)
        {
            if (modules is not { Count: > 0 }) return;
            for (var i = 0; i < modules.Count; i++)
            {
                try
                {
                    modules[i]?.OnWaveStart(waveSpawner);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        
        public void OnWaveEnd(WaveSpawner waveSpawner)
        {
            if (modules is not { Count: > 0 }) return;
            for (var i = 0; i < modules.Count; i++)
            {
                try
                {
                    modules[i]?.OnWaveEnd(waveSpawner);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public void OnSpawnCreature(WaveSpawner waveSpawner, Creature spawned)
        {
            if (modules is not { Count: > 0 }) return;
            for (var i = 0; i < modules.Count; i++)
            {
                try
                {
                    modules[i]?.OnSpawnCreature(waveSpawner, spawned);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}
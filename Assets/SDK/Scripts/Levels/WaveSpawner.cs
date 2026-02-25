using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Levels/WaveSpawner.html")]
    [AddComponentMenu("ThunderRoad/Levels/Spawners/Waves Spawner")]
    public class WaveSpawner : MonoBehaviour
    {
        public enum ReferenceType
        {
            None,
            EnemyConfig
        }

        public enum EnemyConfigType
        {
            MeleeOnlyStd,
            MeleeFocusedStd,
            MixedStd,
            RangedFocusedStd,
            MeleeOnlyArena,
            MeleeFocusedArena,
            MixedArena,
            RangedFocusedArena,
            MeleeOnlyEnd,
            MeleeFocusedEnd,
            MixedEnd,
            RangedFocusedEnd
        }

        public static List<WaveSpawner> instances = new List<WaveSpawner>();

        public ReferenceType referenceType = ReferenceType.EnemyConfig;
        [ShowIf("referenceType", ReferenceType.EnemyConfig)]
        public EnemyConfigType enemyConfigType;

        [Tooltip("When ticked, the wave spawner will ignore the dungeon limits and spawn NPCs regardless of the dungeon limit. This is useful for playing a dungeon room in sandbox mode")]
        public bool ignoreDungeonLimits = false;
        [Tooltip("List of spawn points, where NPC will spawn during a wave start.")]
        public List<Transform> spawns = new List<Transform>();
        [Tooltip("Adds FleePoint to the Spawn Point locations. When wave ends, alive NPC will move to these spots to despawn.")]
        public bool addAsFleepointOnStart = true;

        [Tooltip("Begins wave apon loading in to the level.")]
        public bool beginWaveOnStart;
        [Tooltip("Delay of Wave activation on start.")]
        public float beginWaveOnStartDelay;
        [Tooltip("ID of wave that begins on start")]
        [ShowIf("referenceType", ReferenceType.None)]
        public string startWaveId;
        [Tooltip("When ticked, items and dead bodies will despawn when the wave starts. When disabled, bodies and items do not despawn on start.")]
        public bool cleanBodiesAndItemsOnWaveStart = true;

        [Dropdown(nameof(GetAllFactionID), Searchable = false)]
        [Tooltip("ID of the faction that is ignored by the NPCs of this wave.")]
        public int ignoredFactionId = -1;
        [Tooltip("Determines how often the spawner checks to see if it can spawn a new creature.")]
        public float updateRate = 2;
        [Tooltip("Determines the delay between spawning creatures at the same spawn point.")]
        public float sameSpawnDelay = 3;
        [Tooltip("Delay between NPC spawns.")]
        public float spawnDelay = 2;
        [Tooltip("Automatically end the wave if no action after a specific duration.")]
        public float inactivityEndTimer = 60f;
        
        [Header("Event")]
        public UnityEvent OnWaveBeginEvent = new UnityEvent();
        public UnityEvent OnWaveAnyEndEvent = new UnityEvent();
        public UnityEvent OnWaveWinEvent = new UnityEvent();
        public UnityEvent OnWaveLossEvent = new UnityEvent();
        public UnityEvent OnWaveCancelEvent = new UnityEvent();
        public UnityEvent OnWaveLoopEvent = new UnityEvent();
        public static UnityEvent<WaveSpawner> OnWaveSpawnerEnabledEvent = new UnityEvent<WaveSpawner>();
        public static UnityEvent<WaveSpawner> OnWaveSpawnerDisabledEvent = new UnityEvent<WaveSpawner>();
        public static UnityEvent<WaveSpawner> OnWaveSpawnerStartRunningEvent = new UnityEvent<WaveSpawner>();
        public static UnityEvent<WaveSpawner> OnWaveSpawnerStopRunningEvent = new UnityEvent<WaveSpawner>();


        #region Exposed SDK methods
        public void StartWave(string waveId)
        {
        }

        public void CancelWave()
        {
        }

        public void StopWave(bool success)
        {
        }

        public void Clean()
        {
        }
        #endregion

        public TriDropdownList<int> GetAllFactionID()
        {
            if (Catalog.gameData == null) return null;
            return Catalog.gameData.GetFactions();
        }

        public TriDropdownList<string> GetAllWaveID() => Catalog.GetDropdownAllID(Category.Wave);
    }
}
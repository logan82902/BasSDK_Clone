using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    public class LevelModuleAddressableSpawner : LevelModule
    {
        public List<BasePrefabSpawnConfig> prefabs;
        protected List<GameObject> assets;
    }

    [Serializable]
    public abstract class BasePrefabSpawnConfig
    {
        public string address;
        
        public bool spawnAtLevelReference;
        
        [ShowIf(nameof(spawnAtLevelReference))]
        public string referenceName;
        
        [HideIf(nameof(spawnAtLevelReference))]
        public Vector3 position;
        [HideIf(nameof(spawnAtLevelReference))]
        public Vector3 rotation;
        [HideIf(nameof(spawnAtLevelReference))]
        public Vector3 scale = Vector3.one;
        [HideIf(nameof(spawnAtLevelReference))]
        public bool spawnInsideArea = true;

    }

    public class PrefabSpawnConfig : BasePrefabSpawnConfig {}

    public abstract class PrefabSpawnConfig<T> : BasePrefabSpawnConfig
    {
    }

    public class WaveSpawnerSpawnConfig : PrefabSpawnConfig<UIWaveSpawner>
    {
        public bool useWaveSpawnerAtReference = false;
        [ShowIf(nameof(useWaveSpawnerAtReference))]
        public string waveSpawnerReference;
        [HideIf(nameof(useWaveSpawnerAtReference))]
        public int waveSpawnerIndex = 0;
    }

    public class ItemSpawnerSpawnConfig : PrefabSpawnConfig<UIItemSpawner>
    {
        public bool overrideSpawnPosition = false;
        [ShowIf(nameof(overrideSpawnPosition))]
        public Vector3 spawnPosition;
        [ShowIf(nameof(overrideSpawnPosition))]
        public Vector3 spawnRotation;

    }
}
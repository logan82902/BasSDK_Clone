using Newtonsoft.Json;
using System;
using System.Collections;
using TriInspector;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace ThunderRoad
{
    [System.Serializable]
    public class ItemModuleCrossbow : ItemModuleBow
    {
        [Group("Force")]
        public float pullRatioToLock = 0.95f;

        [Group("Audio")]
        public string audioLockAddress;
        [Group("Audio")]
        public string audioSpanLoopAddress;

    }
}

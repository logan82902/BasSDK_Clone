using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    [System.Serializable]
    [DeclareBoxGroup("Audio", Title = "Audio")]
    public class ItemModuleCrossbow : ItemModule
    {
        public float velocityMultiplier = 1.5f;
        public float baseShootVelocity = 20f;
        public float stringSpring = 1000;

        public static bool forceAutoSpawnBolt;
        public bool autoSpawnBolt;
        [Dropdown(nameof(GetAllItemID))]
        public string boltProjectileID;

        public float pullRatioToLock = 0.95f;

        [Group("Audio")]
        public string audioPullAddress;

        [Group("Audio")]
        public string audioLockAddress;

        [Group("Audio")]
        public string audioShootAddress;

        public TriDropdownList<string> GetAllItemID() => Catalog.GetDropdownAllID(Category.Item);

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

    }
}

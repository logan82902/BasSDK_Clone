using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    [System.Serializable]
    [DeclareBoxGroup("Audio", Title = "Audio")]
    [DeclareBoxGroup("AI", Title = "AI")]
    public class ItemModuleBow : ItemModule
    {
        public float velocityMultiplier = 1.5f;
        public float stringSpring = 500;

        public static bool forceAutoSpawnArrow;
        public bool autoSpawnArrow;

        public bool spawnArrow => autoSpawnArrow || forceAutoSpawnArrow;
        [Dropdown(nameof(GetAllItemID))]
        public string arrowProjectileID;

        public float audioShootMinPull = 0.1f;
        public float audioDrawPullSpeed = 0.02f;

        [Group("Audio")]
        public string audioStringAddress;

        [Group("Audio")]
        public string audioGroupDrawAddress;

        [Group("Audio")]
        public string audioGroupShootAddress;

        [Group("AI")]
        public float maxPullRatioAI = 0.75f;

        public TriDropdownList<string> GetAllItemID() => Catalog.GetDropdownAllID(Category.Item);

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

    }
}

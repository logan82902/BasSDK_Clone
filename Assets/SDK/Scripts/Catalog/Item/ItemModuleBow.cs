using Newtonsoft.Json;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    [System.Serializable]
    [DeclareBoxGroup("Force")]
    [DeclareBoxGroup("Arrow")]
    [DeclareBoxGroup("Audio")]
    [DeclareBoxGroup("AI")]
    public class ItemModuleBow : ItemModule
    {
        [Group("Force")]
        public float velocityMultiplier = 1.5f;
        [Group("Force")]
        public float stringSpring = 500;
        [Group("Force")]
        public float firedBounciness = 0.33f;
        [Group("Force")]
        public float audioShootMinPull = 0.1f;

        public static bool forceAutoSpawnArrow;
        [Group("Arrow")]
        public bool autoSpawnArrow;
        [JsonIgnore]
        public bool spawnArrow => autoSpawnArrow || forceAutoSpawnArrow;
        [Group("Arrow")]
        [Dropdown(nameof(GetAllItemID))]
        public string arrowProjectileID;
        [Group("Arrow")]
        public float arrowDamageMultiplier = 1f;
        [Group("Arrow")]
        public float arrowForceMultiplier = 1f;

        [Group("Audio")]
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

        public override void OnItemDataRefresh(ItemData data)
        {
            base.OnItemDataRefresh(data);
            if (audioStringAddress.IsNullOrEmptyOrWhitespace()) Debug.LogWarning($"Address is empty for String Audio on {data.id}, the audio source will not be updated.");
            if (audioGroupDrawAddress.IsNullOrEmptyOrWhitespace()) Debug.LogWarning($"Address is empty for Draw AudioGroup on {data.id}, no AudioContainer will be assigned to the bow string.");
            if (audioGroupShootAddress.IsNullOrEmptyOrWhitespace()) Debug.LogWarning($"Address is empty for Shoot AudioGroup on {data.id}, no AudioContainer will be assigned to the bow string.");
        }

    }
}

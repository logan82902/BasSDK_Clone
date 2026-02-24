using UnityEngine;
using System;
using System.Collections;
using ThunderRoad.AI.Action;
using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BrainModuleFirearm : BrainModuleRanged
    {
        public float maxShootAngle = 1;

        [JsonIgnore]
        public ItemModuleAI.RangedWeaponData mainFirearmData { get; protected set; } = null;
        [JsonIgnore]
        public Transform mainHandAnchor { get; protected set; } = null;

        [JsonIgnore]
        public ItemModuleAI.RangedWeaponData offhandFirearmData { get; protected set; } = null;
        [JsonIgnore]
        public Transform offhandAnchor { get; protected set; } = null;

        [JsonIgnore]
        public Transform sameWeaponAnchor { get; protected set; } = null;

    }
}
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ThunderRoad.Manikin;
using TriInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ThunderRoad
{
    [System.Serializable]
    public class ItemModuleWardrobe : ItemModule
    {
        public enum CastShadows { None = 0, PlayerOnly = 1, PlayerAndNPC = 2 }
        public Equipment.WardRobeCategory category = Equipment.WardRobeCategory.Apparel;
        public CastShadows castShadows = CastShadows.None;
        public List<CreatureWardrobe> wardrobes = new List<CreatureWardrobe>();
        public bool isMetal;
        public int armorSoundEffectPriority;
        public string armorSoundEffectID;

        [Serializable]
        public class CreatureWardrobe
        {
            [Dropdown(nameof(GetAllCreatureName))]
            [JsonMergeKey]
            public string creatureName;
            public string wardrobeDataAddress;
            
            private int _creatureNameHash = -1;
            [JsonIgnore]
            public int creatureNameHash {
                get
                {
                    if (_creatureNameHash == -1)
                    {
                        _creatureNameHash = Animator.StringToHash(creatureName);
                    }
                    return _creatureNameHash;
                }
            }
            
            [ShowInInspector]
            [ReadOnly]
            [NonSerialized]
            public ManikinWardrobeData manikinWardrobeData;

            public List<PartBlendShapeActivator> runtimeBlendshapeActivators;

            public CreatureWardrobe Clone()
            {
                return MemberwiseClone() as CreatureWardrobe;
            }

            public TriDropdownList<string> GetAllCreatureName()
            {
                TriDropdownList<string> dropdownList = new();
                foreach (CreatureData creatureData in Catalog.GetDataList(Category.Creature))
                {
                    if (dropdownList.Exists(d => d.Value == creatureData.name))
                    {
                        continue;
                    }
                    dropdownList.Add(creatureData.name, creatureData.name);
                }
                return dropdownList;
            }
        }

        [Serializable]
        [AutoLabelWidth]
        [DeclareHorizontalGroup("Horiz", Sizes = new float[] { 0f, 0f, 120f, 120f, 120f})]
        public class PartBlendShapeActivator
        {
            [Group("Horiz")]
            public string wardrobeDataAddress;
            [Group("Horiz")]
            public string meshNameRegex = ".*";
            [Group("Horiz")]
            public int shapeIndex = 0;
            [Group("Horiz")]
            public int defaultValue = 0;
            [Group("Horiz")]
            public int shapeValue = 100;
        }

    }
}

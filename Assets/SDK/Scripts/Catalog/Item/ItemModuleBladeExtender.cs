using Newtonsoft.Json;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    public class ItemModuleBladeExtender : ItemModule
    {
        [Dropdown(nameof(GetAllEffectID))]
        public string openEffectId;
        [JsonIgnore]
        public EffectData openEffectData;
        
        [Dropdown(nameof(GetAllEffectID))]
        public string closeEffectId;
        [JsonIgnore]
        public EffectData closeEffectData;
        public float openCloseSpeed = 10f;
        public bool extensionPierceJoint = true;

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

    }

}
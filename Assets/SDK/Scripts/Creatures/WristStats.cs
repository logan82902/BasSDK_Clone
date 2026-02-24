using System;
using UnityEngine;
using TriInspector;

namespace ThunderRoad
{
    [AddComponentMenu("ThunderRoad/Creatures/Wrist stats")]
    public class WristStats : ThunderBehaviour
    {
        public float showDistance = 0.31f;
        public float showAngle = 40.0f;
        public float distanceFromEyes = 0.25f;
        public float minComfortableDistance = 0.2f;
        
        [Dropdown(nameof(GetAllEffectID))]
        public string healthEffectId;
        [Dropdown(nameof(GetAllEffectID))]
        public string manaEffectId;
        [Dropdown(nameof(GetAllEffectID))]
        public string focusEffectId;

        [NonSerialized]
        public Creature creature;
        [NonSerialized]
        public bool isShown = true;
        
        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

    }
}
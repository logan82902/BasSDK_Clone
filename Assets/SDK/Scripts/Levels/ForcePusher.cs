using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    public class ForcePusher : ThunderBehaviour
    {
        public enum RadialPushMode
        {
            Proximity,
            Absolute
        }

        public float radius = 4f;
        public float force = 10f;
        public float upwardsModifier = 1f;
        public ForceMode forceMode = ForceMode.VelocityChange;
        public GameObject radiusParticles;
        public float delay = 0.5f;

        [Tooltip("How force is applied based on distance from the center. Proximity has force falloff, Absolute will apply the same force regardless of distance.")]
        public RadialPushMode pushMode = RadialPushMode.Absolute;

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);
        [Dropdown(nameof(GetAllEffectID))]
        public string effectId = "Shockwave";
 // ProjectCore
    }
}

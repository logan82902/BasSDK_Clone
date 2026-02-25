using UnityEngine;
using System.Collections.Generic;

using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Effects/HingeEffect.html")]
    [AddComponentMenu("ThunderRoad/Effects/Hinge effect")]
    public class HingeEffect : ThunderBehaviour
    {
        [Dropdown(nameof(GetAllEffectID), AppendToField = true)]
        public string effectId;
        public float minTorque = 5;
        public float maxTorque = 12;
        public HingeJoint joint;

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

    }
}

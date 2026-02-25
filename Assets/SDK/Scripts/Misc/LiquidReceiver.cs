using UnityEngine;
using System.Collections.Generic;

using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Items/LiquidReceiver.html")]
    [AddComponentMenu("ThunderRoad/Liquid receiver")]
    public class LiquidReceiver : MonoBehaviour
    {
        [Dropdown(nameof(GetAllEffectID), AppendToField = true)]
        public string drinkEffectId;

        public float maxAngle = 30;
        public float stopDelay = 0.1f;
        public float effectRate = 1f;

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

    }
}
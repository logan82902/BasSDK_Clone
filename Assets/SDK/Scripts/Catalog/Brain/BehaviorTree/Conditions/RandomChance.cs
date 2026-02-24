using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Condition
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class RandomChance : ConditionNode
    {
        [Range(0.01f, 0.99f)]
        public float chance = 0.5f;
        public float valueUpdateFrequency = 10f;

    }
}

using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Condition
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BlackboardTimeValue : ConditionNode
    {
        public string variableName;
        public BlackboardValue.Comparator variableComparator;
        public float timeDiff;

    }
}
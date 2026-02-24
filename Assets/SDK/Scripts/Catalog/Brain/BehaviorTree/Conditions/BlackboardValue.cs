using TriInspector;

namespace ThunderRoad.AI.Condition
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BlackboardValue : ConditionNode
    {
        public string variableName;
        public VariableType variableType;
        [EnableIf(nameof(variableType), VariableType.Float)]
        public Comparator variableComparator;
        public float comparedValue;

        public enum Comparator
        {
            Equal,
            GreaterThan,
            GreaterThanOrEqual,
            LowerThan,
            LowerThanOrEqual,
        }

        public enum VariableType
        {
            Bool,
            Float,
        }

    }
}
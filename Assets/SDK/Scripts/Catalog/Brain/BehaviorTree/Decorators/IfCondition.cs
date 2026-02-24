using System.Collections;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad.AI.Decorator
{
    [AutoLabelWidth(AddedWidth = 5f)]
    [DeclareVerticalGroup("Vertical")]
    [DeclareBoxGroup("Vertical/Conditions", Title = "Conditions")]
    [DeclareHorizontalGroup("Vertical/Conditions/Horiz")]
    public class IfCondition : DecoratorNode
    {
        [Group("Vertical/Conditions")]
        [PropertyOrder(0)]
        public Operation operation = Operation.AND;

        public enum Operation
        {
            AND,
            OR,
        }

        [ShowInInspector]
        [PropertyOrder(1)]
        [Group("Vertical/Conditions/Horiz")]
        [ListDrawerSettings(DontForceInline = true, AlwaysExpanded = true)]
        public List<ConditionNode> ifConditions = new();
        [ShowInInspector]
        [PropertyOrder(2)]
        [Group("Vertical/Conditions/Horiz")]
        [ListDrawerSettings(DontForceInline = true, AlwaysExpanded = true)]
        public List<ConditionNode> ifNotConditions = new();

        public IfCondition() : base() { }

    }
}

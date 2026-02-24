using System.Collections;
using TriInspector;

namespace ThunderRoad.AI
{
    [AutoLabelWidth(AddedWidth = 5f)]
    [DeclareVerticalGroup("Vertical")]
    [DeclareBoxGroup("Vertical/Child", Title = "Child Node")]
    public abstract class DecoratorNode : Node
    {
        [ShowInInspector]
        [PropertyOrder(3)]
        [InlineProperty]
        [Group("Vertical/Child")]
        public Node child;

        public DecoratorNode() { }

    }

}
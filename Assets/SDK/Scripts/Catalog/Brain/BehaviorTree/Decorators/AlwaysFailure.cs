using TriInspector;

namespace ThunderRoad.AI.Decorator
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class AlwaysFailure : DecoratorNode
    {
        public AlwaysFailure() : base() { }

    }

}

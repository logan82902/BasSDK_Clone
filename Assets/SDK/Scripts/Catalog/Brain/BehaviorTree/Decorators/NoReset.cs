using TriInspector;

namespace ThunderRoad.AI.Decorator
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class NoReset : DecoratorNode
    {
        public NoReset() : base() { }
    }
}
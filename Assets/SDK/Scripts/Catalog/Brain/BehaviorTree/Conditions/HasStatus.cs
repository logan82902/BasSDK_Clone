using TriInspector;

namespace ThunderRoad.AI.Condition
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class HasStatus : ConditionNode
    {
        public bool any = false;
        public string statusID;

    }
}

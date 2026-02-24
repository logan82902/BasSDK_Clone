using TriInspector;

namespace ThunderRoad.AI.Condition
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class IsUnarmed : ConditionNode
    {
        public bool isMainHandCheck = false;
    }
}
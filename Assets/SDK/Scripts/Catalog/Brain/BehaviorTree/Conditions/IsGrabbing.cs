using TriInspector;

namespace ThunderRoad.AI.Condition
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public partial class IsGrabbing : ConditionNode
    {
        public bool weaponOnly = false;
        public Side hand = Side.Right;
        public ItemModuleAI.WeaponClass weaponClass;
        public Filter weaponClassFilter = Filter.Equal;

        public enum Filter
        {
            Equal,
            NotEqual,
        }

    }
}
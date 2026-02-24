using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class DrawItem : CanDrawItem
    {
        [DisableIf(nameof(drawRightWeapon))]
        [LabelWidth(130)]
        public bool holsterRightItem = true;
        [DisableIf(nameof(drawLeftWeapon))]
        [LabelWidth(130)]
        public bool holsterLeftItem = false;
        public bool waitForFinished = false;
    }
}

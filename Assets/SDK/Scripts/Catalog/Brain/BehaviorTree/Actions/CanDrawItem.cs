using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class CanDrawItem : ActionNode
    {

        public string targetTransformVariableName = "";

        [LabelWidth(130)]
        public bool drawRightWeapon;
        [EnableIf(nameof(drawRightWeapon))]
        [LabelWidth(130)]
        public Equipment.WeaponDrawInfo rightInfo = new();

        [LabelWidth(130)]
        public bool drawLeftWeapon;
        [EnableIf(nameof(drawLeftWeapon))]
        [LabelWidth(130)]
        public Equipment.WeaponDrawInfo leftInfo = new();

    }
}

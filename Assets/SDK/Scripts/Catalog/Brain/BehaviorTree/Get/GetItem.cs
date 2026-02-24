using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TriInspector;

namespace ThunderRoad.AI.Get
{
    [DeclareHorizontalGroup("Horiz1", Sizes = new float[] { 155f, 0f })]
    [DeclareHorizontalGroup("Horiz2", Sizes = new float[] { 155f, 0f })]
    [DeclareHorizontalGroup("Horiz3", Sizes = new float[] { 155f, 0f })]
    [DeclareHorizontalGroup("Horiz4", Sizes = new float[] { 155f, 0f })]
    [DeclareHorizontalGroup("Horiz5", Sizes = new float[] { 155f, 0f })]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GetItem : ActionNode
    {
        public float searchRadius = 30f;
        public bool checkCanSeeItem = true;
        [Group("Horiz1")]
        public bool checkID = false;
        [Group("Horiz1")]
        [EnableIf(nameof(checkID))]
        [HideLabel]
        public string itemID = "";
        [Group("Horiz2")]
        public bool checkType = true;
        [Group("Horiz2")]
        [EnableIf(nameof(checkType))]
        [HideLabel]
        public ItemData.Type type = ItemData.Type.Weapon;
        [Group("Horiz3")]
        public bool checkClass = true;
        [Group("Horiz3")]
        [EnableIf(nameof(checkClass))]
        [HideLabel]
        public ItemModuleAI.WeaponClass weaponClass = ItemModuleAI.WeaponClass.Melee;
        [Group("Horiz4")]
        public bool checkHandling = true;
        [Group("Horiz4")]
        [EnableIf(nameof(checkHandling))]
        [HideLabel]
        public ItemModuleAI.WeaponHandling weaponHandling = ItemModuleAI.WeaponHandling.OneHanded;
        [Group("Horiz5")]
        public bool checkGrabbableSide = true;
        [Group("Horiz5")]
        [EnableIf(nameof(checkGrabbableSide))]
        [HideLabel]
        public Side handleSide = Side.Right;

        public float avoidTargetRadius = 4;

        public bool requirePathAvailable = true;
        public bool useShortestPath = true;

        public string outputItemVariableName = "";
        public string outputItemTransformVariableName = "";
        public string outputItemHandleVariableName = "";
        public string outputItemHandleTransformVariableName = "";

        public GetItem() { }

    }
}

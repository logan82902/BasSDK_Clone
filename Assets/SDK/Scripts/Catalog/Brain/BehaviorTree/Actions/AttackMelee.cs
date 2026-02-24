using UnityEngine;
using System;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [DeclareBoxGroup("Inputs", Title = "Inputs")]
    [DeclareHorizontalGroup("Inputs/Horiz")]
    [DeclareHorizontalGroup("Horiz")]
    [DeclareBoxGroup("Horiz/Melee", Title = "Melee")]
    [DeclareBoxGroup("Horiz/Range", Title = "Range")]
    [DeclareBoxGroup("Horiz/Misc", Title = "Misc")]
    [DeclareBoxGroup("Horiz/Debug", Title = "Debug")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class AttackMelee : ActionNode
    {
        [Group("Inputs/Horiz")]
        [LabelWidth(100)]
        public Target target = Target.CurrentTarget;
        [Group("Inputs/Horiz")]
        [EnableIf(nameof(target), Target.InputTransform)]
        [HideIf("target", Target.InputCreature)]
        [LabelWidth(200)]
        public string inputTransformVariableName = "";
        [Group("Inputs/Horiz")]
        [ShowIf("target", Target.InputCreature)]
        [LabelWidth(200)]
        public string inputCreatureVariableName = "";

        [Group("Horiz/Melee")]
        [LabelWidth(160)]
        public bool useMeleeDelay = false;
        [Group("Horiz/Melee")]
        [DisableIf("target", Target.None)]
        [LabelWidth(160)]
        public bool defensePreferenceChance = true;
        [Group("Horiz/Melee")]
        [DisableIf("target", Target.None)]
        [LabelWidth(160)]
        public bool checkCurrentTargetSight = true;
        [Group("Horiz/Melee")]
        [DisableIf("target", Target.None)]
        [LabelWidth(160)]
        public bool checkCloseAllies = true;
        [Group("Horiz/Melee")]
        [DisableIf("target", Target.None)]
        [LabelWidth(160)]
        public bool checkNavMesh = true;

        [Group("Horiz/Range")]
        [EnableIf(nameof(target), Target.None)]
        [LabelWidth(140)]
        public float forceMeleeRange = 0;
        [Group("Horiz/Range")]
        [EnableIf(nameof(target), Target.None)]
        [LabelWidth(140)]
        public float forceMeleeRangeMoved = 0;
        [Group("Horiz/Range")]
        [EnableIf(nameof(target), Target.None)]
        [LabelWidth(140)]
        public float forceWeaponReach = 0;
        [Group("Horiz/Range")]
        [EnableIf(nameof(target), Target.None)]
        [LabelWidth(140)]
        public float forceMaxRangeDelta = 0;

        [Group("Horiz/Misc")]
        [LabelWidth(100)]
        public bool stopOnReset = false;

        [Group("Horiz/Debug")]
        [EnableIf(nameof(target), Target.None)]
        public int pickClipSequentialMaxIndex = 0;
        [Group("Horiz/Debug")]
        [EnableIf(nameof(target), Target.None)]
        public bool updateClipRange = false;

        public enum Target
        {
            CurrentTarget,
            InputCreature,
            InputTransform,
            None,
        }

    }
}


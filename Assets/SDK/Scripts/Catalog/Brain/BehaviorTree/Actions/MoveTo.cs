using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [DeclareBoxGroup("Inputs", Title = "Inputs")]
    [DeclareHorizontalGroup("Inputs/Horiz")]
    [DeclareHorizontalGroup("Inputs/Horiz2")]
    [DeclareHorizontalGroup("Inputs/Horiz3")]
    [DeclareBoxGroup("Approach", Title = "Approach")]
    [DeclareHorizontalGroup("Approach/Horiz")]
    [DeclareBoxGroup("Strafe", Title = "Strafe")]
    [DeclareHorizontalGroup("Strafe/Horiz2")]
    [DeclareHorizontalGroup("Strafe/Horiz")]
    [DeclareBoxGroup("Path", Title = "Path")]
    [DeclareHorizontalGroup("Path/Horiz")]
    [DeclareBoxGroup("Radius", Title = "Radius")]
    [DeclareHorizontalGroup("Radius/Horiz")]
    [DeclareHorizontalGroup("Radius/Horiz2")]
    [DeclareBoxGroup("Instance", Title = "Instance")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class MoveTo : ActionNode
    {
        [Group("Inputs/Horiz")]
        [LabelWidth(100)]
        public MoveTarget moveTarget = MoveTarget.CurrentTarget;
        [DisableIf("moveTarget", MoveTarget.CurrentTarget)]
        [Group("Inputs/Horiz")]
        [LabelWidth(200)]
        public string inputMoveTargetVariableName = "";

        public enum MoveTarget
        {
            CurrentTarget,
            InputTransform,
            InputCreature,
            InputPosition,
        }

        [Group("Inputs/Horiz2")]
        [LabelWidth(100)]
        public TurnTarget turnTarget = TurnTarget.CurrentTarget;
        [Group("Inputs/Horiz2")]
        [EnableIf(nameof(enableTurnTargetDir))]
        [LabelWidth(100)]
        public TurnTargetDirection turnTargetDirection = TurnTargetDirection.Position;
        private bool enableTurnTargetDir => turnTarget == TurnTarget.InputTransform || turnTarget == TurnTarget.InputCreature;
        [Group("Inputs/Horiz2")]
        [EnableIf(nameof(enableTurntargetVarName))]
        [LabelWidth(200)]
        public string inputTurnTargetVariableName = "";
        private bool enableTurntargetVarName => turnTarget == TurnTarget.InputTransform || turnTarget == TurnTarget.InputCreature;

        [Group("Inputs/Horiz3")]
        [LabelWidth(270)]
        public bool forceTurnToUseMoveDirectionAtDistance = true;
        [Group("Inputs/Horiz3")]
        [EnableIf(nameof(forceTurnToUseMoveDirectionAtDistance))]
        [LabelWidth(200)]
        public float turnMoveDirectionDistance = 10;
        [Group("Inputs/Horiz3")]
        [EnableIf(nameof(forceTurnToUseMoveDirectionAtDistance))]
        [LabelWidth(200)]
        public float turnMoveDirectionSpeed = 1;

        public enum TurnTarget
        {
            CurrentTarget,
            InputTransform,
            InputCreature,
            MoveDirection
        }

        public enum TurnTargetDirection
        {
            Position,
            Forward,
        }

        [Group("Approach/Horiz")]
        [LabelWidth(120)]
        [LabelText("Move speed ratio")]
        [Range(0f, 1f)]
        public float approachMoveSpeedRatio = 1;
        [Group("Approach/Horiz")]
        [LabelWidth(120)]
        [LabelText("Turn speed ratio")]
        [Range(0f, 1f)]
        public float approachTurnSpeedRatio = 1;
        [Group("Approach/Horiz")]
        [LabelWidth(120)]
        [LabelText("Run speed ratio")]
        [Range(0f, 1f)]
        public float approachRunSpeedRatio = 1;
        [Group("Approach/Horiz")]
        [LabelWidth(150)]
        public bool moveCloserIfNoSight;

        [Group("Strafe")]
        [DisableIf("moveTarget", MoveTarget.InputTransform)]
        [HideLabel]
        public bool strafeAroundTarget = false;
        [Group("Strafe/Horiz2")]
        [EnableIf(nameof(strafeAroundTarget))]
        [LabelWidth(120)]
        [LabelText("Move speed ratio")]
        public float strafeMoveSpeedRatio = 0.75f;
        [Group("Strafe/Horiz2")]
        [EnableIf(nameof(strafeAroundTarget))]
        [LabelWidth(120)]
        [LabelText("Turn speed ratio")]
        public float strafeTurnSpeedRatio = 1;
        [Group("Strafe/Horiz2")]
        [EnableIf(nameof(strafeAroundTarget))]
        [LabelWidth(120)]
        [LabelText("Run speed ratio")]
        public float strafeRunSpeedRatio = 0;

        [Group("Strafe/Horiz")]
        [EnableIf(nameof(strafeAroundTarget))]
        [LabelWidth(130)]
        public bool checkMaxAllies = false;
        [Group("Strafe/Horiz")]
        [EnableIf(nameof(checkMaxAllies))]
        [LabelWidth(150)]
        public float safeRangeOffset = 0.25f;
        [Group("Strafe/Horiz")]
        [EnableIf(nameof(checkMaxAllies))]
        [LabelWidth(150)]
        public float safeRangeThickness = 2f;

        [Group("Path/Horiz")]
        [LabelWidth(150)]
        public bool reportUnreachable = false;
        [Group("Path/Horiz")]
        [EnableIf(nameof(strafeAroundTarget))]
        [LabelWidth(150)]
        public bool useModuleStrafeDelay = false;
        [Group("Path/Horiz")]
        [DisableIf(nameof(useModuleStrafeDelay))]
        [LabelWidth(150)]
        public float repathMinDelay = 0;
        [Group("Path/Horiz")]
        [DisableIf(nameof(useModuleStrafeDelay))]
        [LabelWidth(150)]
        public float repathMaxDelay = 0;

        [Group("Radius/Horiz")]
        [LabelWidth(100)]
        public AutoRadius autoRadius = AutoRadius.None;
        [EnableIf(nameof(autoRadius), AutoRadius.None)]
        [Group("Radius/Horiz")]
        [LabelWidth(150)]
        public float targetMinRadius = 0f;
        [EnableIf(nameof(autoRadius), AutoRadius.None)]
        [Group("Radius/Horiz")]
        [LabelWidth(150)]
        public float targetMaxRadius = 0.1f;
        [EnableIf(nameof(autoRadius), AutoRadius.MeleeRange)]
        [Group("Radius/Horiz")]
        [LabelWidth(160)]
        public float keepWeaponDistance = 0.6f;
        [EnableIf(nameof(autoRadius), AutoRadius.MeleeRange)]
        [Group("Radius/Horiz")]
        [LabelWidth(160)]
        public bool advanceToMelee = true;
        [EnableIf(nameof(autoRadius), AutoRadius.FromBlackboard)]
        [Group("Radius/Horiz")]
        [LabelWidth(150)]
        public string blackboardMinName;
        [EnableIf(nameof(autoRadius), AutoRadius.FromBlackboard)]
        [Group("Radius/Horiz")]
        [LabelWidth(150)]
        public string blackboardMaxName;


        public enum AutoRadius
        {
            None,
            MeleeRange,
            TargetAgentRadius,
            FromBlackboard,
        }

        [Group("Radius/Horiz2")]
        [LabelWidth(100)]
        public AngleMode angleMode = AngleMode.None;
        [Group("Radius/Horiz2")]
        [LabelWidth(200)]
        public bool changeAngleUntilUnobstructed;
        [DisableIf("angleMode", AngleMode.None)]
        [Group("Radius/Horiz2")]
        [LabelWidth(150)]
        public float targetRadiusMinAngle = 0f;
        [DisableIf("angleMode", AngleMode.None)]
        [Group("Radius/Horiz2")]
        [LabelWidth(150)]
        public float targetRadiusMaxAngle = 0f;

        public enum AngleMode
        {
            None,
            Randomize,
            CheckAllies,
        }

    }
}
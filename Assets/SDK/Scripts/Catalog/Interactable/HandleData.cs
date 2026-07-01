using System;
using UnityEngine;
using System.Collections.Generic;


using TriInspector;

namespace ThunderRoad
{
    [SkipInTypeDropdown]
    [Serializable]
    [DeclareBoxGroup("Handle", Title = "Handle")]
    [DeclareBoxGroup("Handle/Magic", Title = "Magic")]
    [DeclareBoxGroup("Handle/UI", Title = "UI")]
    [DeclareBoxGroup("Handle/Audio", Title = "Audio")]
    [DeclareBoxGroup("Handle/Forces", Title = "Forces")]
    [DeclareBoxGroup("Handle/Allowance", Title = "Allowance")]
    [DeclareBoxGroup("Handle/Telekinesis", Title = "Telekinesis")]
    [DeclareBoxGroup("Handle/Slide", Title = "Slide")]
    public class HandleData : InteractableData
    {
        [Group("Handle")]
        public bool disableOnStart = false;
        [Group("Handle")]
        public bool rotateAroundAxis = false;

        [Group("Handle")]
        public bool forceHoldGripToGrab = false;
        [Group("Handle")]
        public bool forcePlayerPhysicLink = false;

        [Group("Handle")]
        public bool disableHandCollider = false;

        [Group("Handle")]
        public bool alwaysTeleportToHand = false;
        [Group("Handle")]
        public bool disablePinchGrab = false;
        [Group("Handle")]
        public bool setCenterOfMassTohandle = false;
        [Group("Handle")]
        public float handlerLocomotionSpeedMultiplier = 1;
        [Group("Handle")]
        public bool forceAllowTwoHanded;
        [Group("Handle")]
        public bool allowSlidingWithBothHand;

        [Group("Handle/Magic")]
        public bool allowSpellMenu = false;
        [Group("Handle/Magic")]
        public bool allowSpellFire = false;
        [Group("Handle/Magic")]
        [ShowIf("allowSpellFire")]
        public Vector3 spellFireMagicOffset;
        [Group("Handle/Magic")]
        [ShowIf("allowSpellFire")]
        public bool offsetInHandleSpace;

        [Group("Handle/UI")]
        public bool warnIfNotAllowed = false;

        [Group("Handle/Audio")]
        [Dropdown(nameof(GetAllEffectID))]
        public string grabEffectId;
        [NonSerialized]
        public EffectData grabEffectData;
        [Group("Handle/Audio")]
        public float slideFxMinVelocity = 2;
        [Group("Handle/Audio")]
        public float slideFxMaxVelocity = 10;

        [Group("Handle/Forces")]
        public RotationDrive rotationDrive = RotationDrive.Slerp;
        [Group("Handle/Forces")]
        public bool useWYZWhenTwoHanded = true;
        [Group("Handle/Forces")]
        public float positionSpringMultiplier = 1;
        [Group("Handle/Forces")]
        public float positionDamperMultiplier = 1;
        [Group("Handle/Forces")]
        public float rotationSpringMultiplier = 1;
        [Group("Handle/Forces")]
        public float rotationDamperMultiplier = 1;
        [Group("Handle/Forces")]
        public float rotationSpring2hMultiplier = 1;
        [Group("Handle/Forces")]
        public float rotationDamper2hMultiplier = 1;
        [Group("Handle/Forces")]
        public bool forceClimbing = false;
        [Group("Handle/Forces")]
        public bool affectedByBowHandling = false;

        public enum RotationDrive
        {
            Slerp,
            X,
            YZ,
            WYZ,
        }

        [Group("Handle")]
        public bool dominantWhenTwoHanded = true;
        [Group("Handle")]
        public bool ignoreSnap = false;
        [Group("Handle")]
        public bool disabledOnSnap = false;
        [Group("Handle")]
        public bool useDefaultGripForHolder = true;

        [Group("Handle/Allowance")]
        public bool allowAutoGrab = true;
        [Group("Handle/Allowance")]
        public bool allowSteal = true;
        [Group("Handle/Allowance")]
        public bool allowSwap = true;

        [Group("Handle/Telekinesis")]
        public bool allowTelekinesis;
        [Group("Handle/Telekinesis")]
        public float telekinesisPullRepelMultiplier = 1;
        [Group("Handle/Telekinesis")]
        public float telekinesisMaxDistanceMultiplier = 1;
        [Group("Handle/Telekinesis")]
        public float telekinesisPositionSpringMultiplier = 1;
        [Group("Handle/Telekinesis")]
        public float telekinesisPositionDamperMultiplier = 1;
        [Group("Handle/Telekinesis")]
        public float telekinesisPositionMaxForceMultiplier = 1;
        [Group("Handle/Telekinesis")]
        public float telekinesisRotationSpringMultiplier = 1;
        [Group("Handle/Telekinesis")]
        public float telekinesisRotationDamperMultiplier = 1;
        [Group("Handle/Telekinesis")]
        public float telekinesisRotationMaxForceMultiplier = 1;

        [Group("Handle/Slide")]
        public float slideMinDamper = 20;
        [Group("Handle/Slide")]
        public float slideMaxDamper = 300;
        [Group("Handle/Slide")]
        public float slideMotorMaxForce = 0;
        [Group("Handle/Slide")]
        public float slideMotorAcceleration = 5;
        [Group("Handle/Slide")]
        public SlideMotorDirection slideMotorDir = SlideMotorDirection.Gravity;

        public const string tagTkDefault = "TkDefault";
        public const string tagTkRagdoll = "TkRagdoll";
        public const string tagTkDefaultLeft = "TkDefaultLeft";
        public const string tagTkDefaultRight = "TkDefaultRight";
        public const string tagTkRagdollLeft = "TkRagdollLeft";
        public const string tagTkRagdollRight = "TkRagdollRight";

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

        public TriDropdownList<string> GetAllHandPoseID() => Catalog.GetDropdownAllID(Category.HandPose);

        public enum SlideMotorDirection
        {
            Gravity,
            Up,
            Down,
        }

        public enum AttachType
        {
            Parent,
            Joint,
        }

        public enum TwoHandedMode
        {
            Position,
            Dominant,
            AutoFront,
            AutoRear,
        }

    }
}

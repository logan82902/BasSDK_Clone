using System;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    [DeclareBoxGroup("Ragdoll", Title = "Ragdoll")]
    [DeclareBoxGroup("Ragdoll/IK", Title = "IK")]
    [DeclareBoxGroup("Ragdoll/Lift", Title = "Lift")]
    [DeclareBoxGroup("Handle/StabilizationJoint", Title = "StabilizationJoint")]
    [DeclareBoxGroup("Handle/Telekinesis", Title = "Telekinesis")]
    [DeclareBoxGroup("Ragdoll/Move", Title = "Move")]
    [DeclareBoxGroup("Ragdoll/Forces", Title = "Forces")]
    [DeclareBoxGroup("Ragdoll/Char joint", Title = "Char joint")]
    public class HandleRagdollData : HandleData
    {
        [Group("Ragdoll")]
        public Behaviour behaviour = Behaviour.None;
        [Group("Ragdoll")]
        public float scaleDuringCombat = 1f;
        [Group("Ragdoll")]
        public TkCondition tkActiveCondition = TkCondition.Always;

        public enum Behaviour
        {
            None,
            Carry,
            Muffle,
            Choke,
            ChokeAndMuffle
        }

        public enum TkCondition
        {
            Never,
            CreatureDead,
            PartSliced,
            Always,
        }

        [Group("Ragdoll")]
        public List<RagdollPart.Type> activateCollisionOnParts;

        [Group("Ragdoll/IK")]
        public bool useIK;
        [Group("Ragdoll/IK")]
        public bool allowRotationIK;
        [Group("Ragdoll/IK")]
        public float IkPositionWeight = 1;
        [Group("Ragdoll/IK")]
        public float IkRotationWeight = 1;
        [Group("Ragdoll/IK")]
        public bool changeHeight;

        [Group("Ragdoll/Lift")]
        public LiftBehaviour liftBehaviour = LiftBehaviour.None;
        [Group("Ragdoll/Lift")]
        public Part liftPartReference = Part.Root;
        [Group("Ragdoll/Lift")]
        public ForceLiftCondition forceLiftGrabCondition;
        [Group("Ragdoll/Lift")]
        public ForceLiftCondition forceLiftTKCondition;
        [Group("Ragdoll/Lift")]
        public float liftOffset = 0.05f;
        [Group("Ragdoll/Lift")]
        public bool telekinesisSpinningFix = true; // Fixes the TK spin, just in case it might break stuff, easy to disable
        [Group("Ragdoll/Lift")]
        public bool liftSpinningFix = true; // The fix enabling the lifting to lock hips 
        [Group("Ragdoll/Lift")]
        public float liftSpinningFacingPower = 10f; // Hips facing slerp power

        [Group("Ragdoll/Lift")]
        public float liftSpinningFacingMaxSpeed = 5f; // Maximum slerping speed of the hips
        [Group("Ragdoll/Lift")]
        public float liftSpinEnableMaxSpeed = 1.5f; // At which speed does the spin fix get enabled


        [Group("Handle/StabilizationJoint")]
        public bool applyStabilizationJoint = true;
        [Group("Handle/StabilizationJoint")]
        public Vector3 telekinesisStabilizationJointAxis;
        [Group("Handle/StabilizationJoint")]
        public float stabilizationEnableMinSpeed = 5f;

        [Group("Handle/StabilizationJoint")]
        public float stabilizationEnableDot = 0.7f;
        [Group("Handle/StabilizationJoint")]
        public float stabilizationDisableDotMultiplier = 0.7f;
        [Group("Handle/StabilizationJoint")]
        public float stabilizationPositionSpring = 2000;
        [Group("Handle/StabilizationJoint")]
        public float stabilizationPositionDamper = 200;
        [Group("Handle/StabilizationJoint")]
        public float stabilizationPositionMaxForce = 1500;

        [Group("Handle/Telekinesis")]
        public Vector2 playerFaceForceRange = new(25f, 45f);
        [Group("Handle/Telekinesis")]
        public float restingAngle = 20f;

        [Group("Ragdoll/Move")]
        public bool moveStep;
        [Group("Ragdoll/Move")]
        public Part stepPartReference = Part.Head;
        [Group("Ragdoll/Move")]
        public float stepSpeedMultiplier = 1;
        [Group("Ragdoll/Move")]
        public float stepThresholdMultiplier = 1;
        [Group("Ragdoll/Move")]
        public BodyTurnDirection bodyTurnDirection = BodyTurnDirection.None;
        [Group("Ragdoll/Move")]
        [ShowIf("bodyTurnDirection", BodyTurnDirection.ClosestCardinal)]
        public Cardinal cardinal = Cardinal.XZ;

        [Group("Ragdoll/Forces")]
        public float releaseArmSpring = 0.25f;
        [Group("Ragdoll/Forces")]
        public float releaseArmDamper = 0.5f;

        [Group("Ragdoll/Char joint")]
        public List<RagdollPart.Type> overrideCharJointLimitsOnParts;
        [Group("Ragdoll/Char joint")]
        public float swing1Limit = 0;
        [Group("Ragdoll/Char joint")]
        public float lowTwistLimit = 0;
        [Group("Ragdoll/Char joint")]
        public float highTwistLimit = 0;
        [Group("Ragdoll/Char joint")]
        public bool invertLowHighOnBack = true;

		[Flags]
        public enum ForceLiftCondition
        {
            IdleOrPatrol = 1,
            Combat = 2,
            Attack = 4,
            Stagger = 8,
            StaggerFull = 16,
            Electrocuted = 32,
        }

        public enum LiftBehaviour
        {
            None,
            Fall,
            Ungrab,
        }

        public enum Part
        {
            Self,
            Root,
            Target,
            Head,
        }

        public enum BodyTurnDirection
        {
            None,
            HandDirection,
            PartDirection,
            ClosestCardinal,
            GrabberPosition,
        }

        public TriDropdownList<string> GetAllDamagerID() => Catalog.GetDropdownAllID(Category.Damager);

    }
}

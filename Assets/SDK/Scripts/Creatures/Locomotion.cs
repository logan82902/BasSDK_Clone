using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Creatures/Locomotion.html")]
    [AddComponentMenu("ThunderRoad/Locomotion")]
    [RequireComponent(typeof(Rigidbody))]
    public class Locomotion : ThunderBehaviour
    {
        [Header("Ground movement")]
        public bool allowMove = true;
        public bool allowTurn = true;
        public bool allowJump = true;
        public bool allowCrouch = true;

        public AnimationCurve moveForceMultiplierByAngleCurve;
       
        public float forwardSpeed = 0.2f;
        public float backwardSpeed = 0.2f;
        public float strafeSpeed = 0.2f;
        public float runSpeedAdd = 0.1f;
        public float crouchSpeed = 0.1f;

        public FloatHandler globalMoveSpeedMultiplier;
        
        protected float forwardSpeedMultiplier = 1f;
        protected float backwardSpeedMultiplier = 1f;
        protected float strafeSpeedMultiplier = 1f;
        protected float runSpeedMultiplier = 1f;
        protected float crouchSpeedMultiplier = 1f;
        protected float jumpForceMultiplier = 1f;

        public float forwardAngle = 10;
        public float backwardAngle = 10;
        public float runDot = 0.75f;
        public bool runEnabled = true;

        public float crouchHeightRatio = 0.8f;

        public float turnSpeed = 1;
        private float snapTurnTime;
        private bool smoothSnapTurn;

        [Header("Jump / Fall")]
        public float horizontalAirSpeed = 0.02f;
        public float verticalAirSpeed = 0f;
        public float waterSpeed = 0.08f;
        public ForceMode airForceMode = ForceMode.VelocityChange;
        public float jumpGroundForce = 0.3f;
        public float jumpClimbVerticalMultiplier = 0.8f;
        public float jumpClimbVerticalMaxVelocityRatio = 20f;
        public float jumpClimbHorizontalMultiplier = 1f;
        public float jumpMaxDuration = 0.6f;

        [Header("Turn")]
        public float turnSmoothDirection;
        public float turnSnapDirection;
        public float turnSmoothSnapDirection;
        public Vector3 moveDirection;
        public ForceMode verticalForceMode = ForceMode.VelocityChange;

        [Header("Colliders")]
        public float colliderRadius = 0.3f;
        public float colliderShrinkMinRadius = 0.05f;
        public float colliderGrowDuration = 2f;
        public float colliderHeight = 1f;
        [Tooltip("Only enable this on ")]
        public bool collideWithPlayer = true;

        [Header("Ground detection")]
        public float groundDetectionDistance = 0.05f;
        public PhysicMaterial colliderGroundMaterial;
        public float groundDrag = 3f;
        protected float groundDragMultiplier = 1f;
        public PhysicMaterial colliderFlyMaterial;
        public float flyDrag = 1f;
        protected float flyDragMultiplier = 1f;

        [NonSerialized]
        public PhysicBody physicBody;

        [NonSerialized]
        public CapsuleCollider capsuleCollider;

        protected float orgMass;
        protected float orgDrag;
        protected float orgAngularDrag;
        protected float orgSleepThreshold;
        public float customGravity;

        public enum GroundDetection
        {
            Raycast,
            Collision,
        }

        [NonSerialized]
        public Vector3 prevVelocity;
        [NonSerialized]
        public Vector3 prevPosition;
        [NonSerialized]
        public Quaternion prevRotation;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public Vector3 velocity;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public float horizontalSpeed;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public float verticalSpeed;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public float angularSpeed;

        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public bool isCrouched;

        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public LayerMask groundMask;

        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public bool isGrounded;
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public RaycastHit groundHit;

        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public float groundAngle;

        public bool locomotionError { get; private set; } = false;


        private Vector3 accelerationCurrentSpeed;

        public enum TurnMode
        {
            Instant,
            Snap,
            Smooth,
            Disabled,
        }

        public enum CrouchMode
        {
            Disabled,
            Hold,
            Toggle
        }

    }
}

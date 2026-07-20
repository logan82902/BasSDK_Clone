using System;
using System.Collections.Generic;
using ThunderRoad.Skill.Spell;
using TriInspector;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace ThunderRoad.Skill.SpellPower
{
    [Serializable]
    [DeclareBoxGroup("Hand pose", Title = "Hand pose")]
    [DeclareBoxGroup("Throw", Title = "Throw")]
    public class SpellTelekinesis : SpellData
    {
        public static bool holdGrip;
        public static bool showHighlighter = true;
        [Range(0f, 1f)]
        public float palmDirectionPercent = 0.35f;
        public float maxCatchDistance = 6f;
        public float radius = 1f;
        public float maxAngle = 40;
        [Range(0f, 1f)]
        public float raycastArmInfluence = 0.7f;
        [Range(0f, 1f)]
        public float targetingLookInfluence = 0.2f;
        [Range(0f, 1f)]
        public float antiBrokenBias = 1.0f;
        [Range(0f, 1f)]
        public float lastHeldBias = 10f / 180f;
        [Range(0f, 1f)]
        public float stickyBias = 3f / 180f;
        public LayerMask layerMask;
        public LayerMask lineOfSightMask;
        public LayerMask itemLayerMask;

        [Header("Forces")]
        public float positionSpring = 100;
        public float positionDamper = 10;
        public float positionMaxForce = 1000;

        public float rotationSpring = 10;
        public float rotationDamper = 1;
        public float rotationMaxForce = 1000;

        public float drag = 5;
        public float angularDrag = 0.5f;
        public float reverseAngle = 110;
        public float minFingersWeight = 0f;
        public bool gravity = false;
        public float throwMultiplier = 2;
        public int grabThrowLevel = 0;
        public bool forceDestabilizeOnThrow = false;

        [Header("Follow")]
        public AnimationCurve followDefaultSpeedCurve;
        public AnimationCurve followRagdollSpeedCurve;
        protected float catchTime;
        public bool enabled { get; protected set; } = true;
        protected HashSet<object> disablers;

        [Dropdown(nameof(GetAllEffectID))]
        public string defaultHandEffectId;
        protected EffectData defaultHandEffectData;


        [Dropdown(nameof(GetAllEffectID))]
        public string ragdollHandEffectId;
        protected EffectData ragdollHandEffectData;

        [Dropdown(nameof(GetAllEffectID))]
        public string ragdollGripEffectId;
        protected EffectData ragdollGripEffectData;


        [Dropdown(nameof(GetAllSkillID))]
        public string teleSpinId;

        [Dropdown(nameof(GetAllEffectID))]
        public string spinEffectId;
        protected EffectData spinEffectData;


        [NonSerialized]
        public float gripDistance;
        [NonSerialized]
        public Rigidbody grip;
        protected Side side;
        protected ConfigurableJoint joint;
        protected ConstantForce hangingMassForce;
        [NonSerialized]
        public bool justCatched;


        [NonSerialized]
        bool stabilizationReach = false;

        [Header("Pull / Repel")]
        public float repelMaxDistance = 12;
        public float pullAndRepelMaxSpeed = 8;
        public float pullAndRepelNoDragThreshold = 0.8f;
        [NonSerialized]
        public float pullSpeed;
        [NonSerialized]
        public float repelSpeed;
        public bool autoGrab = true;
        [Header("Choke")]
        public bool allowChoke;
        public float chokeDamageInterval = 1f;
        public float chokeDamageVariation = 0.4f;
        public float chokeDamage = 5;
        protected float lastChokeTime;

        [Header("Spin")]
        public bool allowSpin = false;
        public bool spinMode;
        public AnimationCurve spinVelocityCurve;
        public bool spinFromCenterOfMass = true;
        [NonSerialized]
        public float spinVelocity;
        protected float spinStartTime;
        protected bool spinToogle;
        protected float spinIntensity;
        protected float lastSpinHaptic;

        [Header("Push")]
        public float pushDefaultForce = 20;
        public float pushRagdollForce = 30;
        public float pushRagdollOtherPartsForce = 5;
        [Dropdown(nameof(GetAllEffectID))]
        public string pushEffectId;
        protected EffectData pushEffectData;
        public float minVelocity = 0f;
        public float maxVelocity = 5f;

        [Header("Dismemberment")]
        public bool allowDismemberment = true;
        public float dismembermentBreakForceMultiplier = 1f;

        [Group("Hand pose")]
        [Dropdown(nameof(GetAllHandPoseID))]
        public string closeHandPoseId;
        [NonSerialized]
        public HandPoseData closeHandPoseData;

        [Group("Hand pose")]
        [Dropdown(nameof(GetAllHandPoseID))]
        public string openHandPoseId;
        [NonSerialized]
        public HandPoseData openHandPoseData;

        [Group("Throw")]
        public bool clearFloatingOnThrow = false;

        protected RaycastHit[] handRayHits;
        protected List<(Collider hitCollider, float value)> sortedHits;
        protected Collider[] touchedColliders;
        protected HashSet<PhysicBody> touchedPBs;

        [NonSerialized]
        public bool grabRagdoll;
        [NonSerialized]
        private bool isPulling;
        [NonSerialized]
        private bool isRepelling;

        [NonSerialized]
        private float bodyTargetAngle;
        [NonSerialized]
        private float bodyCurrentAngle;

        [NonSerialized]
        private bool blockedTargeting = false;
        [NonSerialized]
        private bool updateTargetBlock = true;

        public float CatchDistance => maxCatchDistance * GetModifier(Modifier.Range);

        public new SpellTelekinesis Clone()
        {
            return this.MemberwiseClone() as SpellTelekinesis;
        }

    }
}

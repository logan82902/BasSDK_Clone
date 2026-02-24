using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TriInspector;


namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Ragdoll")]
    [AddComponentMenu("ThunderRoad/Creatures/Ragdoll")]
    [DeclareTabGroup("Tabs")]
    [DeclareVerticalGroup("Tabs/State")]
    [DeclareVerticalGroup("Tabs/Parts")]
    [DeclareVerticalGroup("Tabs/StandUp")]
    [DeclareVerticalGroup("Tabs/Collision")]
    [DeclareToggleGroup("Tabs/PhysicToggle", Title = "Physics Toggle Enabled")]
    [DeclareHorizontalGroup("Tabs/PhysicToggle/Runtime", Sizes = new float[] { 0f, 120f })]
    public class Ragdoll : ThunderBehaviour
    {
        [Group("Tabs/State"), Tab("General")]
        public State state = State.Disabled;
        [Group("Tabs"), Tab("General")]
        public Transform meshRig;
        [Group("Tabs"), Tab("General")]
        public Transform meshRootBone;
        [Group("Tabs"), Tab("General")]
        public bool hipsAttached;

        [Group("Tabs/PhysicToggle"), Tab("General")]
        public bool physicToggle;
        [Group("Tabs/PhysicToggle"), Tab("General")]
        public float physicTogglePlayerRadius = 5;
        [Group("Tabs/PhysicToggle"), Tab("General")]
        public float physicToggleRagdollRadius = 3;
        [Group("Tabs/PhysicToggle"), Tab("General")]
        public float physicEnabledDuration = 2;

        [Header("Parts")]
        [Group("Tabs/Parts"), Tab("General")]
        public RagdollPart headPart;
        [Group("Tabs/Parts"), Tab("General")]
        public RagdollPart leftUpperArmPart;
        [Group("Tabs/Parts"), Tab("General")]
        public RagdollPart rightUpperArmPart;
        [Group("Tabs/Parts"), Tab("General")]
        public RagdollPart targetPart;
        [Group("Tabs/Parts"), Tab("General")]
        public RagdollPart rootPart;

        [Header("StandUp")]
        [Group("Tabs/StandUp"), Tab("General")]
        public AnimationCurve standUpCurve;
        [Group("Tabs/StandUp"), Tab("General")]
        public float standUpFromGrabDuration = 1.0f;
        [Group("Tabs/StandUp"), Tab("General")]
        public float preStandUpDuration = 3f;
        [Group("Tabs/StandUp"), Tab("General")]
        public float preStandUpRatio = 0.7f;

        [Header("Collision")]
        [Group("Tabs/Collision"), Tab("General")]
        public float collisionEffectMinDelay = 0.2f;
        [Group("Tabs/Collision"), Tab("General")]
        public float collisionMinVelocity = 2.0f;

        [Header("Misc")]
        [Group("Tabs"), Tab("General")]
        public bool allowSelfDamage;
        [Group("Tabs"), Tab("General")]
        public bool grippable = true;

        [Header("Default forces")]
        [Group("Tabs"), Tab("Forces")]
        public float springPositionForce = 1000;
        [Group("Tabs"), Tab("Forces")]
        public float damperPositionForce = 50;
        [Group("Tabs"), Tab("Forces")]
        public float maxPositionForce = 1000;

        [Group("Tabs"), Tab("Forces")]
        public float springRotationForce = 800;
        [Group("Tabs"), Tab("Forces")]
        public float damperRotationForce = 50;
        [Group("Tabs"), Tab("Forces")]
        public float maxRotationForce = 100;

        [Header("Destabilized")]
        [Group("Tabs"), Tab("Forces")]
        public float destabilizedSpringRotationMultiplier = 0.5f;
        [Group("Tabs"), Tab("Forces")]
        public float destabilizedDamperRotationMultiplier = 0.1f;
        [Group("Tabs"), Tab("Forces")]
        public float destabilizedGroundSpringRotationMultiplier = 0.2f;

        [Header("HipsAttached")]
        [Group("Tabs"), Tab("Forces")]
        public float hipsAttachedSpringPositionMultiplier = 1f;
        [Group("Tabs"), Tab("Forces")]
        public float hipsAttachedDamperPositionMultiplier = 0f;
        [Group("Tabs"), Tab("Forces")]
        public float hipsAttachedSpringRotationMultiplier = 1f;
        [Group("Tabs"), Tab("Forces")]
        public float hipsAttachedDamperRotationMultiplier = 0f;

        [Header("Player arm")]
        [Group("Tabs"), Tab("Forces")]
        public float playerArmPositionSpring = 5000f;
        [Group("Tabs"), Tab("Forces")]
        public float playerArmPositionDamper = 40f;
        [Group("Tabs"), Tab("Forces")]
        public float playerArmMaxPositionForce = 3000f;
        [Group("Tabs"), Tab("Forces")]
        public float playerArmRotationSpring = 1000f;
        [Group("Tabs"), Tab("Forces")]
        public float playerArmRotationDamper = 40f;
        [Group("Tabs"), Tab("Forces")]
        public float playerArmMaxRotationForce = 250f;

        [NonSerialized]
        public Creature creature;

        /// <summary>
        /// Set by the PhysicToggleManager to indicate if this ragdoll should have physics enabled on next check
        /// This just acts as a cached bool to avoid storing state in the physic toggle manager
        /// </summary>
        [NonSerialized]
        public bool shouldEnablePhysic;
        
        public static bool playerPhysicBody;

        [NonSerialized]
        public Transform animatorRig;
        [NonSerialized]
        public float totalMass;

        public enum State
        {
            Inert,
            Destabilized,
            Frozen,
            Standing,
            Kinematic,
            NoPhysic,
            Disabled,
        }

        [Group("Tabs/Parts"), Tab("General")]
        public List<RagdollPart> parts;

        [Group("Tabs/Parts"), Tab("General")]
        public List<RagdollPart> safeSliceParts;

        [Group("Tabs/Parts"), Tab("General")]
        [ShowInInspector]
        [NonSerialized]
        public List<Bone> bones = new List<Bone>();


        [NonSerialized]
        public IkController ik; 
        [NonSerialized]
		public HumanoidFullBodyIK humanoidIk;
		[NonSerialized]
        public List<RagdollHand> handlers = new List<RagdollHand>();
        [NonSerialized]
        public List<SpellCaster> tkHandlers = new List<SpellCaster>();

        [NonSerialized]
        public bool isGrabbed;
        [NonSerialized]
        public bool isTkGrabbed;
        [NonSerialized]
        public bool isSliced;

        [NonSerialized]
        public bool charJointBreakEnabled;

        public BoolHandler forcePhysic;

        [NonSerialized]
        public List<PhysicModifier> physicModifiers = new List<PhysicModifier>();

        public class PhysicModifier
        {
            public object handler;
            public EffectData effectData;
            public PhysicModifier(object handler, EffectData effectData = null)
            {
                this.handler = handler;
                this.effectData = effectData;
            }
        }

        [NonSerialized]
        public bool initialized;

        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        [LabelText("Standing Up (Runtime)")]
        [Group("Tabs/StandUp"), Tab("General")]
        public bool standingUp;
        [NonSerialized]
        public float standStartTime = 0f;
        protected Coroutine getUpCoroutine;

        /// <summary>
        /// Helper class to keep the line of which objects would like to apply a stabilization joint.
        /// </summary>
        private class StabilizationJointQueue
        {
            public GameObject owningObject;
            //public GameObject relativeObject;
            public StabilizationJointSettings settings;

            public StabilizationJointQueue(GameObject owningObject, StabilizationJointSettings settings)
            {
                this.owningObject = owningObject;
                this.settings = settings;
            }
        }

        /// <summary>
        /// Helper class to assign necessary settings to the joint,
        /// nearly identical to the stabilization joint, but additional stuff might be needed sometimes
        /// </summary>
        public class StabilizationJointSettings
        {
            public Vector3 axis = Vector3.zero;
            public ConfigurableJointMotion angularXMotion = ConfigurableJointMotion.Free;
            public ConfigurableJointMotion angularYMotion = ConfigurableJointMotion.Free;
            public ConfigurableJointMotion angularZMotion = ConfigurableJointMotion.Free;
            public bool configuredInWorldSpace = false;
            public bool autoConfigureConnectedAnchor = true;
            public SoftJointLimit angularXLimit;
            public SoftJointLimitSpring angularXLimitSpring;
            public SoftJointLimit angularYLimit;
            public SoftJointLimit angularZLimit;
            public SoftJointLimitSpring angularYZLimitSpring;
            public JointDrive angularYZDrive = new JointDrive();
            public JointDrive angularXDrive = new JointDrive();
            public bool isKinematic = false;
            public GameObject relativeObject = null;
        }

        // The joint required to stabilize the motion of the ragdoll when it's connected to by something and is in the air.
        ConfigurableJoint stabilizationJoint;
        GameObject stabilizationJointFollowObject;
        // The queue (Really a list) to see which objects would like to add a joint to stabilize the ragdoll in the air.
        List<StabilizationJointQueue> stabilizationJointQueue = new List<StabilizationJointQueue>();

        // Gets the stabilization joint for use in any other components. i.e. Telekinesis to rotate character to face the players hand forward
        public ConfigurableJoint StabilizationJoint { get { return stabilizationJoint; } }

        public enum PhysicStateChange
        {
            None,
            ParentingToPhysic,
            PhysicToParenting,
        }


        [System.Serializable]
        public class Bone
        {
            public int[] boneHashes;
            public Transform mesh;
            public Transform animation;
            public Transform meshSplit;
            public ConfigurableJoint animationJoint;
            public FixedJoint fixedJoint;

            public RagdollPart part;
            public Bone parent;
            public List<Bone> childs;
            public bool hasChildAnimationJoint;

            public Vector3 orgLocalPosition;
            public Quaternion orgLocalRotation;

            public Vector3 orgCreatureLocalPosition;
            public Quaternion orgCreatureLocalRotation;

        }

        [Button]
        [Group("Tabs/Parts"), Tab("General")]
        public void AutoCreateMeshColliders()
        {
            Dictionary<Transform, List<SkinnedMeshRenderer>> associatedBones = new Dictionary<Transform, List<SkinnedMeshRenderer>>();
            foreach (SkinnedMeshRenderer smr in meshRig.parent.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                MeshCollider smrCollider = new GameObject(smr.name + "_Collider").AddComponent<MeshCollider>();
                smrCollider.sharedMesh = smr.sharedMesh;
                smrCollider.convex = true;
                smrCollider.transform.parent = transform;
                if (smr.bones.Length == 0) Debug.LogError($"{smr.name} has no associated bones and won't be automatically moved to the ragdoll!");

                
                if (smr.bones.Length > 0)
                {
                    Mesh mesh = smr.sharedMesh;
                    var bonesPerVertex = mesh.GetBonesPerVertex();
                    if (bonesPerVertex.Length == 0) return;

                    var boneWeights = mesh.GetAllBoneWeights();

                    int greatestInfluenceBone = -1;
                    float greatestInfluence = 0f;
                    var influences = new float[smr.bones.Length];

                    for (int i = 0; i < boneWeights.Length; i++)
                    {
                        var boneWeight = boneWeights[i];
                        influences[boneWeight.boneIndex] += boneWeight.weight;
                        if (influences[boneWeight.boneIndex] > greatestInfluence)
                        {
                            greatestInfluence = influences[boneWeight.boneIndex];
                            greatestInfluenceBone = boneWeight.boneIndex;
                        }
                    }
                    Transform associatedBone = smr.bones[greatestInfluenceBone];
                    Debug.Log("Associated bone: " + associatedBone.name);
                    foreach (RagdollPart part in GetComponentsInChildren<RagdollPart>())
                    {
                        if (part.meshBone == associatedBone)
                        {
                            smrCollider.transform.parent = part.transform;
                            break;
                        }
                    }
                }
            }
        }

        [Button]
        [Group("Tabs/Parts"), Tab("General")]
        public void AutoAssignParentPartsByBones()
        {
            Dictionary<Transform, RagdollPart> bonedParts = new Dictionary<Transform, RagdollPart>();
            foreach (RagdollPart part in GetComponentsInChildren<RagdollPart>())
            {
                if (part.meshBone != null)
                {
                    bonedParts[part.meshBone] = part;
                }
            }
            foreach (KeyValuePair<Transform, RagdollPart> bonedPart in bonedParts)
            {
                RagdollPart part = bonedPart.Value;
                Transform boneParent = bonedPart.Key.parent;
                while (boneParent != null)
                {
                    if (bonedParts.TryGetValue(boneParent, out RagdollPart parentPart))
                    {
                        part.parentPart = parentPart;
                        break;
                    }
                    boneParent = boneParent.parent;
                }
            }
        }

    }
}

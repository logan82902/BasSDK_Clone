using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.VFX;
using ThunderRoad.Skill.SpellPower;
using ThunderRoad.Modules;
using UnityEngine.Events;
using TriInspector;


namespace ThunderRoad
{
    [DeclareTabGroup("GroupTabs")]
    [DeclareFoldoutGroup("GroupTabs/General/Events", Expanded = false)]
    public class GolemController : ThunderEntity
    {
        [Header("References")]
        public Animator animator;
        [Group("GroupTabs"), Tab("General")]
        public CharacterController characterController;
        [Group("GroupTabs"), Tab("General")]
        public GolemAnimatorEvent animatorEvent;
        [Group("GroupTabs"), Tab("General")]
        public List<CollisionListener> headListeners = new();
        [Group("GroupTabs"), Tab("General")]
        public List<Rigidbody> bodyParts = new();
        [Group("GroupTabs"), Tab("General")]
        public List<Collider> bodyPartColliders = new();
        [Group("GroupTabs"), Tab("General")]
        public List<SkinnedMeshRenderer> skinnedMeshRenderers = new();
        [Group("GroupTabs"), Tab("General")]
        public List<Collider> ignoreCollisionColliders = new();
        [Group("GroupTabs"), Tab("General")]
        public List<Transform> magicSprayPoints = new();

        [Header("Global")]
        [Group("GroupTabs"), Tab("General")]
        public bool awakeOnStart = false;
        [Group("GroupTabs"), Tab("General")]
        public float moveSpeedMultiplier = 1.1f;
        [Group("GroupTabs"), Tab("General")]
        public float hitDamageMultiplier = 1f;
        [Group("GroupTabs"), Tab("General")]
        public float hitForceMultiplier = 1.1f;
        [Group("GroupTabs"), Tab("General")]
        public Renderer headRenderer;

        [Header("Head look")]
        [Group("GroupTabs"), Tab("General")]
        public MultiAimConstraint headAimConstraint;
        [Group("GroupTabs"), Tab("General")]
        public Transform headIktarget;
        [Group("GroupTabs"), Tab("General")]
        public float headLookSpeed = 3;
        [Group("GroupTabs"), Tab("General")]
        public LookMode lookMode = LookMode.Follow;
        [Group("GroupTabs"), Tab("General")]
        public LayerMask sightLayer = 1 << 0;
        [Group("GroupTabs"), Tab("General")]
        public float headLookSpeedMultiplier = 1;

        [Group("GroupTabs"), Tab("General")]
        public Transform eyeTransform;

        [Header("Face plate")]
        [Group("GroupTabs"), Tab("General")]
        public Rigidbody facePlateBody;
        [Group("GroupTabs"), Tab("General")]
        public SimpleBreakable facePlateBreakable;
        [Group("GroupTabs"), Tab("General")]
        public ConfigurableJoint facePlateJoint;
        [Group("GroupTabs"), Tab("General")]
        public float facePlateUnlockAngle = 5;
        [Group("GroupTabs"), Tab("General")]
        public float facePlateOpenAngle = 110;

        [Header("Head crystal")]
        [Group("GroupTabs"), Tab("General")]
        public ConfigurableJoint headCrystalJoint;
        [Group("GroupTabs"), Tab("General")]
        public VisualEffect headCrystalLinkVfx;
        [Group("GroupTabs"), Tab("General")]
        public ParticleSystem headCrystalParticle;
        [Group("GroupTabs"), Tab("General")]
        public FxController headCrystalEffectController;
        [Group("GroupTabs"), Tab("General")]
        public AudioSource headCrystalAudioSourceLoop;
        [Group("GroupTabs"), Tab("General")]
        public AudioSource headCrystalTearingAudioSource;

        [Group("GroupTabs"), Tab("General")]
        public AnimationCurve headCrystalLoopAudioPitchCurve;
        [Group("GroupTabs"), Tab("General")]
        public AnimationCurve headCrystalTearingAudioPitchCurve;
        [Group("GroupTabs"), Tab("General")]
        public AnimationCurve headCrystalTearingAudioVolumeCurve;
        [Group("GroupTabs"), Tab("General")]
        public Rigidbody headCrystalBody;
        [Group("GroupTabs"), Tab("General")]
        public Handle headCrystalHandle;
        [Group("GroupTabs"), Tab("General")]
        public float headCrystalGrabMass = 5f;
        [Group("GroupTabs"), Tab("General")]
        public float headCrystalGrabDrag = 100f;
        [Group("GroupTabs"), Tab("General")]
        public float headCrystalShutdownDuration = 8f;
        [Group("GroupTabs"), Tab("General")]
        public float headCrystalTearingDistance = 1;

        [Header("Death")]
        [Group("GroupTabs"), Tab("General")]
        public Vector2 stunCheckCapsuleHeights = new(1f, 2f);
        [Group("GroupTabs"), Tab("General")]
        public Vector3 radiusMinMaxCapsuleCast = new(1.5f, 3.5f, 6f);
        [Group("GroupTabs"), Tab("General")]
        public AudioSource killAudioSource;
        [Group("GroupTabs"), Tab("General")]
        public ParticleSystem killparticle;
        [Group("GroupTabs"), Tab("General")]
        public float killExplosionForce = 5;
        [Group("GroupTabs"), Tab("General")]
        public float killExplosionRadius = 5;
        [Group("GroupTabs"), Tab("General")]
        public float killExplosionUpward = 0.5f;
        [Group("GroupTabs"), Tab("General")]
        public ForceMode killExplosionForceMode = ForceMode.VelocityChange;
        [Group("GroupTabs"), Tab("General")]
        public Transform killExplosionSourceTransform;
        [Group("GroupTabs"), Tab("General")]
        public List<Transform> colliderResizeOnDeath;

        [Group("GroupTabs/General/Events")]
        public UnityEvent wakeEvent;
        [Group("GroupTabs/General/Events")]
        public UnityEvent startStunEvent;
        [Group("GroupTabs/General/Events")]
        public UnityEvent endStunEvent;
        [Group("GroupTabs/General/Events")]
        public UnityEvent crystalBreakEvent;
        [Group("GroupTabs/General/Events")]
        public UnityEvent defeatEvent;
        [Group("GroupTabs/General/Events")]
        public UnityEvent killEvent;

        [Header("Swing")]
        [MinMaxSlider(0, 50, true)]
        public Vector2 swingVelocity = new(2, 5);
        [Group("GroupTabs"), Tab("Abilities")]
        [Dropdown(nameof(GetAllEffectID), AppendToField = true)]
        public string swingEffectId = "GolemSwingArm";
        protected EffectData swingEffectData;

        [Group("GroupTabs"), Tab("Abilities")]
        public VelocityTracker[] swingTrackers = new VelocityTracker[2];
        
        [Group("GroupTabs"), Tab("Abilities")]
        public Transform handLeft;
        [Group("GroupTabs"), Tab("Abilities")]
        public Transform handRight;

        [Group("GroupTabs"), Tab("Abilities")]
        public Rigidbody[] armRigidbodies = new Rigidbody[2];

        [Header("Powers")]
        [Group("GroupTabs"), Tab("Abilities")]
        public Golem.Tier tier;
        [Group("GroupTabs"), Tab("Abilities")]
        [InlineEditor]
        public List<GolemAbility> abilities = new();
        
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        public enum AttackMotion
        {
            Rampage = 0,
            SwingRight = 1,
            SwingLeft = 2,
            ComboSwing = 3,
            ComboSwingAndSlam = 4,
            SwingBehindRight = 5,
            SwingBehindLeft = 6,
            SwingBehindRightTurnBack = 7,
            SwingBehindLeftTurnBack = 8,
            SwingLeftStep = 9,
            SwingRightStep = 10,
            Slam = 11,
            Stampede = 12,
            Breakdance = 13,
            SlamLeftTurn90 = 14,
            SlamRightTurn90 = 15,
            SwingLeftTurn90 = 16,
            SwingRightTurn90 = 17,
            Spray = 18,
            SprayDance = 19,
            Throw = 20,
            Beam = 21,
            SelfImbue = 22,
            RadialBurst = 23,
            ShakeOff = 24,
            LightShake = 25,
        }

        [Flags]
        public enum AttackSide
        {
            None  = 0,
            Left  = 0b01,
            Right = 0b10,
            Both  = 0b11
        }

        public static AttackSide GetAttackSide(AttackMotion attack) => attack switch
        {
            AttackMotion.Rampage => AttackSide.Both,
            AttackMotion.SwingRight => AttackSide.Left,
            AttackMotion.SwingLeft => AttackSide.Right,
            AttackMotion.ComboSwing => AttackSide.Both,
            AttackMotion.ComboSwingAndSlam => AttackSide.Both,
            AttackMotion.SwingBehindRight => AttackSide.Left,
            AttackMotion.SwingBehindLeft => AttackSide.Right,
            AttackMotion.SwingBehindRightTurnBack => AttackSide.Left,
            AttackMotion.SwingBehindLeftTurnBack => AttackSide.Right,
            AttackMotion.SwingLeftStep => AttackSide.Right,
            AttackMotion.SwingRightStep => AttackSide.Left,
            AttackMotion.Slam => AttackSide.Both,
            AttackMotion.Stampede => AttackSide.Both,
            AttackMotion.Breakdance => AttackSide.Both,
            AttackMotion.SlamLeftTurn90 => AttackSide.Right,
            AttackMotion.SlamRightTurn90 => AttackSide.Left,
            AttackMotion.SwingLeftTurn90 => AttackSide.Right,
            AttackMotion.SwingRightTurn90 => AttackSide.Left,
            AttackMotion.Spray => AttackSide.None,
            AttackMotion.SprayDance => AttackSide.Left,
            AttackMotion.Throw => AttackSide.None,
            AttackMotion.Beam => AttackSide.None,
            AttackMotion.SelfImbue => AttackSide.None,
            AttackMotion.RadialBurst => AttackSide.None,
            AttackMotion.ShakeOff => AttackSide.Both,
            _ => AttackSide.None,
        };

        public static AttackSide GetAttackSide(Side side) => side switch
        {
            Side.Left => AttackSide.Left,
            Side.Right => AttackSide.Right,
            _ => AttackSide.None
        };

        //[TabGroup("GroupTabs", "Debug", Order = 3)]
        [ShowInInspector]
        [ReadOnly]
        public bool animatorIsRoot { get; protected set; }
        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public State state { get; protected set; } = State.Inactive;
        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool isDefeated { get; protected set; }
        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool isKilled { get; protected set; }
        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool isLooking { get; protected set; }
        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public Transform lookingTarget { get; protected set; }

        // Animator states
        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [Range(0, 2)]
        protected int wakeMotion = 0;
        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool isAwake
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || UnityEditor.BuildPipeline.isBuildingPlayer || this.InPrefabScene()) return false;
#endif
                return animator.GetBool(awakeHash);
            }
            set { animator.SetBool(awakeHash, value); }
        }
        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool isBusy
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || UnityEditor.BuildPipeline.isBuildingPlayer || this.InPrefabScene()) return false;
#endif
                return animator.GetBool(isBusyHash);
            }
            set { animator.SetBool(isBusyHash, value); }
        }
        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool inMovement
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || UnityEditor.BuildPipeline.isBuildingPlayer || this.InPrefabScene()) return false;
#endif
                return animator.GetBool(inMovementHash);
            }
        }
        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool inAttackMotion
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || UnityEditor.BuildPipeline.isBuildingPlayer || this.InPrefabScene()) return false;
#endif
                return animator.GetBool(inAttackMotionHash);
            }
            set { animator.SetBool(inAttackMotionHash, value); }
        }
        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool isDeployed
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || UnityEditor.BuildPipeline.isBuildingPlayer || this.InPrefabScene()) return false;
#endif
                return animator.GetBool(isDeployedHash);
            }
            protected set => animator.SetBool(isDeployedHash, value);
        }

        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool deployInProgress
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || UnityEditor.BuildPipeline.isBuildingPlayer || this.InPrefabScene()) return false;
#endif
                return animator.GetBool(isDeployedHash) || animator.GetBool(deployStartedHash);
            }
        }
        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool isStunned
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || UnityEditor.BuildPipeline.isBuildingPlayer || this.InPrefabScene()) return false;
#endif
                return animator.GetBool(isStunnedHash);
            }
        }
        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool stunInProgress
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || UnityEditor.BuildPipeline.isBuildingPlayer || this.InPrefabScene()) return false;
#endif
                return animator.GetBool(isStunnedHash) || animator.GetBool(stunStartedHash);
            }
        }
        [Group("GroupTabs"), Tab("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool isActiveState
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || UnityEditor.BuildPipeline.isBuildingPlayer || this.InPrefabScene()) return false;
#endif             
                return isBusy || inAttackMotion || deployInProgress || stunInProgress;
            }
            
        }

        [ShowInInspector]
        [Group("GroupTabs"), Tab("Debug")]


        public Color HeadEmissionColor
        {
            get => headRenderer.material.GetColor(EmissionColor);
            set => headRenderer.material.SetColor(EmissionColor, value);
        }

        public static int awakeHash, wakeMotionHash, moveSpeedMultiplierHash, isBusyHash, moveHash, inMovementHash, locomotionMultHash, staggerHash, staggerLateralHash, staggerAxialHash, resistPushHash, attackHash, attackMotionHash, deployHash, deployStartedHash, isDeployedHash, stunHash, stunDirectionHash, stunStartedHash, isStunnedHash, inAttackMotionHash;

        public delegate void GolemStateChange(State newState);
        public event GolemStateChange OnGolemStateChange;

        public delegate void GolemAttackEvent(AttackMotion motion, GolemAbility ability);
        public event GolemAttackEvent OnGolemAttackEvent;

        public delegate void GolemRampageEvent();
        public event GolemRampageEvent OnGolemRampage;

        public delegate void GolemStaggerEvent(Vector2 direction);
        public event GolemStaggerEvent OnGolemStagger;

        public delegate void GolemStunEvent(float duration);
        public event GolemStunEvent OnGolemStun;

        public delegate void GolemInterrupt();
        public event GolemInterrupt OnGolemInterrupted;
        public event GolemInterrupt OnGolemHeadshotInterrupt;

        public delegate void GolemDealDamage(Creature target, float damage);
        public event GolemDealDamage OnDamageDealt;

        public enum State
        {
            Inactive = 0,
            WakingUp = 6,
            Active = 1,
            Stunned = 2,
            Rampage = 3,
            Defeated = 4,
            Dead = 5,
        }


        protected TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        private void GetBodyParts()
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        private void GetBodyPartColliders()
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public virtual void LookAt(Transform target)
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public virtual void UnlockFacePlate(bool open)
        {
        }

        [Button("Kill (SKIPS DEFEAT)")]
        [Group("GroupTabs"), Tab("Debug")]
        public virtual void Kill()
        {
            this.StartCoroutine(KillCoroutine());
        }
        public virtual IEnumerator KillCoroutine(){
            yield break;
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public virtual void Resurrect()
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public virtual void SetAwake(bool awake)
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public void Stun(float duration)
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public virtual void StopStun()
        {
        }

        public virtual void StaggerImpact(Vector3 point)
        {
        }

        public virtual void Stagger(float lateral, float axial) => Stagger(new Vector2(lateral, axial));

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public virtual void Stagger(Vector2 direction)
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public virtual void ResistPush(bool active)
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public virtual void PerformAttackMotion(AttackMotion meleeAttack, Action onMeleeEnd = null)
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public virtual void UseAbility(int index)
        {
        }

        public void StartAbility(GolemAbility ability, Action endCallback = null)
        {
        }

        public void EndAbility()
        {
        }

        public void SetMoveSpeedMultiplier(float value)
        {
            animator.SetFloat(moveSpeedMultiplierHash, value);
        }

        protected virtual void OnValidate()
        {
            if (!animator) animator = this.GetComponentInChildren<Animator>();
            if (!animatorEvent) animatorEvent = this.GetComponentInChildren<GolemAnimatorEvent>();
            if (!headAimConstraint) headAimConstraint = this.GetComponentInChildren<MultiAimConstraint>();
            if (!characterController) characterController = this.GetComponentInParent<CharacterController>();
            //remove any null skinned mesh renderers
            if (!skinnedMeshRenderers.IsNullOrEmpty()) skinnedMeshRenderers.RemoveAll(x => x == null);
            if (skinnedMeshRenderers.IsNullOrEmpty()) skinnedMeshRenderers = new List<SkinnedMeshRenderer>(this.GetComponentsInChildren<SkinnedMeshRenderer>());
            
            if (bodyParts.Count == 0) GetBodyParts();
            if (Application.isPlaying) SetMoveSpeedMultiplier(moveSpeedMultiplier);
        }

    }

    public enum LookMode
    {
        Follow,
        HorizontalSweep,
        VerticalSweep,
    }
}
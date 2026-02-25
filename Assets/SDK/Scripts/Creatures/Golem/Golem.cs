using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using TriInspector;

namespace ThunderRoad
{
    [DeclareTabGroup("GroupTabs")]
    [DeclareTabGroup("GroupTabs/AI/CrystalTiers")]
    public class Golem : GolemController
    {
        public static Golem local;

        [Header("AI")]
        //[TabGroup("GroupTabs", "AI", Order = 1)]
        public NavMeshAgent navMeshAgent;
        [Group("GroupTabs"), Tab("AI")]
        public float cycleTime = 0.5f;
        [Group("GroupTabs"), Tab("AI")]
        public float forwardAngle = 30f;
        [Group("GroupTabs"), Tab("AI")]
        public float stopMoveDistance = 5.5f;
        [Group("GroupTabs"), Tab("AI")]
        public float stopMoveSphereHeight = 1f;
        [Group("GroupTabs"), Tab("AI")]
        public float minHeadToTargetDistance = 20f;
        private Vector2 velocity;
        private Vector2 smoothDeltaPosition;
        private float animationDampTime = 0.1f;

        [Header("Awake")]
        [Group("GroupTabs"), Tab("AI")]
        public bool awakeWhenTargetClose = true;
        [Group("GroupTabs"), Tab("AI")]
        public float awakeTargetDistance = 30;
        protected float awakeTime;

        [Header("Melee")]
        [Group("GroupTabs"), Tab("AI")]
        public bool allowMelee = true;
        [Group("GroupTabs"), Tab("AI")]
        public float meleeMaxAttackDistance = 7f;
        [FormerlySerializedAs("abilityCooldownInMelee")]
        [Group("GroupTabs"), Tab("AI")]
        public float abilityCooldown = 15;
        [Group("GroupTabs"), Tab("AI")]
        public Vector2 meleeMinMaxCooldown = new(1f, 3f);
        [Group("GroupTabs"), Tab("AI")]
        public List<MeleeAttackRange> attackRanges;
        private bool setMeleeCooldown = false;
        private float nextMeleeAttackTime;

        [Group("GroupTabs"), Tab("AI")]
        public bool blockActionWhileClimbed = true;
        [Group("GroupTabs"), Tab("AI")]
        public bool allowClimbReact = false;
        [Group("GroupTabs"), Tab("AI")]
        public AudioSource climbReactWarningAudio;
        [Group("GroupTabs"), Tab("AI")]
        public float climbReactWarningTime = 2f;
        [Group("GroupTabs"), Tab("AI")]
        public float climbReactTime = 5f;
        [Group("GroupTabs"), Tab("AI")]
        public float resetClimbedTime = 1f;
        [Group("GroupTabs"), Tab("AI")]
        public List<GolemAbility> climbReacts;
        private bool wasClimbed = false;
        private float climbTime = 0f;
        private int climbReactIntensity = 0;
        private float lastReactWarningTime = 0f;

        [Header("Crystals")]
        [Group("GroupTabs"), Tab("AI")]
        public WeakPointRandomizer golemCrystalRandomizer;
        [Group("GroupTabs"), Tab("AI")]
        public WeakPointRandomizer arenaCrystalRandomizer;
        [Group("GroupTabs/AI/CrystalTiers"), Tab("Default Crystals")]
        [InlineProperty]
        [HideLabel]
        public CrystalConfig defaultConfig;
        [Group("GroupTabs/AI/CrystalTiers"), Tab("Tier 1 Crystals")]
        [InlineProperty]
        [HideLabel]
        public CrystalConfig tier1Config;
        [Group("GroupTabs/AI/CrystalTiers"), Tab("Tier 2 Crystals")]
        [InlineProperty]
        [HideLabel]
        public CrystalConfig tier2Config;
        [Group("GroupTabs/AI/CrystalTiers"), Tab("Tier 3 Crystals")]
        [InlineProperty]
        [HideLabel]
        public CrystalConfig tier3Config;

        public CrystalConfig activeCrystalConfig
        {
            get
            {
                switch (tier)
                {
                    case Tier.Tier1: return tier1Config;
                    case Tier.Tier2: return tier2Config;
                    case Tier.Tier3: return tier3Config;
                }
                return defaultConfig;
            }
        }

        [Group("GroupTabs"), Tab("AI")]
        public bool rampageOnCrystalBreak = true;
        [Group("GroupTabs"), Tab("AI")]
        public List<GolemAbility> crystalBreakReactions = new();
        [Group("GroupTabs"), Tab("AI")]
        public float normalRampageChance = 0.4f;
        [Group("GroupTabs"), Tab("AI")]
        public float rampageDamageMult = 2f;
        [Group("GroupTabs"), Tab("AI")]
        public float rampageForceMult = 1.5f;
        public TriDropdownList<string> GetAllItemID() => Catalog.GetDropdownAllID(Category.Item);

        [Group("GroupTabs"), Tab("AI")]
        [Dropdown(nameof(GetAllItemID), AppendToField = true)]
        public string shardItemId = "CrystalShard";
        protected ItemData shardItemData;
        protected int shardsToDrop = 0;

        [Group("GroupTabs"), Tab("AI")]
        [Range(1, 10)]
        public int crystalsBrokenToWake = 1;
        private int crystalsBrokenDuringStun = 0;
        [Group("GroupTabs"), Tab("AI")]
        public float shieldDisableDelay = 0f;
        [Group("GroupTabs"), Tab("AI")]
        [ColorPickerField(Alpha = true, HDR = true)]
        public Color defeatedEmissionColor;

        [Group("GroupTabs"), Tab("Debug")]
        public bool autoDefeatOnStart;
        [Group("GroupTabs"), Tab("Debug")]
        public bool autoDefeatAndKillOnStart;

        [ShowInInspector]
        [Group("GroupTabs"), Tab("Debug")]
        public bool isProtected => (linkedArenaCrystals?.Count ?? 0) > 0;
        [ShowInInspector]
        [Group("GroupTabs"), Tab("Debug")]
        public int crystalsLeft => crystals.Count;
        [NonSerialized]
        public List<GolemCrystal> crystals = new List<GolemCrystal>();
        [NonSerialized]
        public List<GolemCrystal> linkedArenaCrystals = new List<GolemCrystal>();
        protected int disableArenaCrystalShieldIndex = 0;
        private Quaternion climbInitialRelativeRotation;

        public delegate void GolemCrystalBreak(GolemCrystal crystal);
        public event GolemCrystalBreak OnGolemCrystalBreak;
        public event GolemCrystalBreak OnArenaCrystalBreak;

        public static event Action OnLocalGolemSet;

        [Serializable]
        [DeclareHorizontalGroup("Counts")]
        public class CrystalConfig
        {
            [Group("Counts")]
            public int golemCrystals = 8;
            [Group("Counts")]
            public int arenaCrystals = 3;
            public float arenaCrystalNearbyStun = 10f;
            public float arenaCrystalMaxStun = 30f;
            public Vector2 minMaxShardsDropped = new Vector2(8, 16);
        }

        [Serializable]
        [DeclareFoldoutGroup("$dynamicTitle", Expanded = false)]
        [DeclareVerticalGroup("$dynamicTitle/Fields")]
        [DeclareHorizontalGroup("$dynamicTitle/Fields/MinMaxes")]
        public class AttackRange
        {
            [Group("$dynamicTitle/Fields/MinMaxes")]
            [LabelWidth(120f)]
            public Vector2 angleMinMax;
            [Group("$dynamicTitle/Fields/MinMaxes")]
            [LabelWidth(120f)]
            public Vector2 distanceMinMax;

            public virtual string dynamicTitle => "Attack Range";

            public bool CheckAngleDistance(float targetAngle, float targetDistance)
            {
                return targetAngle >= angleMinMax.x && targetAngle <= angleMinMax.y && targetDistance >= distanceMinMax.x && targetDistance <= distanceMinMax.y;
            }
        }

        [System.Flags]
        public enum Tier
        {
            Any = 0,
            Tier1 = 1,
            Tier2 = 2,
            Tier3 = 4,
        }

        [Serializable]
        [DeclareVerticalGroup("$dynamicTitle/Fields")]
        public class MeleeAttackRange : AttackRange
        {
            [Space]
            [Group("$dynamicTitle/Fields")]
            //[ListDrawerSettings(ShowFoldout = true)]
            public WeightedAttack[] attackOptions;

            [Serializable]
            [DeclareHorizontalGroup("Attack")]
            public class WeightedAttack
            {
                [Group("Attack")]
                [LabelWidth(60f)]
                public AttackMotion attack;
                [Group("Attack")]
                [LabelWidth(60f)]
                public float weight = 1f;
            }

            public override string dynamicTitle
            {
                get
                {
                    string result = attackOptions.IsNullOrEmpty() ? "None" : "";
                    for (int i = 0; i < (attackOptions?.Length ?? 0); i++)
                    {
                        WeightedAttack attack = attackOptions[i];
                        result += $"{attack.attack.ToString()} ({attack.weight.ToString("0.00")})";
                        if (i != attackOptions.Length - 1) result += ", ";
                    }
                    return result;
                }
            }

            public bool TryUseRange(float targetAngle, float targetDistance, out AttackMotion attack, AttackMotion lastAttack = AttackMotion.Rampage)
            {
                attack = attackOptions[UnityEngine.Random.Range(0, attackOptions.Length)].attack;
                if (!attackOptions.WeightedFilteredSelectInPlace(at => at.attack != lastAttack, at => at.weight, out WeightedAttack weightedAttack)) return false;
                attack = weightedAttack.attack;
                return CheckAngleDistance(targetAngle, targetDistance);
            }
        }

        [Serializable]
        public class InflictedStatus
        {
            private TriDropdownList<string> GetAllStatuses => Catalog.GetDropdownAllID<StatusData>();

            [Dropdown(nameof(GetAllStatuses), AppendToField = true)]
            public string data;
            public float duration = 3f;
            public float parameter = 0f;
        }


        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public override void SetAwake(bool awake)
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public virtual void SetAttackTarget(Transform targetTransform)
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public void Rampage()
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public void Rampage(RampageType type = RampageType.Melee)
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public virtual void Defeat()
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public void BreakCrystals(int num = 1)
        {
        }

        [Button]
        [Group("GroupTabs"), Tab("Debug")]
        public void BreakArenaCrystals(int num = 1)
        {
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (!navMeshAgent) navMeshAgent = this.GetComponentInChildren<NavMeshAgent>();
            if (!golemCrystalRandomizer) golemCrystalRandomizer = this.GetComponentInChildren<WeakPointRandomizer>();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0f, stopMoveSphereHeight, 0f), stopMoveDistance);
            Gizmos.color = Color.blue;
            if (navMeshAgent.path?.corners is Vector3[] corners)
            {
                for (int i = 0; i < corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(corners[i], corners[i + 1]);
                }
            }
        }
#endif

    }
}

using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using Plane = UnityEngine.Plane;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Arc Wire", Title = "Arc Wire")]
    [DeclareBoxGroup("Electrocution", Title = "Electrocution")]
    public class SkillArcwire : SpellSkillData
    {
        [Group("Arc Wire")]
        public float beamShockDuration = 3f;

        [Group("Arc Wire")]
        public float beamThickness = 0.1f;

        [Group("Arc Wire")]
        public float beamDuration = 5f;

        [Group("Arc Wire")]
        public float minBeamNodeDistance = 0.3f;

        [Group("Arc Wire")]
        public float minBeamNodeInputVelocity = 0f;

        [Group("Arc Wire")]
        public float maxBeamNodeInputVelocity = 8f;

        [Group("Arc Wire")]
        public float beamNodeVelocityMult = 2;

        [Group("Arc Wire")]
        public float beamNodeDrag = 3f;

        [Group("Arc Wire")]
        public float imbueCostPerBeamNode = 2;

        [Group("Arc Wire")]
        public float dismembermentMaxAngle = 60;

        [Group("Arc Wire")]
        public float sliceWidthMultiplier = 2.5f;

        [Group("Arc Wire")]
        [Dropdown(nameof(GetAllEffectID))]
        public string arcEffectId;

        [Group("Arc Wire")]
        public int arcWirePushLevel = 1;

        [Group("Arc Wire")]
        public float playerBumpForceMultiplier = 1f;

        [Group("Arc Wire")]
        public float playerHitDamage = 0f;

        [Group("Arc Wire")]
        public bool allowChargeSapping = true;

        [Group("Electrocution")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string electrocuteStatusId = "Electrocute";
        [NonSerialized]
        public StatusData electrocuteStatusData;

        [NonSerialized]
        public EffectData arcEffectData;

        [Group("Arc Wire")]
        [Dropdown(nameof(GetAllEffectID))]
        public string arcStaffEffectId;

        [Group("Arc Wire")]
        [Dropdown(nameof(GetAllEffectID))]
        public string arcWireHitEffectId;

        [NonSerialized]
        public EffectData arcStaffEffectData;
        [NonSerialized]
        public EffectData arcWireHitEffectData;

        public static float lastEffectSpawn;

    }

    public class ArcWire : ThunderBehaviour
    {
        public bool drawingArcs;
        public Side drawingSide;
        public LightningTrailNode prevNode;
        public LightningTrailNode nextNode;
        public float arcWhooshIntensity;
        public Transform shootPoint;
        public PhysicBody sourceBody;
        public SkillArcwire skill;
        public Creature caster;

        private bool end;
        private float lastChargeSap;

        public override ManagedLoops EnabledManagedLoops => ManagedLoops.Update;

    }

    public class LightningTrailNode : ThunderBehaviour
    {
        public static HashSet<LightningTrailNode> all = new();
        private SkillArcwire skill;
        private CapsuleCollider trigger;
        public Rigidbody rb;
        public float? duration;
        private LightningTrailNode nextNode;
        private bool active;
        private float startTime;

        public Creature ignored;
        public bool tips;

    }
}

using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Effect", Title = "Effect")]
    [DeclareBoxGroup("Status Effect", Title = "Status Effect")]
    [DeclareBoxGroup("Damage", Title = "Damage")]
    [DeclareBoxGroup("Detection", Title = "Detection")]
    public class SkillHeatwave : SpellSkillData
    {
        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string effectId;

        [Group("Status Effect")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusEffectId = "Burning";
        [NonSerialized]
        public StatusData status;

        [Group("Damage")]
        public float radius = 1.5f;
        [Group("Damage")]
        public int pushLevel = 1;
        [Group("Damage")]
        public float force = 10f;

        [Group("Damage")]
        public float damage = 10f;

        [Group("Damage")]
        public float heat = 40f;

        [Group("Detection")]
        public float punchStartMinMagnitude = 4;
        [NonSerialized]
        public float punchStartMinSqrMagnitude;

        [Group("Detection")]
        public float punchStopMaxMagnitude = 0.5f;
        [NonSerialized]
        public float punchStopMaxSqrMagnitude;

        [Group("Detection")]
        public float punchStopWindow = 0.1f;
        [Group("Detection")]
        public float punchDelay = 0.3f;
        
        [NonSerialized]
        public EffectData effectData;


    }

    public class HeatwavePuncher : ThunderBehaviour
    {
        public override ManagedLoops EnabledManagedLoops => ManagedLoops.Update;

        public SkillHeatwave skill;
        public SpellCastProjectile fire;
        public RagdollHand hand;

        public bool active;

        public float lastPunchingTime;
        public Vector3 lastVelocity;
        private float lastPunch;

    }
}

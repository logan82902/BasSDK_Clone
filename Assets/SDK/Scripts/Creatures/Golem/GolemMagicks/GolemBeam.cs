using System;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad.Skill.SpellMerge;
using System.Linq;
using ThunderRoad.Skill.Spell;
using Plane = UnityEngine.Plane;
using Random = UnityEngine.Random;
using TriInspector;

namespace ThunderRoad
{
    [CreateAssetMenu(menuName = "ThunderRoad/Creatures/Golem/Beam config")]
    [DeclareBoxGroup("Beam", Title = "Beam")]
    [DeclareBoxGroup("Effects", Title = "Effects")]
    [DeclareBoxGroup("Impact", Title = "Impact")]
    [DeclareBoxGroup("Timing", Title = "Timing")]
    [DeclareBoxGroup("Trail", Title = "Trail")]
    public class GolemBeam : GolemAbility
    {
        public static float lastBeamTime;

        [Header("Beam")]
        [Group("Beam")]
        public float hitRange = 30f;
        [Group("Beam")]
        public float radius = 1f;
        [Group("Beam")]
        public float headSweepRange = 5;
        [Group("Beam")]
        public bool lockSweep;
        [Group("Beam")]
        public LayerMask beamRaycastMask;
        [Group("Beam")]
        public float beamMaxDistance = 50;
        [Group("Beam")]
        public float beamStartMaxAngle = 30;
        [Group("Beam")]
        public float beamAngleSoftMax = 60;
        [Group("Beam")]
        public float beamAngleHardMax = 80;


        [Header("Effects")]
#if UNITY_EDITOR
        [Group("Effects")]
        [Dropdown(nameof(GetAllEffectID))]
#endif
        public string chargeEffectID = "";
#if UNITY_EDITOR
        protected TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);
#endif
#if UNITY_EDITOR
        [Group("Effects")]
        [Dropdown(nameof(GetAllEffectID))]
#endif
        public string beamEffectID = "";
        [Group("Effects")]
        public bool stopChargeOnShoot = false;
        
        [Header("Impact")]
        [Group("Impact")]
        public float damagePerSecond = 5f;
        [Group("Impact")]
        public float damagePeriodTime = 0.1f;
        [Group("Impact")]
        public bool blockable = true;
        [Group("Impact")]
        public bool deflectOnBlock = true;
        [Group("Impact")]
        public float appliedForce = 0f;
        [Group("Impact")]
        public float blockedPushForce = 0f;
        [Group("Impact")]
        public ForceMode appliedForceMode = ForceMode.Force;
        [TableList]
        [Group("Impact")]
        public List<Golem.InflictedStatus> appliedStatuses = new();
        
        [Header("Timing")]
        [Group("Timing")]
        public float beamCooldownDuration = 20;
        [Group("Timing")]
        public float beamStopDelayTargetLost = 3;
        [Group("Timing")]
        public Vector2 beamingMinMaxDuration = new(5, 10);

        public enum State
        {
            Deploying,
            Firing,
            Finished
        }
        
        [Header("Trail")]
        [Group("Trail")]
        public bool leaveMoltenTrail;
        private TriDropdownList<string> GetAllSpellIDs() => Catalog.GetDropdownAllID<SpellData>();

        [Group("Trail")]
        [Dropdown(nameof(GetAllSpellIDs))]
        [ShowIf(nameof(leaveMoltenTrail))]
        public string moltenBeamSpellId = "PlasmaBeam";

    }
}

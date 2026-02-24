using System;
using UnityEngine;
using ThunderRoad.Skill.SpellPower;

using TriInspector;

namespace ThunderRoad.Skill
{
    [DeclareBoxGroup("Limb Tearing", Title = "Limb Tearing")]
    [DeclareBoxGroup("Karate Chop", Title = "Karate Chop")]
    [DeclareBoxGroup("Karate Chop Damager", Title = "Karate Chop Damager")]
    public class SkillBruteForce : SkillData
    {
        [Group("Limb Tearing")]
        public float minBreakForceMultiplier = 1.5f;

        [Group("Karate Chop")]
        public bool overrideMinSpeed = false;
        [Group("Karate Chop")]
        public float chopMinSpeed = 6f;
        [Group("Karate Chop Damager")]
        [Dropdown(nameof(GetAllDamagerID))]
        public string damagerId;
        protected DamagerData damagerData;
        [Group("Karate Chop Damager")]
        public int damagerTier = 3;
        [Group("Karate Chop Damager")]
        public bool ignoreArmor = false;
        [Group("Karate Chop Damager")]
        [Dropdown(nameof(GetAllMaterialID))]
        public string materialId;

        [Group("Karate Chop")]
        [Dropdown(nameof(GetAllHandPoseID))]
        public string karateChopTargetPoseId;

        [NonSerialized]
        public HandPoseData karateChopTargetPose;

        [Group("Karate Chop")]
        [Dropdown(nameof(GetAllHandPoseID))]
        public string karateChopDefaultPoseId;

    }

}
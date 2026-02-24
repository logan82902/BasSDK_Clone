using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using ThunderRoad.AI;
using ThunderRoad.AI.Action;
using UnityEngine.AI;
using Newtonsoft.Json;
using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BrainModuleMelee : BrainModuleAttack
    {
        public const float parryMinVelocity = 2f;
        public bool meleeEnabled = true;
        public float attackMaxAngle = 45;
        public float attackTurnSpeedMultiplier = 1;
        public float maxRangeDelta = 0.25f;

        [Header("Recoil and riposte")]
        public bool damageToCancelIsPercentValue = true;
        public float minimumDamageToCancelAttack = 5f;
        public float recoilMinWeaponSpeed = 1;
        [MinMaxSlider(0f, 1f, true)]
        public Vector2 recoilNothingRiposteChance = new(0.25f, 0.75f);
        [Tooltip("This value gets added to the above recoil-nothing-riposte chance, multiplied by the number of consecutive ripostes. This changes the chance to recoil, and chance to riposte.")]
        public Vector2 recoilNothingRiposteModifier = new Vector2(0f, 0f);
        public Vector2 minMaxRiposteDelay = new Vector2(0.75f, 1.5f);
        public float riposteSpeedAdditiveModifier = -0.05f;
        public float physicCullBlockMinimum = 0.1f;

        [Header("Defense settings")]
        public float damagedIgnoreDefenseWindow = 0.75f;

        [Header("Grouping")]
        public int meleeMax = 3;
        public float meleeSafeDistance = 3f;

        // Weapon is rarely totally horizontal when striking
        public float weaponReachRatio = 0.75f;

        public bool updateClipRange = true;

        [Header("Forces")]
        public float armSpringMultiplier = 2.0f;
        public float armDamperMultiplier = 0;
        public float armMaxForceMultiplier = 10.0f;
        [MinMaxSlider(0f, 100f, true)]
        public Vector2 minMaxPhysicCullWeaponSpeed = new(2.5f, 25f);
        public int immediatePushFrameDelay = 2;
        public float maxPushItemMass = 2.99f;
        public float shieldPushMassMult = 1.5f;

        [Header("Animation")]
        public float animationSpeedMultiplier = 1;
        [Range(0f, 1f)]
        public float minimumWeaponMassMultiplier = 0.7f;

        public enum HitType { None, Object, Weapon, Shield, Body }

        [Header("Instance")]
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public float lastAttackStartTime;
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public float lastAttackEndTime;
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public float nextAttackDelay;
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public HitType lastHitType;
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public float lastRiposteTime;
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public float nextRiposteDelay;
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public int consecutiveRipostes;

        public TriDropdownList<string> GetAllAnimationID() => Catalog.GetDropdownAllID(Category.Animation);

    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad
{
    [CreateAssetMenu(menuName = "ThunderRoad/Creatures/Golem/Throw config")]
    [DeclareBoxGroup("Thrown Object", Title = "Thrown Object")]
    [DeclareBoxGroup("Throw", Title = "Throw")]
    [DeclareBoxGroup("Explosion", Title = "Explosion")]
    public class GolemThrow : GolemAbility
    {
        public static float lastThrowTime;
#if UNITY_EDITOR
        [Group("Thrown Object")]
        [Dropdown(nameof(GetAllEffectID))]
#endif
        public string summonEffectID;
        private TriDropdownList<string> GetAllItemID() => Catalog.GetDropdownAllID(Category.Item);
        private TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

        [Group("Thrown Object")]
        [Dropdown(nameof(GetAllItemID))]
        public string throwObjectID;
#if UNITY_EDITOR
        [Group("Thrown Object")]
        [Dropdown(nameof(GetAllEffectID))]
#endif
        public string objectEffectID;
        [Group("Thrown Object")]
        public AnimationCurve objectEffectIntensityCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [Group("Throw")]
        public LayerMask objectSpawnRaycastMask;
        [Group("Throw")]
        public float throwVelocity = 5f;
        [Group("Throw")]
        public float throwCooldownDuration = 20;
        [Group("Throw")]
        public float throwMaxDistance = 50;
        [Group("Throw")]
        public float throwMaxAngle = 30;
        [Group("Throw")]
        public float gravityMultiplier = 1f;
        [Group("Throw")]
        public Side grabArmSide = Side.Right;
        [Group("Throw")]
        public Vector3 holdPosition = Vector3.zero;
        [Group("Throw")]
        public float holdForce;
        [Group("Throw")]
        public float holdDamper;
        [Group("Explosion")]
        public float explosionRadius = 10f;
        [Group("Explosion")]
        public float explosionDamage = 20;
#if UNITY_EDITOR
        [Group("Explosion")]
#endif
        public List<Golem.InflictedStatus> appliedStatuses = new();
        [Group("Explosion")]
        public float explosionForce = 20;
        [Group("Explosion")]
        public ForceMode forceMode = ForceMode.Impulse;
        [Group("Explosion")]
        public float upwardForceMult = 1f;
        [Group("Explosion")]
        public LayerMask explosionLayerMask;

        [Group("Explosion")]
#if UNITY_EDITOR
        [Dropdown(nameof(GetAllEffectID))]
#endif
        public string explosionEffectID;

    }
}

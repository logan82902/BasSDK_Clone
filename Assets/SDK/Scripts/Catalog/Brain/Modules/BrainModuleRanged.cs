using UnityEngine;
using System;
using System.Collections.Generic;
using ThunderRoad.AI.Action;

using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    [DeclareBoxGroup("Speed", Title = "Speed")]
    [DeclareBoxGroup("Force", Title = "Force")]
    public abstract class BrainModuleRanged : BrainModuleAttack
    {
        public float aimMaxAngleHorizontal = 30;
        [Group("Speed")]
        public float aimMoveSpeed = 10;
        public float turnSpeed = 1;
        public Vector2 minMaxTimeToAttackFromAim = new(0, 4);
        [Group("Force")]
        public float aimArmSpringMultiplier = 0.5f;

        public float aimTime { get; protected set; }

        [ShowInInspector]
        [ReadOnly]
        public AttackState state = AttackState.None;

        public enum AttackState
        {
            None,
            Arming, //BowState Reloading
            Aiming,
            Attacking, //BowState Shooting
            AttackDone, //BowState ShootDone
        }

    }
}
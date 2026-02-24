using System;
using System.Collections.Generic;
using UnityEngine;


using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    public class MeleeIdle : IdlePose
    {
        [LabelWidth(150)]
        [Group("StanceNode/Horiz/Fields/Row3")]
        public bool requireThrustWeapon = false;

        public override bool showDifficulty => true;

        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        [ShowIf("isPlaying")]
        public List<AttackMotion> attackMotions;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        [ShowIf("isPlaying")]
        public AttackMotion lastPick;

        [NonSerialized]
        public float targetMinRange = 0f;
        [NonSerialized]
        public float targetMaxRange = 0f;
        [NonSerialized]
        public float targetMinReachRange = 0f;
        [NonSerialized]
        public float targetMaxReachRange = 0f;
        [NonSerialized]
        public float realMinRange = 0f;
        [NonSerialized]
        public float realMaxRange = 0f;
        [NonSerialized]
        public float realMinReachRange = 0f;
        [NonSerialized]
        public float realMaxReachRange = 0f;



    }
}

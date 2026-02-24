using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TriInspector;

namespace ThunderRoad.Skill.SpellMerge
{
    [DeclareBoxGroup("Physics", Title = "Physics")]
    [DeclareBoxGroup("Force", Title = "Force")]
    public class SpellMergeNeutronCore : SpellMergeFire
    {
        [Group("Physics")]
        public float drag = 1f;
        [Group("Physics")]
        public float dragDelay = 1f;
        [Group("Force")]
        public float forceMultiplier = -1;
        [Group("Force")]
        public AnimationCurve forceDistanceCurve;

        [Group("Force")]
        public AnimationCurve forceTimeCurve;

        [Group("Force")]
        public float duration = 5;

        [Group("Force")]
        public float guidanceAmount = 20;

        [Group("Force")]
        public float radius = 4;
    }
}
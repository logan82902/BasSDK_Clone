using UnityEngine;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;


using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BrainModuleStance : BrainData.Module
    {
        public enum Stance
        {
            Idle,
            Fists,
            MeleeShield,
            Melee1H,
            Melee2H,
            DualWield,
            Bow,
            Flee,
            Staff,
            PoleWeapon,
        }

        public float subStanceSwitchTime = 0.25f;
        public AnimationCurve switchCurve = new(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
        [MinMaxSlider(0f, 20f, true)]
        public Vector2 minMaxTimeBetweenChangeSubStance = new();
        public bool forceHoldUpperOff = false;

        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        [JsonIgnore]
        public Stance stance = Stance.Idle;

    }
}
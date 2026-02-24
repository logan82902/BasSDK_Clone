using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareHorizontalGroup("Intensity")]
    [DeclareBoxGroup("Intensity/Curve", Title = "Curve")]
    [DeclareBoxGroup("Intensity/Smoothing", Title = "Smoothing")]
    [DeclareHorizontalGroup("Range")]
    [DeclareBoxGroup("Range/Curve", Title = "Curve")]
    [DeclareBoxGroup("Range/Smoothing", Title = "Smoothing")]
    [DeclareHorizontalGroup("Color")]
    [DeclareBoxGroup("Color/Curve", Title = "Curve")]
    [DeclareBoxGroup("Color/Smoothing", Title = "Smoothing")]
    [DeclareBoxGroup("Flicker", Title = "Flicker")]
    public class EffectModuleLight : EffectModule
    {
        [Group("General")]
        public Quaternion localRotation;
        [LabelWidth(150f)]
        [Group("Intensity/Curve")]
        public AnimationCurve intensityCurve = AnimationCurve.Constant(0, 1, 1);
        [Group("Intensity/Smoothing")]
        public float intensitySmoothFactor = 0;

        [LabelWidth(150f)]
        [Group("Range/Curve")]
        public AnimationCurve rangeCurve = AnimationCurve.Constant(0, 1, 1);

        [Group("Range/Smoothing")]
        public float rangeSmoothFactor = 0;

        [LabelWidth(150f)]
        [Group("Color/Curve")]
        public Color colorStart;

        [Group("Color/Curve")]
        public Color colorEnd;

        [NonSerialized]
        [JsonIgnore]
        protected Gradient colorCurve;

        [Group("Color/Smoothing")]
        public float colorSmoothFactor = 0;
        [Group("General")]
        public float loopFadeDelay = 0;

        [Group("Flicker")]

        public AnimationCurve flickerIntensityCurve = AnimationCurve.Constant(0, 1, 1);
        [Group("Flicker")]

        public AnimationCurve flickerRateCurve = AnimationCurve.Constant(0, 1, 0);

    }
}

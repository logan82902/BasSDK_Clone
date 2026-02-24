using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [DeclareBoxGroup("Color", Title = "Color")]
    [DeclareBoxGroup("Color (HDR)", Title = "Color (HDR)")]
    [DeclareHorizontalGroup("Color (HDR)/MainColor")]
    [DeclareHorizontalGroup("Color (HDR)/SecondaryColor")]
    [DeclareBoxGroup("Color (Non HDR)", Title = "Color (Non HDR)")]
    [DeclareHorizontalGroup("Color (Non HDR)/MainNoHdr")]
    [DeclareHorizontalGroup("Color (Non HDR)/SecondaryNoHdr")]
    public class EffectModuleShader : EffectModule
    {
        public EffectTarget linkBaseColor = EffectTarget.None;
        public EffectTarget linkEmissionColor = EffectTarget.None;
        public EffectTarget linkExtraColor = EffectTarget.None;

        public string extraPropertyName;
        protected int extraPropertyId;

        public float lifeTime = 5;
        public float refreshSpeed = 0.1f;
        public bool useSecondaryRenderer;

        [Group("Color")]
        [NonSerialized]
        [JsonIgnore]
        [GradientUsage(true)]
        private Gradient mainGradient;


        [Group("Color")]
        [NonSerialized]
        [JsonIgnore]
        [GradientUsage(true)]
        private Gradient secondaryGradient;


        [Group("Color")]
        [NonSerialized]
        [JsonIgnore]
        private Gradient mainGradientNoHdr;


        [Group("Color")]
        [NonSerialized]
        [JsonIgnore]
        private Gradient secondaryGradientNoHdr;


        [Group("Color (HDR)/MainColor")]
        [ColorPickerField(Alpha = true, HDR = true)]
        [SerializeField]
        public Color mainColorStart;
        [Group("Color (HDR)/MainColor")]
        [ColorPickerField(Alpha = true, HDR = true)]
        [SerializeField]
        public Color mainColorEnd;
        [Group("Color (HDR)/SecondaryColor")]
        [ColorPickerField(Alpha = true, HDR = true)]
        [SerializeField]
        public Color secondaryColorStart;
        [Group("Color (HDR)/SecondaryColor")]
        [ColorPickerField(Alpha = true, HDR = true)]
        [SerializeField]
        public Color secondaryColorEnd;

        [Group("Color (Non HDR)/MainNoHdr")]
        [SerializeField]
        public Color mainNoHdrColorStart;
        [Group("Color (Non HDR)/MainNoHdr")]
        [SerializeField]
        public Color mainNoHdrColorEnd;
        [Group("Color (Non HDR)/SecondaryNoHdr")]
        [SerializeField]
        public Color secondaryNoHdrColorStart;
        [Group("Color (Non HDR)/SecondaryNoHdr")]
        [SerializeField]
        public Color secondaryNoHdrColorEnd;
        public static List<EffectShader> pool = new List<EffectShader>();
        public static Transform poolRoot;


        public override void Clean()
        {
        }

        public override void CopyHDRToNonHDR()
        {
        }


    }
}

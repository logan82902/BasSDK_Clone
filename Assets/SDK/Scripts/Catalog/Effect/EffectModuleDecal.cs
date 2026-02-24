using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [DeclareBoxGroup("Decal", Title = "Decal")]
    [DeclareBoxGroup("Size", Title = "Size")]
    [DeclareBoxGroup("Color", Title = "Color")]
    [DeclareBoxGroup("Color (HDR)", Title = "Color (HDR)")]
    [DeclareHorizontalGroup("Color (HDR)/MainColor")]
    [DeclareHorizontalGroup("Color (HDR)/SecondaryColor")]
    [DeclareBoxGroup("Color (Non HDR)", Title = "Color (Non HDR)")]
    [DeclareHorizontalGroup("Color (Non HDR)/MainNoHdr")]
    [DeclareHorizontalGroup("Color (Non HDR)/SecondaryNoHdr")]
    public class EffectModuleDecal : EffectModule
    {
        [Group("Decal")]
        public string materialAddress;
        [NonSerialized]
        public Material material;
        [Group("Decal")]
        public float baseLifeTime = 60;
        [Group("Decal")]
        public float emissionLifeTime = 10;
        [Group("Decal")]
        public float fadeRefreshSpeed = 0.1f;

        [Group("Decal")]
        public int priority = 0;
        [Group("Decal")]
        public bool allowRagdollPart = false;

        [Group("Size")]
        public Vector3 size = Vector3.one;
        [Group("Size")]
        public float sizeRandomRange = 0;
        [Group("Size")]
        public AnimationCurve sizeCurve;

        [Group("Color")]
        public EffectTarget linkBaseColor = EffectTarget.None;
        [Group("Color")]
        public EffectTarget linkEmissionColor = EffectTarget.None;

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

        public static List<EffectDecal> pool = new List<EffectDecal>();
        public static Transform poolRoot;
        public static int poolCount = 100;

        public override void Clean()
        {
        }

        public override void CopyHDRToNonHDR()
        {
        }

    }
}

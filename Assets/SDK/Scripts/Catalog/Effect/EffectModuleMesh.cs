using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;




using TriInspector;

namespace ThunderRoad
{
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Color", Title = "Color")]
    [DeclareBoxGroup("Color (HDR)", Title = "Color (HDR)")]
    [DeclareHorizontalGroup("Color (HDR)/MainColor")]
    [DeclareHorizontalGroup("Color (HDR)/SecondaryColor")]
    [DeclareBoxGroup("Color (Non HDR)", Title = "Color (Non HDR)")]
    [DeclareHorizontalGroup("Color (Non HDR)/MainNoHdr")]
    [DeclareHorizontalGroup("Color (Non HDR)/SecondaryNoHdr")]
    [DeclareBoxGroup("Mesh", Title = "Mesh")]
    public class EffectModuleMesh : EffectModule
    {
        [Group("General")]
        public AnimationCurve intensityCurve;
        [Group("General")]
        public float lifeTime = 5;
        [Group("General")]
        public float refreshSpeed = 0.1f;

        [Group("Color")]
        public EffectTarget linkBaseColor = EffectTarget.None;
        [Group("Color")]
        public EffectTarget linkTintColor = EffectTarget.None;
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

        [Group("Mesh")]
        public Vector3 localScale = Vector3.one;
        [Group("Mesh")]
        public bool useSizeCurve;
        [Group("Mesh")]
        [ShowIf("useSizeCurve")]
        public AnimationCurve sizeCurve;
        [Group("Mesh")]
        public float sizeFadeDuration;

        [Group("Mesh")]
        public Vector3 localPosition = Vector3.zero;
        [Group("Mesh")]
        public Vector3 localRotation = Vector3.zero;
        [Group("Mesh")]
        public bool useRotationYCurve;
        [Group("Mesh")]
        [ShowIf("useRotationYCurve")]
        public AnimationCurve rotationYCurve;
        [Group("Mesh")]
        public float rotationFadeDuration;

        [Group("Mesh")]
        public string meshAddress;
        [NonSerialized]
        public Mesh mesh;

        [Group("Mesh")]
        [ShowInInspector]
        [TableList(AlwaysExpanded = true)]
        public List<Materials> materials = new();

        [Group("Mesh")]
        [ShowInInspector]
        [TableList(AlwaysExpanded = true)]
        public List<MaterialProperty> materialProperties = new();


        public class Materials
        {
            public string materialAddress;

            public class Material : Materials 
            {
                [ShowInInspector]
                [ReadOnly]
                [NonSerialized]
                public UnityEngine.Material value; 
            }
        }

        public class MaterialProperty
        {
            public string name;
            [Serializable]
            public class Vector : MaterialProperty { public Vector4 value; }
            [Serializable]
            public class Float : MaterialProperty { public float value; }
            [Serializable]
            public class Int : MaterialProperty { public int value; }
            [Serializable]
            public class Color : MaterialProperty { public UnityEngine.Color value; }
        }


        public override void Clean()
        {
        }

        public override void CopyHDRToNonHDR()
        {
        }


    }
}

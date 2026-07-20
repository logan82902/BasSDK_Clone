using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;
using System.Collections;
using Newtonsoft.Json;



using TriInspector;

namespace ThunderRoad
{
    public enum SpawnTarget
    {
        Default,
        Source,
        Target
    }

    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Color", Title = "Color")]
    [DeclareBoxGroup("Color (HDR)", Title = "Color (HDR)")]
    [DeclareHorizontalGroup("Color (HDR)/MainColor")]
    [DeclareHorizontalGroup("Color (HDR)/SecondaryColor")]
    [DeclareBoxGroup("Color (Non HDR)", Title = "Color (Non HDR)")]
    [DeclareHorizontalGroup("Color (Non HDR)/MainNoHdr")]
    [DeclareHorizontalGroup("Color (Non HDR)/SecondaryNoHdr")]
    [DeclareBoxGroup("Scale", Title = "Scale")]
    [DeclareBoxGroup("PointCache", Title = "PointCache")]
    [DeclareBoxGroup("Mesh", Title = "Mesh")]
    [DeclareBoxGroup("Vfx", Title = "Vfx")]
    public class EffectModuleVfx : EffectModule
    {
        [Group("General")]
        public string vfxAddress;
        [NonSerialized]
        public VisualEffectAsset vfxAsset;
        [Group("General")]
        public bool despawnOnEnd = false;
        [Group("General")]
        public float despawnDelay = 0;
        [Group("General")]
        public AnimationCurve intensityCurve;
        [Group("General")]
        public float lifeTime = 5;
        [Group("General")]
        public bool useSecondaryRenderer;
        [Group("General")]
        public bool lookAtTarget;
        [Group("General")]
        public SpawnTarget spawnOn = SpawnTarget.Default;

        protected TriDropdownList<LayerName> LayerOverrideOptions()
        {
            TriDropdownList<LayerName> options = new();
            LayerName[] layerNames = (LayerName[])Enum.GetValues(typeof(LayerName));
            for (int i = 0; i < layerNames.Length; i++)
            {
                LayerName lName = layerNames[i];
                string text = lName == LayerName.None ? "Don't override" : $"Override: {lName.ToString()}";
                options.Add(text, lName);
            }
            return options;
        }

        [Group("General")]
        [Dropdown(nameof(LayerOverrideOptions))]
        public LayerName layerOverride = LayerName.None;

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

        [Group("Scale")]
        public Vector3 localScale = Vector3.one;
        [Group("Scale")]
        public bool useScaleCurve;
        [Group("Scale")]
        [ShowIf("useScaleCurve")]
        public AnimationCurve scaleCurve;

        [Group("PointCache")]
        public bool usePointCache = false;
        [Group("PointCache")]
        [ShowIf("usePointCache")]
        public bool pointCacheSkinnedMeshUpdate = false;
        [Group("PointCache")]
        [ShowIf("usePointCache")]
        public int pointCachePointCount = 4096;
        [Group("PointCache")]
        [ShowIf("usePointCache")]
        public int pointCacheSeed = 0;
        [Group("PointCache")]
        [ShowIf("usePointCache")]
        public PointCacheGenerator.Distribution pointCacheDistribution = PointCacheGenerator.Distribution.RandomUniformArea;
        [Group("PointCache")]
        [ShowIf("usePointCache")]
        public PointCacheGenerator.MeshBakeMode pointCacheBakeMode = PointCacheGenerator.MeshBakeMode.Triangle;

        [Group("Mesh")]
        public bool useMesh = false;
        [ShowIf("useMesh")]
        [Group("Mesh")]
        public string meshAddress;
        [NonSerialized]
        public Mesh meshAsset;

        [Group("Vfx")]
        [TableList(AlwaysExpanded = true)]
        public List<VfxProperty> materialProperties = new();


        [Serializable]
        public class VfxProperty
        {
            public string name;
            
            public class Vector2 : VfxProperty
            {
                public UnityEngine.Vector2 value;
            }

            public class Vector3 : VfxProperty
            {
                public UnityEngine.Vector3 value;
            }

            public class Float : VfxProperty
            {
                public float value;
            }

            public class Int : VfxProperty
            {
                public int value;
            }

            public class Gradient : VfxProperty
            {
                public UnityEngine.Gradient value;
            }

        }

        public override void Clean()
        {
        }

        public override void CopyHDRToNonHDR()
        {
        }

    }
}

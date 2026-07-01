using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;



using TriInspector;

namespace ThunderRoad
{
    [DeclareBoxGroup("Particle", Title = "Particle")]
    [DeclareBoxGroup("Color", Title = "Color")]
    [DeclareBoxGroup("Color (HDR)", Title = "Color (HDR)")]
    [DeclareHorizontalGroup("Color (HDR)/MainColor")]
    [DeclareHorizontalGroup("Color (HDR)/SecondaryColor")]
    [DeclareBoxGroup("Color (Non HDR)", Title = "Color (Non HDR)")]
    [DeclareHorizontalGroup("Color (Non HDR)/MainNoHdr")]
    [DeclareHorizontalGroup("Color (Non HDR)/SecondaryNoHdr")]
    [DeclareBoxGroup("Scale", Title = "Scale")]
    [DeclareBoxGroup("Rotation", Title = "Rotation")]
    [DeclareBoxGroup("Collision", Title = "Collision")]
    public class EffectModuleParticle : EffectModule
    {
        [Group("Particle")]
        public EffectLink effectLink = EffectLink.Intensity;
        [Group("Particle")]
        public float insideParticleRadius = 0f;
        [Group("Particle")]
        public float cullMinIntensity = 0.05f;
        [Group("Particle")]
        public float cullMinSpeed = 0.05f;
        [Group("Particle")]
        public string effectParticleAddress;
        [NonSerialized]
        public EffectParticle effectParticlePrefab;

        [Group("Particle")]
        public AnimationCurve intensityCurve;
        [Group("Particle")]
        public bool renderInLateUpdate;

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

        [Group("Rotation")]
        public Vector3 localRotation = Vector3.zero;

        [Group("Collision")]
        [Dropdown(nameof(GetAllEffectID))]
        public string collisionEffectId;
        [NonSerialized]
        public EffectData collisionEffectData;
        [Group("Collision")]
        [Tooltip("If this value is greater than zero, stop + despawn the effect after that duration.")]
        public float collisionEffectDuration = 0f;
        [Group("Collision")]
        public LayerMask collisionLayerMask = ~0;
        [Group("Collision")]
        public float collisionMaxGroundAngle = 45;
        [Group("Collision")]
        public float collisionEmitRate = 0.2f;
        [Group("Collision")]
        public float collisionMinIntensity = 0;
        [Group("Collision")]
        public float collisionMaxIntensity = 1;
        [Group("Collision")]
        public bool collisionUseMainGradient;
        [Group("Collision")]
        public bool collisionUseSecondaryGradient;

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);


        public override void CopyHDRToNonHDR()
        {
        }

        public override void Clean()
        {
        }

    }
}

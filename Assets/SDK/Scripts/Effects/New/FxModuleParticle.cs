using UnityEngine;


using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Effects/FxModuleParticle.html")]
    public class FxModuleParticle : FxModule
    {
        [Header("Emission Rate Over Time")]
        [LabelText("Link")]
        public Link emissionRateOverTimeLink = Link.None;
        [HideIf("emissionRateOverTimeLink", Link.None)]
        [LabelText("Curve")]
        public AnimationCurve emissionRateOverTimeCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
        [HideIf("emissionRateOverTimeLink", Link.None)]
        [LabelText("Range")]
        public Vector2 emissionRateOverTimeRange = new(0, 10);
        [HideIf("emissionRateOverTimeLink", Link.None)]
        [LabelText("Reduction")]
        public float emissionRateOverTimeReduction;


        [Header("Emission Rate Over Distance")]
        [LabelText("Link")]
        public Link emissionRateOverDistanceLink = Link.None;
        [HideIf("emissionRateOverDistanceLink", Link.None)]
        [LabelText("Curve")]
        public AnimationCurve emissionRateOverDistanceCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
        [HideIf("emissionRateOverDistanceLink", Link.None)]
        [LabelText("Range")]
        public Vector2 emissionRateOverDistanceRange = new(0, 10);
        [HideIf("emissionRateOverDistanceLink", Link.None)]
        [LabelText("Reduction")]
        public float emissionRateOverDistanceReduction;


        [Header("Life time")]
        [LabelText("Link")]
        public Link lifeTimeLink = Link.None;
        [HideIf("lifeTimeLink", Link.None)]
        [LabelText("Curve")]
        public AnimationCurve curveLifeTime;
        [HideIf("lifeTimeLink", Link.None)]
        [LabelText("Range")]
        public Vector2 lifeTimeRange = new(0, 10);
        [HideIf("lifeTimeLink", Link.None)]
        [LabelText("Random reduction")]
        public float lifeTimeReduction;


        [Header("Emission burst")]
        [LabelText("Link")]
        public Link emissionBurstLink = Link.None;
        [HideIf("emissionBurstLink", Link.None)]
        [LabelText("Curve")]
        public AnimationCurve emissionBurstCurve;
        [HideIf("emissionBurstLink", Link.None)]
        [LabelText("RandomRange")]
        public short emissionBurstRandomRange;


        [Header("Scale")]
        [LabelText("Link")]
        public Link scaleLink = Link.None;
        [HideIf("scaleLink", Link.None)]
        [LabelText("Curve")]
        public AnimationCurve scaleCurve;
        [HideIf("scaleLink", Link.None)]
        [LabelText("Weight")]
        public Vector3 scaleMultiplier = Vector3.one;


        [Header("Start speed")]
        [LabelText("Link")]
        public Link startSpeedLink = Link.None;
        [HideIf("startSpeedLink", Link.None)]
        [LabelText("Curve")]
        public AnimationCurve startSpeedLinkCurve;
        [HideIf("emissionBurstLink", Link.None)]
        [LabelText("Min speed ratio")]
        [Range(0, 1)]
        public float startSpeedMinSpeedRatio = 1;

        protected MaterialPropertyBlock materialPropertyBlock;
#pragma warning disable 0109 // unity this.particleSystem is obsolete, we can override this
        protected new ParticleSystem particleSystem;
#pragma warning restore 0109        
        protected ParticleSystemRenderer particleSystemRenderer;
        protected ParticleSystem.MainModule particleSystemMain;
        protected ParticleSystem.MinMaxCurve minMaxCurve = new ParticleSystem.MinMaxCurve();

    }
}

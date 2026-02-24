using UnityEngine;
using System;


using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    public class FxBlendCurves
    {
        public bool useIntensityCurve;
        [ShowIf("useIntensityCurve")]
        public AnimationCurve intensityCurve;

        public bool useSpeedCurve;
        [ShowIf("useSpeedCurve")]
        public AnimationCurve speedCurve;
        [ShowIf("useIntensityCurve")]
        [ShowIf("useSpeedCurve")]
        public BlendMode blendMode = BlendMode.Min;

        public FxBlendCurves()
        {
            this.intensityCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
            this.speedCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        }

        public FxBlendCurves(AnimationCurve curve)
        {
            this.intensityCurve = curve;
            this.speedCurve = curve;
        }

        public bool IsUsed()
        {
            return (useIntensityCurve || useSpeedCurve);
        }

        public bool TryGetValue(float intensity, float speed, out float value)
        {
            if (useIntensityCurve && useSpeedCurve)
            {
                switch (blendMode)
                {
                    case BlendMode.Min:
                        value = Mathf.Min(intensityCurve.Evaluate(intensity), speedCurve.Evaluate(speed));
                        return true;
                    case BlendMode.Max:
                        value = Mathf.Max(intensityCurve.Evaluate(intensity), speedCurve.Evaluate(speed));
                        return true;
                    case BlendMode.Average:
                        value = Mathf.Lerp(intensityCurve.Evaluate(intensity), speedCurve.Evaluate(speed), 0.5f);
                        return true;
                    case BlendMode.Multiply:
                        value = intensityCurve.Evaluate(intensity) * speedCurve.Evaluate(speed);
                        return true;
                }
            }
            else if (useIntensityCurve)
            {
                value = intensityCurve.Evaluate(intensity);
                return true;
            }
            else if (useSpeedCurve)
            {
                value = speedCurve.Evaluate(speed);
                return true;
            }
            value = 0;
            return false;
        }
    }
}

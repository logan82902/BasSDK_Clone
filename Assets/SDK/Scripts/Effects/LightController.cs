using UnityEngine;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/LightController")]
    public class LightController : MonoBehaviour
    {
        protected float currentIntensity;
        protected float targetIntensity;
        
        [Header("Intensity to intensity (0 = disabled)")]
        public float minIntensity;
        public float maxIntensity;
        public float randomRangeIntensity;

        [Header("Intensity to range (0 = disabled)")]
        public float minRange;
        public float maxRange;
        public float randomRange;

        [Header("Lerp time (0 = disabled)")]
        public float lerpTime = 0;

        public Light light;

        public void Awake()
        {
            light ??= GetComponent<Light>();
        }

        public virtual void SetActive(bool active)
        {
            light.enabled = active;
        }

        public virtual void SetIntensityInstant(float value)
        {
            targetIntensity = value;
            UpdateIntensity(true);
        }

        public virtual void SetIntensity(float value)
        {
            targetIntensity = value;
            if (lerpTime == 0)
            {
                UpdateIntensity(true);
            }
        }

        public void LerpOn(float speed)
        {
            lerpTime = speed;
            SetIntensity(1);
        }

        public void LerpOff(float speed)
        {
            lerpTime = speed;
            SetIntensity(0);
        }

        public void UpdateIntensity(bool instant = false)
        {
            if (Mathf.Approximately(currentIntensity, targetIntensity)) return;

            currentIntensity = instant ? targetIntensity : Mathf.Lerp(currentIntensity, targetIntensity, Time.unscaledDeltaTime * lerpTime);
            if (maxIntensity > 0)
            {
                light.intensity = Mathf.Lerp(minIntensity, maxIntensity, currentIntensity);
            }
            if (maxRange > 0)
            {
                light.range = Mathf.Lerp(minRange, maxRange, currentIntensity);
            }
        }

        public virtual void SetColor(Color mainColor)
        {
            light.color = mainColor;
        }

        public void Update()
        {
            UpdateIntensity();
        }
    }
}

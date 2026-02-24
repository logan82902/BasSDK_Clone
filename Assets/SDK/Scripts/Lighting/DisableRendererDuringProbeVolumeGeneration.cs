using UnityEngine;

namespace ThunderRoad
{
    [ExecuteInEditMode]
    public class DisableRendererDuringProbeVolumeGeneration : MonoBehaviour
    {
#if UNITY_EDITOR
        public new Renderer renderer;

        private void OnValidate()
        {
            if (renderer == null)
            {
                renderer = this.gameObject.GetComponent<Renderer>();
            }
            if (renderer == null)
            {
                Debug.LogErrorFormat(this, "No renderer assigned on the component [DisableRendererDuringProbeVolumeGeneration]");
            }
        }

        private void OnEnable()
        {
            // warning suppressed due to it being unsubscribed in OnDisable()/ManagedOnDisable()
#pragma warning disable UDR0005 // Domain Reload Analyzer
            LightProbeVolumeGenerator.OnGenerationStarted += OnVolumeGenerationStarted;
            LightProbeVolumeGenerator.OnGenerationEnded += OnVolumeGenerationEnded;
#pragma warning restore UDR0005 // Domain Reload Analyzer
        }

        private void OnDisable()
        {
            LightProbeVolumeGenerator.OnGenerationStarted -= OnVolumeGenerationStarted;
            LightProbeVolumeGenerator.OnGenerationEnded -= OnVolumeGenerationEnded;
        }

        private void OnVolumeGenerationStarted()
        {
            if (renderer != null)
            {
                Debug.Log("Disabled renderer " + renderer.name);
                renderer.enabled = false;
            }
        }

        private void OnVolumeGenerationEnded()
        {
            if (renderer != null)
            {
                Debug.Log("Enabled renderer " + renderer.name);
                renderer.enabled = true;
            }
        }
#endif
    }
}

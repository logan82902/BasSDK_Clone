using UnityEngine;
using UnityEngine.Serialization;

namespace ThunderRoad
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(PlayerTeleporter))]
    public class OutOfBoundsFader : ThunderBehaviour
    {
        public Collider boundsCollider;
        public bool constrainPlayer = true;
        public bool constrainSpectator = true;
        public AnimationCurve teleportTimerByBoundsDistance = AnimationCurve.Linear(0f, 8f, 3f, 3f);
        public AnimationCurve fadingByTimeLeft = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        [Tooltip("Audio plays whenever the fade strength goes above the fade intensity audio curve at the same time. Audio won't play again until the fade strength drops below this curve.")]
        [FormerlySerializedAs("fadeIntensityVoiceCurve")]
        public AnimationCurve fadeIntensityAudioCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 0f);
        public AudioEffect fadeAudioType = AudioEffect.Voice;
        [FormerlySerializedAs("voiceOnFade")]
        public string fadeAudio = string.Empty;
        public bool useAltUnderwater = false;
        public AudioEffect underwaterFadeAudioType = AudioEffect.Voice;
        public string underwaterFadeAudio = string.Empty;

        [HideInInspector]
        public PlayerTeleporter playerTeleporter;

        protected float fadeStrength = 0f;
        protected bool hasPlayedFadeVoice = false;
        protected float playerTimeLeftNormalized = 1f;
        protected float playerTeleportTime = -10f;

        public enum AudioEffect
        {
            Voice,
            Effect,
        }

        private void OnValidate()
        {
            if (boundsCollider == null) boundsCollider = GetComponent<Collider>();
            if (boundsCollider != null) boundsCollider.isTrigger = true;
            if (playerTeleporter == null) playerTeleporter = GetComponent<PlayerTeleporter>();
            if (playerTeleporter != null)
            {
                playerTeleporter.useFading = false;
                playerTeleporter.useRelativePosition = false;
            }
        }

    }
}

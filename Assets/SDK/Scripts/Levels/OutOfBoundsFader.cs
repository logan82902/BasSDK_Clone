using UnityEngine;

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
        public AnimationCurve fadeIntensityVoiceCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 0f);
        public string voiceOnFade = string.Empty;

        [HideInInspector]
        public PlayerTeleporter playerTeleporter;

        protected float fadeStrength = 0f;
        protected bool hasPlayedFadeVoice = false;
        protected float playerTimeLeftNormalized = 1f;
        protected float playerTeleportTime = -10f;

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

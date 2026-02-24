using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;


using TriInspector;

namespace ThunderRoad
{
    public class GolemAnimatorEvent : MonoBehaviour
    {
        [Header("References")]
        public GolemController golem;
        public Animator animator;
        public List<AudioSource> audioSources = null;
        public List<ParticleSystem> particleSystems = null;

        [Header("Feet")]
        [ReadOnly]
        public bool rightFootPlanted;
        [ReadOnly]
        public bool leftFootPlanted;

        [Header("Events")]
        public UnityEvent onLeftFootPlant;
        public UnityEvent onRightFootPlant;

        public Action<bool> onEnableHitbox;

        public static int blendIdHash, rightFootHash;

        protected Dictionary<string, AudioSource> keyedAudios = new Dictionary<string, AudioSource>();
        protected Dictionary<string, ParticleSystem> keyedParticles = new Dictionary<string, ParticleSystem>();


        private void OnValidate()
        {
            if (!animator) animator = this.GetComponent<Animator>();
            if (!golem) golem = this.GetComponentInParent<GolemController>();
            if (audioSources.IsNullOrEmpty())
            {
                audioSources = new List<AudioSource>();
                audioSources.AddRange(golem.GetComponentsInChildren<AudioSource>());
            }

            if (particleSystems.IsNullOrEmpty())
            {
                particleSystems = new List<ParticleSystem>();
                particleSystems.AddRange(golem.GetComponentsInChildren<ParticleSystem>());
            }
        }

        private void Awake()
        {
            InitAnimationParametersHashes();
            for (int i = 0; i < audioSources.Count; i++)
            {
                if (keyedAudios.ContainsKey(audioSources[i].name)) continue;
                if (audioSources[i].GetComponentInParent<GolemCrystal>() != null) continue;
                keyedAudios.Add(audioSources[i].name, audioSources[i]);
            }
            for (int i = 0; i < particleSystems.Count; i++)
            {
                if (keyedAudios.ContainsKey(particleSystems[i].name)) continue;
                if (particleSystems[i].GetComponentInParent<GolemCrystal>() != null) continue;
                keyedParticles[particleSystems[i].name] = particleSystems[i];
            }
        }

        private void InitAnimationParametersHashes()
        {
            blendIdHash = Animator.StringToHash("BlendID");
            rightFootHash = Animator.StringToHash("RightFoot");
        }

        void OnAnimatorMove()
        {
        }
        
        public void RightPlant(UnityEngine.AnimationEvent e)
        {
            float blendAnimId = animator.GetFloat(blendIdHash);
            float thisParameter = e?.floatParameter ?? 0f;
            float forwardRound = Mathf.Round(blendAnimId);
            // Whether the sate is the same as the current one according to the animator
			bool correctState = e == null ? true : e.animatorStateInfo.fullPathHash == animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
			if (correctState && Mathf.Abs(thisParameter - forwardRound) <= Mathf.Epsilon)
            {
                animator.SetBool(rightFootHash, false);
                rightFootPlanted = true;
                onRightFootPlant?.Invoke();
            }
        }

        public void RightUnplant(UnityEngine.AnimationEvent e)
        {
            float blendAnimId = animator.GetFloat(blendIdHash);
            float thisParameter = e?.floatParameter ?? 0f;
            float forwardRound = Mathf.Round(blendAnimId);

            if (Mathf.Abs(thisParameter - forwardRound) <= Mathf.Epsilon)
            {
                rightFootPlanted = false;
            }
        }

        public void LeftPlant(UnityEngine.AnimationEvent e)
        {
            float blendAnimId = animator.GetFloat(blendIdHash);
            float thisParameter = e?.floatParameter ?? 0f;
            float forwardRound = Mathf.Round(blendAnimId);
			bool correctState = e == null ? true : e.animatorStateInfo.fullPathHash == animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
			if (correctState && Mathf.Abs(thisParameter - forwardRound) <= Mathf.Epsilon)
            {
                animator.SetBool(rightFootHash, true);
                leftFootPlanted = true;
                onLeftFootPlant?.Invoke();
            }
        }

        public void LeftUnplant(UnityEngine.AnimationEvent e)
        {
            float blendAnimId = animator.GetFloat(blendIdHash);
            float thisParameter = e?.floatParameter ?? 0f;
            float forwardRound = Mathf.Round(blendAnimId);

            if (Mathf.Abs(thisParameter - forwardRound) <= Mathf.Epsilon)
            {
                leftFootPlanted = false;
            }
        }

        void ActivateAbilityStep(UnityEngine.AnimationEvent e)
        {
        }

        void StartTurnTo(UnityEngine.AnimationEvent e)
        {
        }

        void StopTurnTo(UnityEngine.AnimationEvent e)
        {
        }

        void EnableHitbox(UnityEngine.AnimationEvent e)
        {
            onEnableHitbox?.Invoke(true);
        }

        void DisableHitbox(UnityEngine.AnimationEvent e)
        {
            onEnableHitbox?.Invoke(false);
        }

        void EffectOn(UnityEngine.AnimationEvent e)
        {
        }

        void EffectOff(UnityEngine.AnimationEvent e)
        {
        }

        void PlayAudioSource(UnityEngine.AnimationEvent e)
        {
            if (keyedAudios.TryGetValue(e.stringParameter, out AudioSource audioSource)) audioSource.Play();
        }

        void PlayParticleEffect(UnityEngine.AnimationEvent e)
        {
            if (!keyedParticles.TryGetValue(e.stringParameter, out ParticleSystem system))
            {
                Debug.LogWarning($"Golem attempted to {(e.intParameter == 0 ? "stop" : "play")} particle"
                                 + $" effect {e.stringParameter}, but no particle system of that name could be found.");
                return;
            }
            switch (e.intParameter)
            {
                case 0:
                    system.Stop();
                    break;
                case 1:
                    system.Play();
                    break;
            }
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Events;

namespace ThunderRoad
{
    public class AudioContainerPlayer : MonoBehaviour
    {
        public AudioContainer audioContainer;
        public bool playOnAwake;
        public bool loop = false;

        [NonSerialized]
        public AudioSource audioSource;

        public AudioMixerName audioMixer = AudioMixerName.Effect;

        public bool isPlaying
        {
            get { return audioSource.isPlaying; }
        }
        public void Play()
        {
        }

        public void Stop()
        {
        }
    }
}
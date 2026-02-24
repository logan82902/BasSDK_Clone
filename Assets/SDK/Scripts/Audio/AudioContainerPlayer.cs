using System;
using UnityEngine;


using TriInspector;

namespace ThunderRoad
{
    public class AudioContainerPlayer : MonoBehaviour
    {
        public AudioContainer audioContainer;
        public bool playOnAwake;

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

    }
}
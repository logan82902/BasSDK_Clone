using System;
using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;


using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    [DeclareBoxGroup("Music", Title = "Music")]
    public class MusicSegment
    {
        [Group("Music")]
        public float volumeDb = 0.0f;

        [Group("Music")]
        public string musicAddress = "";

        public int timesPlayedInSerie = 1;

        [NonSerialized]
        public IResourceLocation musicLocation;

        [NonSerialized]
        public AudioClip audioClip;

    }
}
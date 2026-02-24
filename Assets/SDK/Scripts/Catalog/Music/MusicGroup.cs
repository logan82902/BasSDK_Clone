using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;


using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    [DeclareBoxGroup("Container", Title = "Container")]
    [DeclareBoxGroup("Music", Title = "Music")]
    public class MusicGroup : CatalogData
    {
        [Group("Container")]
        [HideReferencePicker]
        public List<MusicSegment> musicSegments = null;

        [Group("Container")]
        public bool shuffleMode = true;

        [Group("Container")]
        public int restartSequenceIndex = 0;

        [Group("Music")]
        public float volumeDb = 0.0f;

        [Group("Music")]
        public int BPM = 0;

        [Group("Music")]
        public int beatsPerBar = 0;

        [Group("Music")]
        public int barPerGrid = 0;

        private List<MusicSegment> _musicSegmentsOrder = null;
    }
}
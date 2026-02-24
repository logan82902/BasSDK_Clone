using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;


using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BrainModuleFear : BrainData.Module
    {
        public bool receiving = true;
        public string panicScreamDialogID = "falling";
        public float cowerDuration = 10f;
        public string cowerAnimationID = "HumanCower";

        [NonSerialized]
        [JsonIgnore]
        public AnimationData cowerAnimation;
        [JsonIgnore]
        public bool isCowering => Time.time <= cowerEndTime;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public float cowerEndTime = -1f;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public float endPanicBrainActiveTime = -1f;

    }
}

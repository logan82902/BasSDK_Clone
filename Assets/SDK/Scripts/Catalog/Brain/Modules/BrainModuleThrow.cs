using UnityEngine;
using System;
using System.Collections.Generic;
using ThunderRoad.AI.Action;
using System.Linq;
using System.Collections;
using ThunderRoad.Skill.SpellPower;
using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BrainModuleThrow : BrainModuleRanged
    {
        [Header("Rock throwing")]
        public bool rockThrowEnabled = true;
        public Vector3 rockSpawnPositioning = new Vector3(0f, 0f, 0.1f);
        public float rockGrabTime = 0.5f;
        public Vector2 rockGrabLookIKAdjustDurations = new Vector2(0.5f, 0.25f);

    }
}
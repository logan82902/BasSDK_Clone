using UnityEngine;
using System;
using System.Collections;
using ThunderRoad.AI.Action;
using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BrainModuleCrossbow : BrainModuleRanged
    {
        public float maxShootAngle = 1;
        public string pullWireAnimation;

    }
}
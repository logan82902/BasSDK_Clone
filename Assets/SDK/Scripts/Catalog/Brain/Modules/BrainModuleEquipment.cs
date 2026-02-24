using System;
using System.Collections;
using UnityEngine;


using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BrainModuleEquipment : BrainData.Module
    {
        public const float drawDelay = 0.8f;

        public bool allowArmGrabDisarm = true;
        public int grabDisarmPushLevel = 2;

        public bool allowArmStabDisarm = true;

        public int handHitDisarmPushLevel = 2;

        [MinMaxSlider(0, 10, true)]
        public Vector2 dropObjectsDelay = new(0, 0.5f);

        [NonSerialized]
        public bool isDrawing;
        [NonSerialized]
        public bool isHolstering;
        [NonSerialized]
        public Item rightBeingDrawn;
        [NonSerialized]
        public Item leftBeingDrawn;

        public static int hashDrawLeft, hashDrawRight;
        public static bool hashInitialized;

    }
}
using UnityEngine;
using System.Collections.Generic;
using System;
using Shadowood.RaycastTexture;
using Unity.Mathematics;

using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Levels/Water.html")]
    [AddComponentMenu("ThunderRoad/Levels/Water")]
    public class Water : MonoBehaviour
    {
        public RaycastTexture raycastTexture;
        
        public GameObject underWaterEffect;

        [Tooltip("When enabled, the Water will only be enabled when you enter the room (Dungeons Only)")]
        public bool showWhenInRoomOnly = true;

        [NonSerialized]
        public bool waterHeightCanChangeovertime = false;
        
        public RaycastTexture.eMode waterType => raycastTexture?.mode ?? RaycastTexture.eMode.Ocean;

        [Button(ButtonSizes.Medium, "Set Recommended Values")]
        private void SetOceanRecommendedValues()
        {
            if(waterType == RaycastTexture.eMode.Ocean)
            {
                waveHeight = 4f;
                waveLength = 9f;
                period = 8f;
            }
            else
            {
                waveHeight = 100f;
                waveLength = 20f;
                period = 50f;
            }
        }
        
        [Tooltip("Wave height in meters")]
        public float waveHeight = 4f;
        [Tooltip("Length between wave peaks in world space")]
        public float waveLength = 9f;
        [Tooltip("How long it takes for a wave to complete a full cycle in seconds")]
        public float period = 8f;
        
        public enum Quality
        {
            Low,
            High,
        }

    }
}
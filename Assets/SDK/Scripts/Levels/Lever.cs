using System;
using UnityEngine;
using UnityEngine.Events;


using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Items/Lever.html")]
    public class Lever : HingeDrive
    {
        [Header("Lever values")]
        [Range(0f, 1f)] public float deadZone = 0.8f;
        public bool invertOutput = true;

        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public State state = State.InBetween;

        [Header("Lever Related Events")] public UnityEvent leverUpEvent = new UnityEvent();
        public UnityEvent leverDownEvent = new UnityEvent();
        public UnityEvent<float> leverAnalogEvent = new UnityEvent<float>();

        public enum State
        {
            Up,
            Down,
            InBetween,
        };

    }
}
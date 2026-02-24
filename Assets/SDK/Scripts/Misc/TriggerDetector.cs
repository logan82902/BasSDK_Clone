using System;
using UnityEngine;
using UnityEngine.Events;

namespace ThunderRoad
{
    public class TriggerDetector : MonoBehaviour
    {
        public UnityEvent<Collider> OnTriggerEnterEvent = new();
        public UnityEvent<Collider> OnTriggerExitEvent = new();
        public UnityEvent<Collider> OnTriggerStayEvent = new();

        private void OnTriggerEnter(Collider other) => OnTriggerEnterEvent?.Invoke(other);
        private void OnTriggerExit(Collider other) => OnTriggerExitEvent?.Invoke(other);
        private void OnTriggerStay(Collider other) => OnTriggerStayEvent?.Invoke(other);
    }

}

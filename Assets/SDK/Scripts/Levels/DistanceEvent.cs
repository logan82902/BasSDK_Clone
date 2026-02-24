using UnityEngine;
using UnityEngine.Events;



namespace ThunderRoad
{
    public class DistanceEvent : MonoBehaviour
    {
        public bool usePlayerHeadAsReference;
        public Transform reference;
        public float radius = 2;
        public float checkInterval = 1;

        public UnityEvent onRadiusEnter;
        public UnityEvent onRadiusExit;

        public bool inRadius;

        public enum DisableBehavior
        {
            None,
            OutputLastCurveValue,
            OutputFirstCurveValue,
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(this.transform.position, radius);
        }
    }
}
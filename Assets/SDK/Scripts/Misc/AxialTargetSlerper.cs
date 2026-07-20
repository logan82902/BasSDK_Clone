using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    public class AxialTargetSlerper : ThunderBehaviour
    {
        public Transform target;
        public bool smooth = true;
        [ShowIf(nameof(smooth))]
        public float smoothingTime = 0.1f;
        [ShowIf(nameof(smooth))]
        public float maxTurnSpeed = 10000f;
        [HideIf(nameof(smooth))]
        public float slerpSpeed = 10f;
        public bool lookRotationPreventTwist = true;
        public Vector3 localForwardVector = Vector3.forward;
        public Vector3 localUpVector = Vector3.up;
        public Vector3 worldUpVector = Vector3.up;

        public bool SlerpActive { get; set; }

        private float smoothVelocity;

        public override ManagedLoops EnabledManagedLoops => ManagedLoops.Update;

        protected internal override void ManagedUpdate()
        {
            if (!SlerpActive) return;
            Quaternion frameStep = smooth ? Utils.QuaternionSmoothSlerp(transform.rotation, target.rotation, ref smoothVelocity, smoothingTime, maxTurnSpeed, Time.deltaTime) : Utils.QuaternionSlerpTowards(transform.rotation, target.rotation, slerpSpeed * Time.deltaTime);
            transform.rotation = frameStep;
            if (lookRotationPreventTwist)
            {
                Quaternion lookRotationQuaternion = Quaternion.LookRotation(transform.TransformDirection(localForwardVector), Vector3.up);
                transform.rotation = lookRotationQuaternion * Quaternion.LookRotation(Vector3.forward, Vector3.up).To(Quaternion.LookRotation(localForwardVector, localUpVector));
            }
        }
    }
}

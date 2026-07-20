using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    public class CollisionIgnore : MonoBehaviour
    {
        public bool ignoreCollisionOnAwake = true;
        public bool collectCollidersByOverlap = false;
        public float overlapCapsuleHeight;
        public float overlapCapsuleRadius;
        public Vector3 overlapCapsulePosition = Vector3.zero;
        public Vector3 overlapCapsuleRotation = Vector3.zero;
        public LayerMask overlapCapsuleLayerMask = ~0;
        public List<Collider> colliders = new List<Collider>();

        private Vector3 overlapCapsuleWorldPosition => transform.TransformPoint(overlapCapsulePosition);
        private Quaternion overlapCapsuleWorldRotation => transform.TransformRotation(Quaternion.Euler(overlapCapsuleRotation));

        private void OnValidate()
        {
            if (!gameObject.activeInHierarchy) return;
            if (colliders.Count == 0)
            {
                GetChildColliders();
            }
        }

        protected void Awake()
        {
            if (collectCollidersByOverlap)
            {
                float halfHeightMinRadius = (overlapCapsuleHeight / 2f) - overlapCapsuleRadius;
                Vector3 capsuleDirection = overlapCapsuleWorldRotation * Vector3.forward;
                Vector3 pointA = overlapCapsuleWorldPosition + (Mathf.Clamp(halfHeightMinRadius, 0f, Mathf.Infinity) * capsuleDirection.normalized);
                Vector3 pointB = overlapCapsuleWorldPosition - (Mathf.Clamp(halfHeightMinRadius, 0f, Mathf.Infinity) * capsuleDirection.normalized);
                colliders.AddRange(Physics.OverlapCapsule(pointA, pointB, overlapCapsuleRadius, overlapCapsuleLayerMask));
            }
            for (int i = colliders.Count - 1; i >= 0; i--)
            {
                if (colliders[i] == null) colliders.RemoveAt(i);
            }
            if (ignoreCollisionOnAwake)
            {
                Set(true);
            }
        }

        public void Set(bool ignoreCollision)
        {
            foreach (Collider collider1 in colliders)
            {
                if (collider1 == null) continue;
                foreach (Collider collider2 in colliders)
                {
                    if (collider2 == null) continue;
                    if (collider1 != collider2)
                    {
                        Physics.IgnoreCollision(collider1, collider2, ignoreCollision);
                    }
                }
            }
        }

        [Button]
        public void GetChildColliders()
        {
            colliders = new List<Collider>(this.gameObject.GetComponentsInChildren<Collider>(true));
        }

        private void OnDrawGizmos()
        {
            if (!collectCollidersByOverlap) return;
            Common.DrawGizmoCapsule(overlapCapsuleWorldPosition, overlapCapsuleWorldRotation, overlapCapsuleHeight, overlapCapsuleRadius);
        }
    }
}

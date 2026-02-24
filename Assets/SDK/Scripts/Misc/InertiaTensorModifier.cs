using UnityEngine;


using TriInspector;

namespace ThunderRoad
{
    public class InertiaTensorModifier : MonoBehaviour
    {
        public Collider referenceCollider;
        public Vector3 customInertiaTensor;
        public Quaternion customInertiaTensorRotation;
        public bool applyCustomInertiaTensorOnStart = true;

    }
}

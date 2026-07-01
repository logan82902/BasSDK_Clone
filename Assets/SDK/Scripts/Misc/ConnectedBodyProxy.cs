using System;
using TriInspector;
using UnityEngine;
using UnityEngine.Animations;

namespace ThunderRoad
{
    public class ConnectedBodyProxy : ThunderBehaviour
    {
        [InfoBox("The GameObject this ConnectedBodyProxy is on has no joint on it!", TriMessageType.Error, nameof(missingJoint))]
        [InfoBox("The joint on this GameObject has no connected body! ConnectedBodyProxy doesn't work unless its joint has a connected body.", TriMessageType.Error, nameof(jointMissingBody))]
        [InfoBox("The connected body of the joint has no constraints and isn't kinematic. Are you sure you need a connected body proxy?", TriMessageType.Warning, nameof(proxyBodyUnnecessary))]
        public bool startWithProxyActive = true;
        [ReadOnly]
        public Joint joint;
        private bool missingJoint
        {
            get
            {
                if (joint == null) joint = GetComponent<Joint>();
                return joint == null;
            }
        }
        private bool jointMissingBody
        {
            get
            {
                if (missingJoint) return false;
                return (joint ?? GetComponent<Joint>()).connectedBody == null;
            }
        }
        private bool proxyBodyUnnecessary
        {
            get
            {
                if (jointMissingBody) return false;
                Rigidbody connectedBody = (joint ?? GetComponent<Joint>()).connectedBody;
                if (connectedBody.isKinematic) return false;
                if (connectedBody.constraints != RigidbodyConstraints.None) return false;
                return true;
            }
        }

        private bool proxyActive = false;
        private Rigidbody orgConnectedBody;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        [ShowInPlayMode]
        public Rigidbody proxyBody;

        private void OnValidate()
        {
            SetJoint();
            if (joint == null) Debug.LogError("ConnectedBodyProxy requires a joint on the same GameObject to function!");
        }

        private void Awake()
        {
            SetJoint();
        }

        private void SetJoint()
        {
            if (joint == null) SetJoint(GetComponent<Joint>());
        }

        public void SetJoint(Joint joint)
        {
            this.joint = joint;
        }

        void Start()
        {
            SetProxyConnection(startWithProxyActive);
        }

        public void SetProxyConnectionOn() => SetProxyConnection(true);

        public void SetProxyConnectionOff() => SetProxyConnection(false);

        private void SetProxyConnection(bool active)
        {
            if (joint == null)
            {
                Debug.LogError($"Trying to {(active ? "activate" : "deactivate")} ConnectedBodyProxy on a GameObject with no joint on it!");
                return;
            }
            if (active == proxyActive) return;
            if (active)
            {
                // no proxy body means first time setup never ran
                if (proxyBody == null)
                {
                    orgConnectedBody = joint.connectedBody;
                    if (orgConnectedBody == null)
                    {
                        Debug.LogError($"Joint on {name} is missing a connected body, aborting connected body proxy setup.");
                        return;
                    }

                    // create the proxy body; the proxy body is a kinematic rigidbody with position and rotation constraints to constantly update its position and rotation to match the original connected body
                    proxyBody = new GameObject($"ConnectedBodyProxy_{name}").AddComponent<Rigidbody>();
                    proxyBody.isKinematic = true;
                    proxyBody.useGravity = false;
                    proxyBody.mass = orgConnectedBody.mass;
                    proxyBody.transform.position = orgConnectedBody.transform.position;
                    proxyBody.transform.rotation = orgConnectedBody.transform.rotation;
                    proxyBody.transform.parent = orgConnectedBody.transform.parent;
                    PositionConstraint positionConstraint = proxyBody.gameObject.AddComponent<PositionConstraint>();
                    positionConstraint.AddSource(new ConstraintSource() { sourceTransform = joint.connectedBody.transform, weight = 1f });
                    positionConstraint.locked = true;
                    positionConstraint.constraintActive = true;
                    RotationConstraint rotationConstraint = proxyBody.gameObject.AddComponent<RotationConstraint>();
                    rotationConstraint.AddSource(new ConstraintSource() { sourceTransform = joint.connectedBody.transform, weight = 1f });
                    rotationConstraint.locked = true;
                    rotationConstraint.constraintActive = true;

                    // we want to lock the anchor position in place so we don't need to worry about it auto-updating. force auto-configure to false and make sure it didn't auto-update
                    Vector3 connectedAnchor = joint.connectedAnchor;
                    joint.autoConfigureConnectedAnchor = false;
                    joint.connectedAnchor = connectedAnchor;
                }
                joint.connectedBody = proxyBody;
            }
            else
            {
                if (orgConnectedBody == null)
                {
                    Debug.LogError("Original connected body has gone missing! Can't set proxy connection back off!");
                    return;
                }
                joint.connectedBody = orgConnectedBody;
            }
            proxyActive = active;
            foreach (Collider ownCollider in joint.GetComponentsInChildren<Collider>())
            {
                foreach (Collider otherCollider in orgConnectedBody.GetComponentsInChildren<Collider>())
                {
                    // if the joint is configured to enable collision, keep collision on (ignore: false), but otherwise, set the collision state depending on the active state of the proxy
                    // when the joint is connected to the proxy body, it won't properly prevent collision between the joint's rigidbody and the original connected body, so we have to do it manually
                    Physics.IgnoreCollision(ownCollider, otherCollider, !joint.enableCollision && active);
                }
            }
        }

        private void OnDestroy()
        {
            if (joint != null) SetProxyConnection(false);
            Destroy(proxyBody.gameObject);
        }
    }
}

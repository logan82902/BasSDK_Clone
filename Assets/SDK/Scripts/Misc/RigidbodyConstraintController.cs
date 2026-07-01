using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    public class RigidbodyConstraintController : ThunderBehaviour
    {
        public Rigidbody rb;

        public void SetRB(Rigidbody rb) => this.rb = rb;

        public void SetFreezePosition(bool active) => rb.constraints = active ? rb.constraints | RigidbodyConstraints.FreezePosition : rb.constraints & ~RigidbodyConstraints.FreezePosition;

        public void SetFreezePositionX(bool active) => rb.constraints = active ? rb.constraints | RigidbodyConstraints.FreezePositionX : rb.constraints & ~RigidbodyConstraints.FreezePositionX;

        public void SetFreezePositionY(bool active) => rb.constraints = active ? rb.constraints | RigidbodyConstraints.FreezePositionY : rb.constraints & ~RigidbodyConstraints.FreezePositionY;

        public void SetFreezePositionZ(bool active) => rb.constraints = active ? rb.constraints | RigidbodyConstraints.FreezePositionZ : rb.constraints & ~RigidbodyConstraints.FreezePositionZ;

        public void SetFreezeRotation(bool active) => rb.constraints = active ? rb.constraints | RigidbodyConstraints.FreezeRotation : rb.constraints & ~RigidbodyConstraints.FreezeRotation;

        public void SetFreezeRotationX(bool active) => rb.constraints = active ? rb.constraints | RigidbodyConstraints.FreezeRotationX : rb.constraints & ~RigidbodyConstraints.FreezeRotationX;

        public void SetFreezeRotationY(bool active) => rb.constraints = active ? rb.constraints | RigidbodyConstraints.FreezeRotationY : rb.constraints & ~RigidbodyConstraints.FreezeRotationY;

        public void SetFreezeRotationZ(bool active) => rb.constraints = active ? rb.constraints | RigidbodyConstraints.FreezeRotationZ : rb.constraints & ~RigidbodyConstraints.FreezeRotationZ;

        public void SetFreezeAll(bool active) => rb.constraints = active ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
    }
}

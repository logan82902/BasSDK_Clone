using UnityEngine;

namespace ThunderRoad
{
    public class ConfigJointMotionConstaintController : ThunderBehaviour
    {
        public ConfigurableJoint joint;

        public void SetJoint(ConfigurableJoint joint) => this.joint = joint;

        public void SetPositionalMotionLocked()
        {
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
        }
        public void SetPositionalMotionLimited()
        {
            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;
        }
        public void SetPositionalMotionFree()
        {
            joint.xMotion = ConfigurableJointMotion.Free;
            joint.yMotion = ConfigurableJointMotion.Free;
            joint.zMotion = ConfigurableJointMotion.Free;
        }

        public void SetXMotionLocked() => joint.xMotion = ConfigurableJointMotion.Locked;
        public void SetXMotionLimited() => joint.xMotion = ConfigurableJointMotion.Limited;
        public void SetXMotionFree() => joint.xMotion = ConfigurableJointMotion.Free;

        public void SetYMotionLocked() => joint.yMotion = ConfigurableJointMotion.Locked;
        public void SetYMotionLimited() => joint.yMotion = ConfigurableJointMotion.Limited;
        public void SetYMotionFree() => joint.yMotion = ConfigurableJointMotion.Free;

        public void SetZMotionLocked() => joint.zMotion = ConfigurableJointMotion.Locked;
        public void SetZMotionLimited() => joint.zMotion = ConfigurableJointMotion.Limited;
        public void SetZMotionFree() => joint.zMotion = ConfigurableJointMotion.Free;

        public void SetAngularMotionLocked()
        {
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;
        }
        public void SetAngularMotionLimited()
        {
            joint.angularXMotion = ConfigurableJointMotion.Limited;
            joint.angularYMotion = ConfigurableJointMotion.Limited;
            joint.angularZMotion = ConfigurableJointMotion.Limited;
        }
        public void SetAngularMotionFree()
        {
            joint.angularXMotion = ConfigurableJointMotion.Free;
            joint.angularYMotion = ConfigurableJointMotion.Free;
            joint.angularZMotion = ConfigurableJointMotion.Free;
        }

        public void SetAngularXMotionLocked() => joint.angularXMotion = ConfigurableJointMotion.Locked;
        public void SetAngularXMotionLimited() => joint.angularXMotion = ConfigurableJointMotion.Limited;
        public void SetAngularXMotionFree() => joint.angularXMotion = ConfigurableJointMotion.Free;

        public void SetAngularYMotionLocked() => joint.angularYMotion = ConfigurableJointMotion.Locked;
        public void SetAngularYMotionLimited() => joint.angularYMotion = ConfigurableJointMotion.Limited;
        public void SetAngularYMotionFree() => joint.angularYMotion = ConfigurableJointMotion.Free;

        public void SetAngularZMotionLocked() => joint.angularZMotion = ConfigurableJointMotion.Locked;
        public void SetAngularZMotionLimited() => joint.angularZMotion = ConfigurableJointMotion.Limited;
        public void SetAngularZMotionFree() => joint.angularZMotion = ConfigurableJointMotion.Free;
    }
}

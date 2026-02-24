using System.Collections.Generic;
using UnityEngine;


using TriInspector;

namespace ThunderRoad
{
    [DeclareBoxGroup("Stance basics", Title = "Stance basics")]
    [DeclareBoxGroup("Shooting", Title = "Shooting")]
    public class FirearmStanceData : StanceData
    {
        [Group("Stance basics")]
        [OnValueChanged(nameof(UpdateIDs))]
        public string rootName;
        [Group("Shooting")]
        [HideLabel]
        [HideReferencePicker]
        [Header("Idle")]
        public FirearmPose idlePose = new();
        [Group("Shooting")]
        [HideLabel]
        [HideReferencePicker]
        [Header("Aim")]
        public FirearmPose aimPose = new();
        [Group("Shooting")]
        [HideLabel]
        [HideReferencePicker]
        [Header("Fire")]
        public FirearmAction fireAction = new();
        [Group("Shooting")]
        [HideLabel]
        [HideReferencePicker]
        [Header("Reload")]
        public FirearmAction reloadAction = new();

        protected void UpdateIDs()
        {
            idlePose.id = rootName + "IdlePose";
            aimPose.id = rootName + "AimPose";
            fireAction.id = rootName + "ShootAnimation";
            reloadAction.id = rootName + "ReloadAnimation";
        }

        public override IEnumerable<StanceNode> AllStanceNodes()
        {
            yield return idlePose;
            yield return aimPose;
            yield return fireAction;
            yield return reloadAction;
        }

    }
}

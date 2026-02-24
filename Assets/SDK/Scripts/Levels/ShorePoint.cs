using System;
using System.Collections.Generic;
using UnityEngine;

using TriInspector;

namespace ThunderRoad
{
    public class ShorePoint : ThunderBehaviour
    {
        public static List<ShorePoint> levelShorePoints;

        [ReadOnly, ShowInInspector]
        public Vector3 groundPoint;
        [SerializeField, HideInInspector]
        private Vector3 lastUpdatePosition;


        private void OnValidate()
        {
            UpdateGroundpoint();
        }

        private void Start()
        {
            if (levelShorePoints == null) levelShorePoints = new List<ShorePoint>();
            levelShorePoints.Add(this);
            UpdateGroundpoint();
        }

        private void OnDestroy()
        {
            if (levelShorePoints != null) levelShorePoints.Remove(this);
        }

        public void UpdateGroundpoint()
        {
            if (transform.position == lastUpdatePosition) return;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f, Common.MakeLayerMask(LayerName.Default, LayerName.LocomotionOnly), QueryTriggerInteraction.Ignore))
            {
                groundPoint = hit.point;
                lastUpdatePosition = transform.position;
            }
            else
            {
                Debug.LogError($"ShorePoint {name} could not find ground point; make sure its center is above the ground.");
            }
        }
    }
}

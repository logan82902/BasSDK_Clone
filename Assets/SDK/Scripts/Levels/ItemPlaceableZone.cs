using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using TriInspector;

namespace ThunderRoad
{
    [RequireComponent(typeof(Collider))]
    public class ItemPlaceableZone : ThunderBehaviour
    {
        public string containerID;
        public bool allowItemsFromItemSpawners = true;
        public List<ItemSpawner> ignoredItemSpawners = new List<ItemSpawner>();

        [ShowInInspector]
        [ReadOnly]
        public ContentStatus contentStatus { get; protected set; } = ContentStatus.Prespawn;

        public List<Item> items => itemsInZone.Keys.ToList();
        [ShowInInspector]
        [ReadOnly]
        public Container container { get; protected set; }

        protected Dictionary<string, Container> placeableContainers = new Dictionary<string, Container>();
        protected Collider triggerCollider;
        protected Dictionary<Item, List<Collider>> itemsInZone = new Dictionary<Item, List<Collider>>();

        public enum ContentStatus
        {
            Prespawn,
            ContentReady,
            WaitingForContent,
            Spawned,
        }

    }

    public class ContentCustomDataRelativePosRot : ContentCustomData
    {
        public Vector3 localPos;
        public Quaternion localRot;

        public ContentCustomDataRelativePosRot() { }

        public ContentCustomDataRelativePosRot(Transform item, Transform relative)
        {
            localPos = relative.InverseTransformPoint(item.position);
            localRot = relative.InverseTransformRotation(item.rotation);
        }
    }
}


using TriInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThunderRoad
{
    public class PersistentItemAdder : MonoBehaviour
    {
        [Header("Item")]
        [HideLabel]
        [InlineProperty]
        public ItemContent item = new();
        [InlineButton(nameof(PlacementToTransform), "Auto-set")]
        [Header("Placement")]
        [HideLabel]
        [InlineProperty]
        public ContentStatePlaced placement = new();
        [Header("Container")]
        public Container persistenceContainer;
        public bool addOnStart = true;

        private void PlacementToTransform() =>
            placement = new ContentStatePlaced() { position = transform.position, rotation = transform.rotation };


        public void AddItem()
        {
        }
    }
}

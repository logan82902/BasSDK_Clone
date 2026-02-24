using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class PickupItem : ActionNode
    {
        public string inputItemHandleVariableName = "";
        public float pickupIKDuration = 0.75f;
        public float pickupDuration = 1.05f;
        public bool useIK = false;

        [Dropdown(nameof(GetAllAnimationID))]
        [Header("Animation")]
        public string pickupAnimationId;
        [NonSerialized]
        public AnimationData pickupAnimationData;

        public TriDropdownList<string> GetAllAnimationID() => Catalog.GetDropdownAllID(Category.Animation);

    }
}


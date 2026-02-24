using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class PlayAnimation : ActionNode
    {
        [Header("Animation")]
        public bool useAddressableAddress = false;
        [Dropdown(nameof(GetAllAnimationID))]
        [HideIf("useAddressableAddress")]
        public string animationDataID;
        [HideIf("useAddressableAddress")]
        public bool overrideSpeed;
        [ShowIf("useAddressableAddress")]
        public string animationAddress;
        [ShowIf(nameof(showAnimationSpeed))]
        public float animationSpeed = 1f;
        private bool showAnimationSpeed => useAddressableAddress || overrideSpeed;
        public PlayType playType = PlayType.OneShot;
        public bool avoidLastPick = false;
        public bool upperOnly = false;
        public Vector2 animDurationMinMax = new Vector2(1f, 3f);
        public bool exitAutomatically = true;
        public bool interruptStop = true;
        public bool mirror = false;
        [Header("Tree")]
        public bool doCallbackNode;
        [EnableIf(nameof(doCallbackNode))]
        public Node callbackNode;

        public enum PlayType
        {
            OneShot,
            Loop,
            ThreeStep,
            LimitedOneShot,
            LimitedThreeStep,
        }

        public TriDropdownList<string> GetAllAnimationID() => Catalog.GetDropdownAllID(Category.Animation);

    }
}

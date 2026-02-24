using Newtonsoft.Json;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class AttackThrow : AttackRanged
    {
        public SidePref preferredThrowHand = SidePref.PreferRight;

        public enum SidePref
        {
            PreferRight,
            PreferLeft,
            OnlyRight,
            OnlyLeft
        }

        public bool grabRock = false;

    }
}

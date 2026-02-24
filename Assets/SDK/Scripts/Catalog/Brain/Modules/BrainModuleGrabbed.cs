using System.Collections;
using ThunderRoad.Skill;
using ThunderRoad.Skill.SpellPower;
using UnityEngine;

using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BrainModuleGrabbed : BrainData.Module
    {
        public bool freeOneHand = true;
        public float freeOneHandDelay = 2;
        public bool freeBothHand = true;
        public float freeBothHandDelay = 5;
        public float grabThrowMinVelocity = 2.0f;
    }
}
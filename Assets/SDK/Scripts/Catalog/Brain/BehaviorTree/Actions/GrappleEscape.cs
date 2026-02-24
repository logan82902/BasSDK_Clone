using UnityEngine;
using System.Collections;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GrappleEscape : ActionNode
    {
        public float initialSuccessChance = 0.25f;
        public float attemptsMultiplier = 0.1f;
        public Vector2 timeBetweenAttempts = new Vector2(1f, 2f);
        public float twoGrabPenalty = 0.15f;
        public float maximumSuccessChance = 0.5f;
        public bool resetAttemptsOnDamage = true;
    }
}

using Newtonsoft.Json;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [DeclareBoxGroup("Overrides", Title = "Overrides")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class AttackBow : AttackRanged
    {
        [Group("Overrides")]
        public bool overrideMaxShootAngle = false;
        [Group("Overrides")]
        [EnableIf(nameof(overrideMaxShootAngle))]
        public float maxShootAngleOverride = 0f;

    }
}

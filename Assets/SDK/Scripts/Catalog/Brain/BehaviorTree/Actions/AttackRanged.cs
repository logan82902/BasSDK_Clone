using System;
using Newtonsoft.Json;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [DeclareBoxGroup("Inputs", Title = "Inputs")]
    [DeclareBoxGroup("Overrides", Title = "Overrides")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public abstract class AttackRanged : ActionNode
    {
        [Group("Inputs")]
        public string inputTransformVariableName = "";
        [Group("Inputs")]
        public bool useCurrentTargetIfNullTransform = true;
        [Group("Overrides")]
        public bool overrideSpread = false;
        [Group("Overrides")]
        [EnableIf(nameof(overrideSpread))]
        public Vector2 spreadOverride = Vector2.zero;
        [Group("Overrides")]
        public bool overrideWeaponRotationX = false;
        [Group("Overrides")]
        public bool overrideWeaponRotationY = false;
        [Group("Overrides")]
        public bool overrideWeaponRotationZ = false;
        [Group("Overrides")]
        public Vector3 weaponRotationOverride = Vector3.zero;

    }
}

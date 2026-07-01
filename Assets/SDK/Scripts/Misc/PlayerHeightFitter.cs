using UnityEngine;
using System;
using TriInspector;

namespace ThunderRoad
{
    [DeclareHorizontalGroup("LerpValues")]
    [DeclareHorizontalGroup("Clamps")]
    [DeclareVerticalGroup("Verti")]
    [DeclareHorizontalGroup("Verti/LiveValues")]
    public class PlayerHeightFitter : ThunderBehaviour
    {
        [MinMaxSlider(0f, 3f, ShowFields = true)]
        [HideLabel]
        [Title("Min/Max Height")]
        public Vector2 minMaxHeight = new Vector2(1.5f, 1.9f);
        [Group("LerpValues")]
        [InlineProperty]
        [HideLabel]
        [Title("Local Position")]
        public MinMaxVector3 minMaxLocalPosition = new MinMaxVector3(Vector3.zero, Vector3.zero);
        [Group("LerpValues")]
        [InlineProperty]
        [HideLabel]
        [Title("Local Scale")]
        public MinMaxVector3 minMaxLocalScale = new MinMaxVector3(Vector3.one, Vector3.one);
        [Title("Options")]
        [Group("Verti")]
        [AutoLabelWidth(AddedWidth = 25f)]
        public float defaultLerpValue = 0.5f;
        [ShowInInspector]
        [Group("Verti/LiveValues")]
        [ReadOnly]
        public float currentLerpValue { get; private set; } = 0f;
        [ShowInInspector]
        [Group("Verti/LiveValues")]
        [ReadOnly]
        public float targetLerpValue { get; private set; } = 0f;
        [Group("Clamps")]
        public float lowerClamp = 0f;
        [Group("Clamps")]
        public float upperClamp = 1f;
        public float smoothingTime = 0.2f;

        private float lerpValueVelocity = 0f;

        [Serializable]
        [DeclareBoxGroup("Min", HideTitle = true)]
        [DeclareBoxGroup("Max", HideTitle = true)]
        public class MinMaxVector3
        {
            [Group("Min")]
            [AutoLabelWidth]
            public Vector3 min;
            [Group("Max")]
            [AutoLabelWidth]
            public Vector3 max;

            public MinMaxVector3(Vector3 min, Vector3 max)
            {
                this.min = min;
                this.max = max;
            }

            // always uses unclamped because we manually clamp the t value going in
            public Vector3 GetValue(float t) => Vector3.LerpUnclamped(new Vector3(min.x, min.y, min.z), new Vector3(max.x, max.y, max.z), t);
        }

        [Button]
        public void SetValue(float t)
        {
        }

        [Button]
        public void SetTargetValue(float t)
        {
        }

        [Button]
        public void Activate()
        {
        }

        [Button]
        public void Deactivate()
        {
        }

    }
}

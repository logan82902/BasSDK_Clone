using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad.Skill.Spell;
using UnityEngine;
using Object = UnityEngine.Object;
using TriInspector;

namespace ThunderRoad.Skill.SpellMerge
{
    [Serializable]
    [DeclareBoxGroup("Throw", Title = "Throw")]
    [DeclareBoxGroup("Lift", Title = "Lift")]
    [DeclareBoxGroup("Bubble", Title = "Bubble")]
    public class SpellMergeGravity : SpellMergeData
    {

        [Group("Throw")]
        [ShowIf("allowThrow")]
        public float throwDefaultForce = 22;
        [Group("Throw")]
        [ShowIf("allowThrow")]
        public float throwRagdollLimbForce = 6;

        [Group("Lift")]
        public float mergingLiftRadius = 5f;
        [Group("Lift")]
        public float liftMinForce = 0.3f;
        [Group("Lift")]
        public float liftMaxForce = 0.6f;
        [Group("Lift")]
        public float liftRagdollForceMultiplier = 2f;
        [Group("Lift")]
        public float liftDrag = 1f;
        [Group("Lift")]
        public Vector3 randomTorqueRange = new(0.2f, 0.2f, 0.2f);
        [Group("Lift")]
        public float playerGravityRatio = 0.5f;

        [Group("Bubble")]
        [Dropdown(nameof(GetAllEffectID))]
        public string bubbleEffectId;
        protected EffectData bubbleEffectData;
        [Group("Bubble")]
        public float bubbleDuration = 10;
        [Group("Bubble")]
        public float bubbleEffectMaxScale = 15;
        [Group("Bubble")]
        public AnimationCurve bubbleScaleCurveOverTime;
        [Group("Bubble")]
        public float bubbleEndNormalizedTime = 0.85f;
        [Group("Bubble")]
        public AnimationCurve bubbleEndCurve;
        [Group("Bubble")]
        public float bubbleHandSeparationMaxAngle = 45f;

        [Group("Bubble")]
        public bool enableHandControl;

        [Group("Bubble")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusId = "BubbleGravity";

        [Group("Bubble")]
        [ShowIf("enableHandControl")]
        public float handControlForce = 0.5f;
        [Group("Bubble")]
        [ShowIf("enableHandControl")]
        public ForceMode handControlForceMode = ForceMode.VelocityChange;

        protected static Vector3[] randomTorqueArray;
        protected static float[] randomHeightArray;

        protected bool capturedPlayer;

        [NonSerialized]
        public static bool bubbleActive;
        [NonSerialized]
        public static bool closing;

        [NonSerialized]
        public Vector3 bubblePosition;


        public delegate void BubbleEvent(Mana mana, Vector3 position, Zone zone);

        public event BubbleEvent OnBubbleOpen;
        public event BubbleEvent OnBubbleClose;

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            if (!string.IsNullOrEmpty(throwEffectId))
                throwEffectData = Catalog.GetData<EffectData>(throwEffectId);
            if (!string.IsNullOrEmpty(bubbleEffectId))
                bubbleEffectData = Catalog.GetData<EffectData>(bubbleEffectId);
            if (randomTorqueArray == null)
            {
                randomTorqueArray = new Vector3[5];
                randomTorqueArray[0] = new Vector3(UnityEngine.Random.Range(-randomTorqueRange.x, randomTorqueRange.x), UnityEngine.Random.Range(-randomTorqueRange.y, randomTorqueRange.y), UnityEngine.Random.Range(-randomTorqueRange.z, randomTorqueRange.z));
                randomTorqueArray[1] = new Vector3(UnityEngine.Random.Range(-randomTorqueRange.x, randomTorqueRange.x), UnityEngine.Random.Range(-randomTorqueRange.y, randomTorqueRange.y), UnityEngine.Random.Range(-randomTorqueRange.z, randomTorqueRange.z));
                randomTorqueArray[2] = new Vector3(UnityEngine.Random.Range(-randomTorqueRange.x, randomTorqueRange.x), UnityEngine.Random.Range(-randomTorqueRange.y, randomTorqueRange.y), UnityEngine.Random.Range(-randomTorqueRange.z, randomTorqueRange.z));
                randomTorqueArray[3] = new Vector3(UnityEngine.Random.Range(-randomTorqueRange.x, randomTorqueRange.x), UnityEngine.Random.Range(-randomTorqueRange.y, randomTorqueRange.y), UnityEngine.Random.Range(-randomTorqueRange.z, randomTorqueRange.z));
                randomTorqueArray[4] = new Vector3(UnityEngine.Random.Range(-randomTorqueRange.x, randomTorqueRange.x), UnityEngine.Random.Range(-randomTorqueRange.y, randomTorqueRange.y), UnityEngine.Random.Range(-randomTorqueRange.z, randomTorqueRange.z));
            }
            if (randomHeightArray == null)
            {
                randomHeightArray = new float[5];
                randomHeightArray[0] = 0f;
                randomHeightArray[1] = 0.03f;
                randomHeightArray[2] = -0.04f;
                randomHeightArray[3] = 0.02f;
                randomHeightArray[4] = -0.05f;
            }
        }

    }
}

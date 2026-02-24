using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Event-Linkers/BowEventLinker.html")]
    [AddComponentMenu("ThunderRoad/Bows/Bow Event Linker")]
    public class BowEventLinker : EventLinker
    {
        // These are explicitly assigned int values so that even if the order of the list gets changed, the assignments in prefabs and scenes will remain the same
        public enum BowEvent
        {
            OnArrowAdd = 0,
            OnArrowRemoved = 1,
            OnArrowFired = 2,
            OnStringGrab = 3,
            OnStringUngrab = 4,
            OnStringReleaseStart = 5,
            OnStringReleaseEnd = 6,
            OnLinkerStart = 7,
        }

        public enum ArrowState
        {
            Either,
            Arrow,
            NoArrow
        }

        [System.Serializable]
        [DeclareHorizontalGroup("Options", Sizes = new float[] { 220f, 0f })]
        public class BowUnityEvent
        {
            [AutoLabelWidth(AddedWidth = 5f)]
            public BowEvent bowEvent;
            [Group("Options")]
            [AutoLabelWidth(AddedWidth = 5f)]
            public ArrowState arrowStateRequired;
            [ShowInInspector]
            [Group("Options")]
            [AutoLabelWidth(AddedWidth = 5f)]
            [MinMaxSlider(0f, 1f, ShowFields = true)]
            public Vector2 pullRatioRange
            {
                get
                {
                    Vector2 value = new Vector2(pullPercent - allowance, pullPercent + allowance);
                    Vector2 clamped = value.ClampMinMax(0f, 1f);
                    if (!clamped.x.IsApproximately(value.x) || !clamped.y.IsApproximately(value.y))
                    {
                        allowance = Mathf.Abs(clamped.y - clamped.x) / 2f;
                        pullPercent = clamped.x + allowance;
                        return clamped;
                    }
                    return value;
                }
                set
                {
                    allowance = Mathf.Abs(value.y - value.x) / 2f;
                    pullPercent = value.x + allowance;
                }
            }
            //[Range(0f, 1f)]
            [HideInInspector]
            public float pullPercent = 0.5f;
            //[Range(0f, 1.1f)]
            [HideInInspector]
            public float allowance = 1f;
            public UnityEvent<BowString> onActivate;
        }

        public BowString bowString;
        public List<BowUnityEvent> bowEvents = new List<BowUnityEvent>();
        protected Dictionary<BowEvent, List<BowUnityEvent>> eventsDictionary;

        private void OnValidate()
        {
            bowString ??= GetComponent<BowString>();
        }


        public override void UnsubscribeNamedMethods()
        {
            // No named methods to unsubscribe
        }

    }
}

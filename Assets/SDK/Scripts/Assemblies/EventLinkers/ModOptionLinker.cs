using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

using TriInspector;

namespace ThunderRoad
{
    public abstract class ModOptionLinker<T, V> : EventLinker where V : UpdateChecker<T>
    {

        [System.Serializable]
        public class ValueInvoker
        {
            public List<UnityEvent<T>> events;
        }

        //[Header("Mod option")]
        public string optionDataID;
        [Space]
        public bool ignoreFirstUpdate = true;
        public List<UnityEvent<T>> onValueUpdateEvents;
        public List<ValueInvoker> valueInvokers;
        public List<V> specificValueCheckEvents;

        [NonSerialized]
        public ModOptionData<T> optionData;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public T lastValue = default;

        public void Start()
        {
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

        }

        private void LinkedValueChange(object obj)
        {
            if (!listening) return;
            ValueUpdate(obj, true);
        }

        private void ValueUpdate(object obj, bool fireUpdates)
        {
            if (obj is T input)
            {
                lastValue = input;
                if (!fireUpdates) return;
                if (ignoreFirstUpdate)
                {
                    ignoreFirstUpdate = false;
                    return;
                }
                ExecuteInOrder(onValueUpdateEvents, lastValue, null);
            }
            else
            {
                Debug.LogError("Mod option type is different some mod option linker type");
            }
        }

        public void RunAllValueCheckers()
        {
            for (int i = 0; i < specificValueCheckEvents.Count; i++)
            {
                var valueChecker = specificValueCheckEvents[i];
                if (valueChecker.Check(lastValue))
                {
                    ExecuteInOrder(valueChecker.valueMatchEvents, lastValue);
                }
            }
        }

        public void CheckOptionValue(int index)
        {
            if (specificValueCheckEvents.CountCheck(count => index >= count))
            {
                return;
            }
            var valueChecker = specificValueCheckEvents[index];
            if (valueChecker.Check(lastValue))
            {
                ExecuteInOrder(valueChecker.valueMatchEvents, lastValue);
            }
        }

        public void RunValueInvokeEvents(int index)
        {
            if (valueInvokers.CountCheck(count => index >= count))
            {
                return;
            }
            var valueInvoker = valueInvokers[index];
            ExecuteInOrder(valueInvoker.events, lastValue);
        }
    }

    [Serializable]
    [DeclareHorizontalGroup("Condition")]
    public abstract class UpdateChecker<T>
    {
        public enum Comparison
        {
            Equals,
            NotEquals,
            Lesser,
            LesserOrEqual,
            GreaterOrEqual,
            Greater
        }

        [Group("Condition")]
        [Dropdown(nameof(AllowedComparisons), Searchable = false)]
        [AutoLabelWidth(AddedWidth = 5f)]
        [LabelText("Option Value")]
        public Comparison comparison;
        [Group("Condition")]
        [HideLabel]
        public T comparedValue;
        public List<UnityEvent<T>> valueMatchEvents;

        private float lastPrintTime = 0f;

        public virtual TriDropdownList<Comparison> AllowedComparisons()
        {
            TriDropdownList<Comparison> result = new TriDropdownList<Comparison>();
            foreach (Comparison comparison in Enum.GetValues(typeof(Comparison)))
            {
                result.Add(Utils.AddSpacesToSentence(comparison.ToString(), true), comparison);
            }
            return result;
        }

        public abstract bool Check(T value);

        protected void PrintMessage(string message, bool error)
        {
            if (Time.time - lastPrintTime < 1f) return;
            if (error) Debug.LogError(message);
            else Debug.LogWarning(message);
            lastPrintTime = Time.time;
        }
    }
}

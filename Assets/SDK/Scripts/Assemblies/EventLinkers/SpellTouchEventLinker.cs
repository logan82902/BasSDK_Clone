using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Event-Linkers/SpellTouchEventLinker.html")]
    [AddComponentMenu("ThunderRoad/Spell Touch Event Linker")]
    [RequireComponent(typeof(Collider))]
    public class SpellTouchEventLinker : MonoBehaviour
    {
        public enum Step
        {
            Enter,
            Exit
        }
        [Serializable]
        public class SpellUnityEvent
        {
            [AutoLabelWidth(AddedWidth = 5f)]
            public string spellId;
            [NonSerialized]
            public int spellHashId;
            [AutoLabelWidth(AddedWidth = 5f)]
            public Step step;
            [AutoLabelWidth(AddedWidth = 5f)]
            public float minCharge = 0;
            public UnityEvent onActivate;
        }
        public List<SpellUnityEvent> spellEvents = new List<SpellUnityEvent>();

    }
}

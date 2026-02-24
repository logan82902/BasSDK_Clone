using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TriInspector;

// event linkers are already set up to unsubscribe from named events + static events when they're destroyed
#pragma warning disable UDR0005 // Domain Reload Analyzer
namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Event-Linkers/DamagerEventLinker.html")]
	[AddComponentMenu("ThunderRoad/Damager Event Linker")]
    public class DamagerEventLinker : EventLinker
    {
        // These are explicitly assigned int values so that even if the order of the list gets changed, the assignments in prefabs and scenes will remain the same
        public enum DamagerEvent
        {
            OnDamageDealt = 0,
            OnKillDealt = 1,
            OnLinkerStart = 2,
            OnPierceEnter = 3,
            OnUnpierce = 4,
            OnPierceThrough = 5,
        }

        [System.Serializable]
        public class DamagerUnityEvent
        {
            [AutoLabelWidth(AddedWidth = 5f)]
            public DamagerEvent damagerEvent;
            [ShowIf(nameof(showPierceTarget))]
            [AutoLabelWidth(AddedWidth = 5f)]
            public PierceTarget pierceTarget = PierceTarget.Anything;
            private bool showPierceTarget => damagerEvent == DamagerEvent.OnPierceEnter || damagerEvent == DamagerEvent.OnUnpierce || damagerEvent == DamagerEvent.OnPierceThrough;
            public UnityEvent<Damager> onActivate;
        }

        [System.Flags]
        public enum PierceTarget
        {
            Static = 1,
            Item = 2,
            Ragdoll = 4,
            Anything = 7,
        }

        public Damager damager;
        public List<DamagerUnityEvent> damagerEvents = new List<DamagerUnityEvent>();
        public CollisionHandler collisionHandler { get; protected set; }
        protected Dictionary<DamagerEvent, List<DamagerUnityEvent>> eventsDictionary;

        private void OnValidate()
        {
            damager ??= GetComponent<Damager>();
        }


        public override void UnsubscribeNamedMethods()
        {
            // No named methods to unsubscribe
        }

    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TriInspector;

// event linkers are already set up to unsubscribe from named events + static events when they're destroyed
#pragma warning disable UDR0005 // Domain Reload Analyzer
namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Event-Linkers/CollisionEventLinker.html")]
	[AddComponentMenu("ThunderRoad/Collision Event Linker")]
    public class CollisionEventLinker : EventLinker
    {
        // These are explicitly assigned int values so that even if the order of the list gets changed, the assignments in prefabs and scenes will remain the same
        public enum CollisionEvent
        {
            OnCollisionEnter = 0,
            OnHarmlessCollision = 1,
            OnCollisionExit = 2,
            OnDamageDealt = 3,
            OnKillDealt = 4,
            OnLinkerStart = 5,
        }

        [System.Serializable]
        public class CollisionUnityEvent
        {
            [AutoLabelWidth(AddedWidth = 5f)]
            public CollisionEvent collisionEvent;
            public UnityEvent<ColliderGroup> onActivate;
        }

        public ColliderGroup colliderGroup;
        public List<CollisionUnityEvent> collisionEvents = new List<CollisionUnityEvent>();
        public CollisionHandler collisionHandler { get; protected set; }
        protected Dictionary<CollisionEvent, List<CollisionUnityEvent>> eventsDictionary;

        private void OnValidate()
        {
            colliderGroup ??= GetComponent<ColliderGroup>();
        }


        public override void UnsubscribeNamedMethods()
        {
            // No named methods to unsubscribe
        }

    }
}

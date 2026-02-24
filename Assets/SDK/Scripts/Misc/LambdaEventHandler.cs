using System;
using System.Collections.Generic;

using TriInspector;

namespace ThunderRoad
{
	public abstract class LambdaEventHandler : ThunderBehaviour
    {
        protected List<Action> anonymousHandlerUnsubscribers;

        [Button]
        public virtual void CheckConfiguredEvents()
        {
            UnsubscribeAllAnonymous();
            if (anonymousHandlerUnsubscribers == null) anonymousHandlerUnsubscribers = new List<Action>();
        }

        protected void UnsubscribeAllAnonymous()
        {
            if (anonymousHandlerUnsubscribers.IsNullOrEmpty()) return;
            foreach (Action anonymousHandlerUnsubscriber in anonymousHandlerUnsubscribers)
            {
                anonymousHandlerUnsubscriber.Invoke();
            }
            anonymousHandlerUnsubscribers.Clear();
        }

        protected virtual void OnDestroy()
        {
            UnsubscribeAllAnonymous();
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

namespace ThunderRoad
{
    public class AnimatorEventReceiver : ThunderBehaviour
    {
        public UnityEvent<float> onFloatEvent = new();
        public UnityEvent<int> onIntEvent = new();
        public UnityEvent<string> onStringEvent = new();
        public UnityEvent<Object> onObjectEvent = new();
        public void FireFloat(float parameter) => onFloatEvent?.Invoke(parameter);
        public void FireInt(int parameter) => onIntEvent?.Invoke(parameter);
        public void FireString(string parameter) => onStringEvent?.Invoke(parameter);
        public void FireObject(Object parameter) => onObjectEvent?.Invoke(parameter);
    }
}
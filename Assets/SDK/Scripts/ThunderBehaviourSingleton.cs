namespace ThunderRoad
{
using UnityEngine;

    public class ThunderBehaviourSingleton<T> : ThunderBehaviour where T : ThunderBehaviour
    {
        #region Fields
        // RuntimeInitializeOnLoadMethod is not allowed on a generic class, so this variable is reset in the non-generic derived classes
#pragma warning disable UDR0001
        protected static T _instance = null;
#pragma warning restore UDR0001
        #endregion Fields

        #region Properties
        public static T Instance 
        {
            get
            {
#if UNITY_EDITOR
                // Find instance in scene in editor
                if (_instance == null && Application.isPlaying == false)
                {
                    _instance = FindObjectOfType<T>();
                }
                if (_instance == null)
                {
                    Debug.LogError($"Unable to find instance of singleton {typeof(T).Name} in scene.");
                }
#endif //UNITY_EDITOR

                return _instance;
            }
        }
        public static bool HasInstance => _instance != null;

        #endregion Properties

        #region Methods
        void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("Two singleton of type " + GetType().Name + " are present. New one will replace the other");
            }

            _instance = this as T;
            OnSetInstance();
        }

        protected virtual void OnSetInstance() { }

        private void OnDestroy()
        {
            OnInstanceDestroyed?.Invoke();
            _instance = null;
        }
        #endregion Methods

        #region Events
        public delegate void InstanceDestroyed();
        public event InstanceDestroyed OnInstanceDestroyed;
        #endregion Events
    }
}
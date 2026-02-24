using Newtonsoft.Json;
using System;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    [AutoLabelWidth(MinWidth = 0f)]
    public abstract class MusicDynamicModule
    {
        [JsonMergeKey, JsonProperty("$type"), JsonIgnore]
        public Type type
        {
            get { return GetType(); }
            set { }
        }

        public bool isDefault = true;

        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        protected bool _isConnected = false;

        public bool IsConnected => _isConnected;
        public virtual void Connect()
        {
            _isConnected = true;
        }
        public virtual void Disconnect()
        {
            _isConnected = false;
        }
    }
}

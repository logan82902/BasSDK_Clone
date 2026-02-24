using System.Collections.Generic;
using Newtonsoft.Json;
using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    public abstract class IdlePose : StanceNode
    {
        [ShowInInspector]
        [ReadOnly]
        [JsonIgnore]
        [ShowIf("isPlaying")]
        public List<IdlePose> nextIdles;

    }
}

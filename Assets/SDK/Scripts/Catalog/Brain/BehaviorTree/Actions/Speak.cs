using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class Speak : ActionNode
    {
        public enum PlayMode
        {
            Default,
            WithCooldown,
            OnlyOnce,
        }
        public string dialogId;
        public bool abnormalNoise = false;
        public bool checkMuffled = true;
        public PlayMode playMode = PlayMode.Default;
        public bool ignoreIfFromWave = true;
        [ShowIf("playMode", PlayMode.WithCooldown)]
        public Vector2 cooldownMinMax = new(3f, 5f);
        [ShowIf("playMode", PlayMode.WithCooldown)]
        public bool succeedOnCooldown = true;

    }
}

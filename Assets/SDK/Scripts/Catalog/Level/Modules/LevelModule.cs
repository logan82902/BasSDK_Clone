using System;
using System.Collections;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    [AutoLabelWidth(MinWidth = 0f)]
    public abstract class LevelModule : Module
    {
        [NonSerialized]
        public Level level;

        public virtual IEnumerator OnPlayerSpawnCoroutine()
        {
            yield break;
        }
    }
}

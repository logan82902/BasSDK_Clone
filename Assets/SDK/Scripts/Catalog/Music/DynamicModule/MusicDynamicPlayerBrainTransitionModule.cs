using System;
using System.Collections.Generic;

namespace ThunderRoad
{
    public class BrainMusicMapping : UnitySerializedDictionary<BrainModulePlayer.Exposure, string> { }


    [Serializable]
    public class MusicDynamicPlayerBrainTransitionModule : MusicDynamicModule
    {
        public BrainMusicMapping exposureMusicGroupId = new BrainMusicMapping();

    }
}

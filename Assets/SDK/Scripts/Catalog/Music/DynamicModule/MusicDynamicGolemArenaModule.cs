using System;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad
{
    public class GolemMusicMapping : UnitySerializedDictionary<Golem.State, string> { }


    [Serializable]
    public class MusicDynamicGolemArenaModule : MusicDynamicStingerModule
    {
        [Dropdown(nameof(GetAllEffectID))]
        public string headshotStingerId;
        [NonSerialized]
        public EffectData headshotStinger;
        
        public List<Golem.State> stingerOnGolemStateChange;
        public GolemMusicMapping golemStateActiveMusicId = new GolemMusicMapping();

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);


        public string[] dynamicModuleToDisconnectOnGolemActivation;
        public string[] dynamicModuleToConnectOnGolemActivation;

    }
}

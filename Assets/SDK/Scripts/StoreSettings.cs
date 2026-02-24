using System;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    public class StoreSettings
    {
        [Group("Mods")]
        public bool allowJsonMods = true;
        [Group("Mods")]
        public bool allowScriptedMods = true;
        [Group("Mods")]
        public bool allowModManager = true;
        [Group("Mods")]
        public bool authenticateViaEmail = false;

    }
}

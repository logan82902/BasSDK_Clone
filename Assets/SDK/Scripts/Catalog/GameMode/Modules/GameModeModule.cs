using System;
using TriInspector;

namespace ThunderRoad.Modules
{
    [AutoLabelWidth(MinWidth = 0f)]
    public abstract class GameModeModule : Module
    {
        [NonSerialized]
        public GameModeData gameMode;
    }
}

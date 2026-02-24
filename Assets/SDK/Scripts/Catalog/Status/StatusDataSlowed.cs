
using TriInspector;

namespace ThunderRoad
{
    [DeclareBoxGroup("Slow Effect", Title = "Slow Effect")]
    public class StatusDataSlowed : StatusData
    {
        [Group("Slow Effect")]
        public float baseSlowMult = 0.3f;
    }

}
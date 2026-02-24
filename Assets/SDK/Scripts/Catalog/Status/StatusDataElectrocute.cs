
using TriInspector;

namespace ThunderRoad
{
    [DeclareBoxGroup("Player Speeds", Title = "Player Speeds")]
    public class StatusDataElectrocute : StatusData
    {
        [Group("Player Speeds")]
        public float playerSpeedModifier = 0.2f;
        [Group("Player Speeds")]
        public float chargeSpeedModifier = 0.2f;
    }
}

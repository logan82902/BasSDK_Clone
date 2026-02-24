using Newtonsoft.Json;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class AttackFirearm : AttackRanged
    {
        public enum FirearmOptions
        {
            MainHand,
            OffHand,
            DualWield
        }

        public FirearmOptions firearmToShoot = FirearmOptions.MainHand;

    }
}

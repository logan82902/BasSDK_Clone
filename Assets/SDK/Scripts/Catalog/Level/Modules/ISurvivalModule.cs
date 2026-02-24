using UnityEngine;

namespace ThunderRoad
{
    public interface ISurvivalModule
    {
        public bool TryGetTeleportPosition(out Vector3 position);
        public bool TryGetPillarSounds(out AudioContainer raise, out AudioContainer lower);
    }
}
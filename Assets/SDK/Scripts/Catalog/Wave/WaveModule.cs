using System.Collections;

namespace ThunderRoad
{
    public class WaveModule
    {
        public virtual void OnCatalogRefresh() {}

        public virtual IEnumerator OnCatalogRefreshCoroutine()
        {
            yield return null;
        }

        public virtual void OnWaveStart(WaveSpawner spawner)
        {
        }
        public virtual void OnWaveEnd(WaveSpawner spawner) {}

        public void OnSpawnCreature(WaveSpawner waveSpawner, Creature spawned)
        {
        }
    }
}
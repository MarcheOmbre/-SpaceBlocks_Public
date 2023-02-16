using TravelMind.Currencies.Loot;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Entities.Debris
{
    public class CurrencyDebris : DebrisContent
    {
        [SerializeField] private CurrencyLoot currencyLootPrefab;
        [SerializeField] [Min(0)] private float activationTime;
        
        private float spawnTime;
        
        private void OnEnable()
        {
            spawnTime = Time.time;
            Rigidbody2D.isKinematic = false;
        }

        private void Update()
        {
            if (Time.time - spawnTime < activationTime)
                return;

            Pools.Despawn(gameObject, Pools.PoolType.EntitiesSpaceDebris);
            Pools.Spawn(currencyLootPrefab.gameObject, Pools.PoolType.Currency, transform.position, Quaternion.identity);
        }
    }
}

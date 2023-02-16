using Interactions.Abstracts;
using Interactions.Interfaces;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Entities.Projectiles
{
    public abstract class AProjectile : AFireableMonoBehaviour
    {
        private float currentMaxDistance;
        private Vector2 startPosition;

        protected virtual void OnEnable() => startPosition = transform.position;

        protected virtual void Update()
        {
            if (Vector2.Distance(startPosition, transform.position) >= currentMaxDistance)
                Despawn();
        }

        protected void Despawn() => Pools.Despawn(gameObject, Pools.PoolType.EntitiesProjectiles);
        
        public override void Initialize(Vector2 direction, IAttackable target, float maxDistance) => currentMaxDistance = maxDistance;
    }
}

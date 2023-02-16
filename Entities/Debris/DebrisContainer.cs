using System;
using Interactions.Interfaces;
using TravelMind.Plugins.Interactions;
using TravelMind.Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TravelMind.Entities.Debris
{
    public class DebrisContainer : MonoBehaviour, IMinable
    {
        [Serializable]
        private struct ContentInfo
        {
            public DebrisContent debrisContentPrefab;
            public int amount;
        }
        

        private const int MaxPrefabsPerResource = 10;

        public Rigidbody2D Rigidbody2D => rigidbody2D;
        
        public bool IsTargettable => isActiveAndEnabled && CurrentHealth > 0;
        
        public Vector2 Position => transform.position;
        
        public float MaxHealth => maxHealth;
        
        public float CurrentHealth { get; private set; }
        
        
        [SerializeField] private new Renderer renderer;
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private float maxHealth;
        [SerializeField] private ContentInfo[] contentInfo;
        [SerializeField] [Min(0)] private float explosionForce = 10f;
        [SerializeField] private float damagesBaseOnCollision = 1f;
        
        private Vector2 lastVelocity;

        private void OnEnable()
        {
            //Set variables
            CurrentHealth = maxHealth;
        }

        private void FixedUpdate()
        {
            lastVelocity = rigidbody2D.velocity;
        }

        private void Update()
        {
            var isVisible = Utility2D.IsInCameraView(InteractionsManager.Instance.InteractionCamera, renderer);
            if (!isVisible && Vector2.Dot(rigidbody2D.velocity.normalized, (-transform.position).normalized) <= 0)
                Pools.Despawn(gameObject, Pools.PoolType.EntitiesSpaceDebris);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            //Reflect
            rigidbody2D.velocity = Vector2.Reflect(lastVelocity, collision.contacts[0].normal);

            //Attack
            if (!collision.collider.TryGetComponent<IAttackable>(out var attackable))
                return;
            
            attackable.Attack(damagesBaseOnCollision * lastVelocity.magnitude);
        }

        private void DistributeResources()
        {
            foreach (var content in contentInfo)
            {
                var resourcesCount = Mathf.Min(content.amount, MaxPrefabsPerResource);
                
                for (var i = 0; i < resourcesCount; i++)
                {
                    var spawnedResource = Pools.Spawn<DebrisContent>(content.debrisContentPrefab.gameObject,
                        Pools.PoolType.EntitiesSpaceDebris, transform.position);
                    
                    spawnedResource.Rigidbody2D.AddForce(Random.insideUnitCircle.normalized * explosionForce, ForceMode2D.Impulse);
                }
            }
        }
        
        public void Mint(float damage)
        {
            if (CurrentHealth <= 0)
                return;
            
            CurrentHealth -= damage;
            
            if (CurrentHealth > 0)
                return;
            
            DistributeResources();
            Pools.Despawn(gameObject, Pools.PoolType.EntitiesSpaceDebris);
        }
    }
}

using System;
using Interactions.Abstracts;
using Interactions.Interfaces;
using TravelMind.Components.Core.Abstracts;
using TravelMind.Components.Core.Interfaces;
using TravelMind.Plugins.Utils;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Components.Core.Implementations
{
    public class SimpleWeapon : AComponent, IWeaponBehaviour
    {
        public override IEngineBehaviour EngineBehaviour => null;
        public override IShieldBehaviour ShieldBehaviour => null;
        public override IMinerBehaviour MinerBehaviour => null;
        public override IThrusterBehaviour ThrusterBehaviour => null;
        public override IWeaponBehaviour WeaponBehaviour => this;

        private IAttackable CurrentTarget { get; set; }
        

        [Header("References")]
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private AFireableMonoBehaviour projectilePrefab;
        
        [Header("Shots")]
        [SerializeField] [Min(0)] private float shotEnergyCost = 0.5f;
        [SerializeField] [Min(0)] private int maxShotsPerBurst = 3;
        
        [Header("Cooldown")]
        [SerializeField] [Min(0)] private float timeBetweenShots = 0.5f;
        [SerializeField] [Min(0)] private float burstCooldown = 0.5f;
        
        [Header("Capacity")]
        [SerializeField] [Min(1f)] private float maxRange = 10f;
        [SerializeField] [Min(0)] private float rotationSpeed = 5f;

        private Shared.EnergyCollector currentEnergyCollector;
        private Func<IAttackable> currentTargetGetter;
        private Transform currentShipTransform;
        private int currentEnemyLayer;
        
        private float targetRefreshTimer;
        private float burstTimer;
        private float shotTimer;
        private int burstShots;

        public override bool IsEnabled => base.IsEnabled && currentEnergyCollector != null 
                                                         && currentShipTransform != null && currentTargetGetter != null;

        
        private void Update()
        {
            //Handle timers
            if (burstShots >= maxShotsPerBurst)
            {
                if (burstTimer > 0)
                    burstTimer -= Time.deltaTime;
                else
                    burstShots = 0;
            }
            
            if (shotTimer > 0)
                shotTimer -= Time.deltaTime;
            
            if (!IsEnabled)
                return;
            
            CheckTarget();
            
            RefreshRotation();
        }

        private void CheckTarget()
        {
            if(CurrentTarget is {IsTargettable: false})
                CurrentTarget = null;
            
            //Target refreshment
            targetRefreshTimer += Time.deltaTime;
            if (targetRefreshTimer < Ship.TargetRefreshRate) 
                return;
                
            targetRefreshTimer = 0;
   
            //Check brain target
            var brainTarget = currentTargetGetter();
            if (brainTarget != null && CurrentTarget != brainTarget && Vector2.Distance(brainTarget.Position, projectileSpawnPoint.position) <= maxRange)
                CurrentTarget = brainTarget;

            //If no target, check if current target is still alive
            if (CurrentTarget != null)
            {
                if(CurrentTarget.CurrentHealth <= 0)
                    CurrentTarget = null;
                else
                    return;
            }

            //Filter resources by distance
            var colliders = Physics2D.OverlapCircleAll(projectileSpawnPoint.position, maxRange, 1 << currentEnemyLayer);

            foreach (var overlappedCollider in colliders)
            {
                if (!overlappedCollider.TryGetComponent<IAttackable>(out var attackable))
                    continue;

                CurrentTarget = attackable;
                break;
            }
                
        }

        private void RefreshRotation()
        {
            var rotation = CurrentTarget == null ||  CurrentTarget.CurrentHealth <= 0 ? currentShipTransform.rotation : 
                Quaternion.LookRotation(Vector3.forward, (CurrentTarget.Position - (Vector2)transform.position).normalized);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        public void Initialize(Shared.EnergyCollector energyCollector, Func<IAttackable> attackableCollector, Transform shipTransform, int enemyLayer)
        {
            currentEnergyCollector = energyCollector;
            currentTargetGetter = attackableCollector;
            currentShipTransform = shipTransform;
            currentEnemyLayer = enemyLayer;
            
            transform.rotation = shipTransform.rotation;
        }

        public override void Use()
        {
            if (!IsEnabled)
                return;
            
            //Check burst cooldown
            if (burstShots >= maxShotsPerBurst)
                return;

            //Check shot cooldown
            if (shotTimer > 0)
                return;

            //Mint resources
            if (!currentEnergyCollector(shotEnergyCost, true, out var _))
                return;
            
            shotTimer = timeBetweenShots;
            burstShots++;
            
            if(burstShots >= maxShotsPerBurst)
                burstTimer = burstCooldown;
            
            //Spawn projectile
            var projectile = Pools.Spawn<AFireableMonoBehaviour>(projectilePrefab.gameObject, Pools.PoolType.EntitiesProjectiles, 
                projectileSpawnPoint.position, projectileSpawnPoint.rotation);

            projectile.gameObject.SetLayerRecursively(gameObject.layer);
            projectile.Initialize(transform.up, CurrentTarget, maxRange);
        }
    }
}
using System;
using Interactions.Interfaces;
using TravelMind.Components.Core.Abstracts;
using TravelMind.Components.Core.Interfaces;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Components.Core.Implementations
{
    public class SimpleMiner : AComponent, IMinerBehaviour
    {
        public override IEngineBehaviour EngineBehaviour => null;
        public override IShieldBehaviour ShieldBehaviour => null;
        public override IMinerBehaviour MinerBehaviour => this;
        public override IThrusterBehaviour ThrusterBehaviour => null;
        public override IWeaponBehaviour WeaponBehaviour => null;

        
        public float CurrentNormalizedEnergyConsumption { get; private set; }
        
        public IMinable CurrentTarget { get; private set; }

        [SerializeField] [Min(0)] private float drainRate = 0.5f;
        [SerializeField] [Min(0)] private float power = 0.5f;
        [SerializeField] [Min(1f)] private float maxRange;
        [SerializeField] [Min(0)] private float loadTime = 0.5f;

        private float targetRefreshTimer;
        private float loadCurrentTimer;
        
        private Shared.EnergyCollector currentEnergyCollector;
        private Func<IMinable> currentTargetGetter;

        public override bool IsEnabled => base.IsEnabled && currentEnergyCollector != null && currentTargetGetter != null;
        
        private void Update()
        {
            //Check if enabled
            if (!IsEnabled)
            {
                CurrentNormalizedEnergyConsumption = 0;
                return;
            }
            
            CheckTarget();
            
            //Compute energy consumption
            if (CurrentTarget == null)
                return;
            
            //Current target is null
            CurrentNormalizedEnergyConsumption = 0;
            
            loadCurrentTimer -= Time.deltaTime;
            if (loadCurrentTimer <= 0)
                return;

            if (!currentEnergyCollector(drainRate * Time.deltaTime, false, out var collectedEnergy))
                return;
            
            CurrentNormalizedEnergyConsumption = Mathf.Clamp01(collectedEnergy / Time.deltaTime / drainRate);
            CurrentTarget.Mint(CurrentNormalizedEnergyConsumption * power * Time.deltaTime);
        }
        
        public void Initialize(Shared.EnergyCollector energyCollector, Func<IMinable> targetGetter)
        {
            currentEnergyCollector = energyCollector;
            currentTargetGetter = targetGetter;
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
            if (brainTarget != null && CurrentTarget != brainTarget && Vector2.Distance(brainTarget.Position, transform.position) <= maxRange)
                CurrentTarget = brainTarget;

                //Check new resources (if no resource is selected)
            if (CurrentTarget != null)
            {
                if(CurrentTarget.CurrentHealth <= 0)
                    CurrentTarget = null;
                else
                    return;
            }

            //Filter resources by distance
            var colliders = Physics2D.OverlapCircleAll(transform.position, maxRange, 1 << Layers.DebrisLayer);
            foreach (var overlappedCollider in colliders)
            {
                if (!overlappedCollider.TryGetComponent<IMinable>(out var minable))
                    continue;

                CurrentTarget = minable;
                break;
            }
        }

        public override void Use() => loadCurrentTimer = loadTime;
    }
}

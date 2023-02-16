using System;
using TravelMind.Components.Core.Abstracts;
using TravelMind.Components.Core.Interfaces;
using UnityEngine;

namespace TravelMind.Components.Core.Implementations
{
    public class SimpleThruster : AComponent, IThrusterBehaviour
    {
        public override IEngineBehaviour EngineBehaviour => null;
        public override IShieldBehaviour ShieldBehaviour => null;
        public override IMinerBehaviour MinerBehaviour => null;
        public override IThrusterBehaviour ThrusterBehaviour => this;
        public override IWeaponBehaviour WeaponBehaviour => null;


        public float CurrentNormalizedEnergyConsumption { get; private set; }


        [SerializeField] [Min(0)] private float drainRate = 0.5f;
        [SerializeField] [Min(0)] private float power = 0.5f;
        [SerializeField] [Min(0)] private float loadTime = 0.5f;
        
        private Shared.EnergyCollector currentEnergyCollector;
        private Action<float> currentOnThrust;
        private float loadCurrentTimer;

        public override bool IsEnabled => base.IsEnabled && currentEnergyCollector != null;
        
        private void Update()
        {
            //Check if enable
            if (!IsEnabled)
            {
                CurrentNormalizedEnergyConsumption = 0;
                return;
            }
            
            //Compute energy consumption
            CurrentNormalizedEnergyConsumption = 0;

            loadCurrentTimer -= Time.deltaTime;
            if (loadCurrentTimer <= 0)
                return;
            
            if (currentEnergyCollector == null || !currentEnergyCollector(drainRate * Time.deltaTime, false, out var collectedEnergy))
                return;
            
            CurrentNormalizedEnergyConsumption = Mathf.Clamp01(collectedEnergy / Time.deltaTime / drainRate);
            //TODO: Distance calculation based different factors
            currentOnThrust(CurrentNormalizedEnergyConsumption * power * Time.deltaTime);
        }
        
        public void Initialize(Shared.EnergyCollector energyCollectorCollector, Action<float> onThrust)
        {
            currentEnergyCollector = energyCollectorCollector;
            currentOnThrust = onThrust;
        }

        public override void Use() => loadCurrentTimer = loadTime;
    }
}
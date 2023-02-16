using TravelMind.Components.Core.Abstracts;
using TravelMind.Components.Core.Interfaces;
using UnityEngine;

namespace TravelMind.Components.Core.Implementations
{
    public class SimpleEngine : AComponent, IEngineBehaviour
    {
        public override IEngineBehaviour EngineBehaviour => this;
        public override IShieldBehaviour ShieldBehaviour => null;
        public override IMinerBehaviour MinerBehaviour => null;
        public override IThrusterBehaviour ThrusterBehaviour => null;
        public override IWeaponBehaviour WeaponBehaviour => null;

        public float BatteryTotalCapacity => energyMaxCapacity;
        public float BatteryCurrentCapacity { get; private set; }
        

        [SerializeField] [Min(1)] private float energyMaxCapacity = 10f;
        [SerializeField] [Min(0.1f)] private float energyFillRate = 0.2f;

        
        private void OnEnable() => BatteryCurrentCapacity = energyMaxCapacity;

        public void GetEnergy(float expectedEnergy, out float energyTaken)
        {
            energyTaken = 0;

            if (!IsEnabled)
                return;
            
            energyTaken = Mathf.Min(expectedEnergy, BatteryCurrentCapacity);
            BatteryCurrentCapacity -= energyTaken;
        }

        public override void Use()
        {
            if(!IsEnabled)
                return;
            
            BatteryCurrentCapacity = Mathf.Min(energyMaxCapacity, BatteryCurrentCapacity + energyFillRate);
        }
    }
}

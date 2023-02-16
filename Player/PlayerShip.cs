using System.Collections.Generic;
using System.Linq;
using Interactions.Interfaces;
using TravelMind.Blocks.Core;
using TravelMind.Components.Core.Abstracts;
using TravelMind.Entities.Abstracts;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Player
{
    public class PlayerShip : AShip
    {
        protected override int EnemyLayer => Layers.EnemiesLayer;
        
        private IMinable currentMinableTarget;
        private IAttackable currentAttackableTarget;
        
        private readonly List<AComponent> engineComponents = new();
        
        [SerializeField] [Min(0)] private float healthDeadValue;

        protected override void Update()
        {
            base.Update();
            
            if(currentMinableTarget is {IsTargettable: false})
                currentMinableTarget = null;
            
            if(currentAttackableTarget is {IsTargettable: false})
                currentAttackableTarget = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            currentMinableTarget = null;
            currentAttackableTarget = null;
        }

        #region Blocks
        
        protected override void OnHealthChanged(float currentHealth, float totalHealth, ShipBlock shipBlock)
        {
            if(currentHealth <= healthDeadValue)
                Debug.Log("You're dead");
        }

        protected override void OnBlockRemoved(int currentBlocks, int totalBlocks, ShipBlock shipBlock)
        {
            //Check block
        }

        protected override void OnComponentRemoved(Component component)
        {
            //Check component
        }
        
        #endregion
        
        #region Components

        protected override void ClearComponents()
        {
            base.ClearComponents();
            
            engineComponents.Clear();
        }

        protected override void OnRefreshComponent(AComponent component)
        {
            base.OnRefreshComponent(component);
            
            if(component.IsEngine)
                engineComponents.Add(component);
        }

        protected override bool TryGetEnergy(float requestedEnergy, bool needAllEnergy, out float collectedEnergy)
        {
            collectedEnergy = 0f;
            
            do
            {
                if (needAllEnergy && engineComponents.Sum(x => x.EngineBehaviour.BatteryCurrentCapacity) < requestedEnergy)
                    return false;
                
                //Check available engines
                var filledEngines = engineComponents.Where(x => x.IsEnabled && x.EngineBehaviour.BatteryCurrentCapacity > 0).ToArray();
                if (filledEngines.Length <= 0)
                    return false;

                //Compute rate
                var energyRate = (requestedEnergy - collectedEnergy) / filledEngines.Length;
                
                //Get energy
                foreach (var engine in filledEngines)
                {
                    engine.EngineBehaviour.GetEnergy(energyRate, out var energyTaken);
                    collectedEnergy = Mathf.Clamp(collectedEnergy + energyTaken, 0, requestedEnergy);
                }
                
            } while (collectedEnergy < requestedEnergy);

            return true;
        }
        
        public override IMinable GetCurrentMinableTarget() => currentMinableTarget;

        public override IAttackable GetCurrentAttackableTarget() => currentAttackableTarget;

        public void SetMinableTarget(IMinable minable) => currentMinableTarget = minable;

        public void SetAttackableTarget(IAttackable attackable) => currentAttackableTarget = attackable;

        #endregion
    }
}

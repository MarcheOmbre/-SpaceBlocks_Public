using System;
using System.Collections.Generic;
using Interactions.Interfaces;
using TravelMind.Blocks.Core;
using TravelMind.Builder.Data;
using TravelMind.Components.Core.Abstracts;
using TravelMind.Entities.Abstracts;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Enemies
{
    public class SimpleLoadableEnemyShip : AShip
    {
        public event Action OnKilled = delegate { };
        
        
        [SerializeField] private BuilderResourcesLinker builderResourcesLinker;
        
        private readonly List<AComponent> weapons = new();


        protected override void Update()
        {
            base.Update();

            foreach (var weapon in weapons) 
                weapon.Use();
        }

        public void Initialize(BuilderDataContainer dataContainer) =>
            builderResourcesLinker.LoadFromContainer(dataContainer);

        #region Blocks
        
        protected override void OnHealthChanged(float currentHealth, float totalHealth, ShipBlock shipBlock) { }

        protected override void OnBlockRemoved(int currentBlocks, int totalBlocks, ShipBlock shipBlock)
        {
            if (currentBlocks > 0)
                return;
            
            Pools.Despawn(gameObject, Pools.PoolType.EntitiesShips);
            OnKilled();
        }

        protected override void OnComponentRemoved(Component component) { }

        #endregion
        
        #region Components
        //Unlimited energy for enemy
        protected override int EnemyLayer => Layers.PlayerLayer;

        protected override void ClearComponents()
        {
            base.ClearComponents();
            
            weapons.Clear();
        }

        protected override void OnRefreshComponent(AComponent component)
        {
            base.OnRefreshComponent(component);

            if(component.IsWeapon)
                weapons.Add(component);
        }

        protected override bool TryGetEnergy(float requestedEnergy, bool needAllEnergy, out float collectedEnergy)
        {
            collectedEnergy = requestedEnergy;
            return true;
        }

        public override IMinable GetCurrentMinableTarget() => null;
        public override IAttackable GetCurrentAttackableTarget() => null;
        
        #endregion
    }
}

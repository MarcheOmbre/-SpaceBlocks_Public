using System;
using System.Collections.Generic;
using System.Linq;
using Interactions.Interfaces;
using TravelMind.Blocks.Core;
using TravelMind.Components.Core;
using TravelMind.Components.Core.Abstracts;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Entities.Abstracts
{
    public abstract class AShip : MonoBehaviour
    {
        public event Action<float> OnThrust = delegate { };
        
        public event Action OnComponentsChanged = delegate { };
        
        
        protected BlocksGridManager BlocksGridManager => blocksGridManager;
        
        protected ComponentsGridManager ComponentsGridManager => componentsGridManager;

        protected abstract int EnemyLayer { get; }
        
        
        [SerializeField] private BlocksGridManager blocksGridManager;
        [SerializeField] private ComponentsGridManager componentsGridManager;

        private readonly List<ShipBlock> shipBlocks = new();
        
        private float totalHealth;
        private float currentHealth;
        private int totalBlocks;
        private int currentBlocks;
        private float currentThrustForce;
        
        
        protected virtual void OnEnable()
        {
            blocksGridManager.OnBlockRemoved += OnBlockRemoved;
            componentsGridManager.OnChanged += RefreshComponents;
            
            RefreshBlocks();
            RefreshComponents();
        }
        
        protected virtual void Update()
        {
            if (!(currentThrustForce > 0)) 
                return;
            
            OnThrust(currentThrustForce);
            currentThrustForce = 0;
        }

        protected virtual void OnDisable()
        {
            componentsGridManager.OnChanged -= RefreshComponents;
            blocksGridManager.OnBlockRemoved -= OnBlockRemoved;
        }
        
        #region Blocks
        
        private void OnBlockRemoved(Vector2Int coordinates)
        {
            var component = componentsGridManager.GetAtCoordinates(coordinates);
            if (component == null)
                return;
            
            componentsGridManager.Remove(coordinates);
            OnComponentRemoved(component);
        }

        private void RefreshBlocks()
        {
            ClearBlocks();
            
            totalHealth = 0;
            totalBlocks = 0;

            foreach (var block in blocksGridManager.GetByAvailability(Ship.Availability.Occupied)
                         .Select(vector2Int => blocksGridManager.GetAtCoordinates(vector2Int))
                         .Where(block => block != null))
            {
                block.OnAttacked += OnAttacked;
                block.OnDepleted += OnDepleted;
                
                totalHealth += block.MaxHealth; 
                totalBlocks ++;
                
                shipBlocks.Add(block);
            }

            currentHealth = totalHealth;
            currentBlocks = totalBlocks;
        }
        
        protected virtual void ClearBlocks()
        {
            foreach (var shipBlock in shipBlocks)
            {
                shipBlock.OnAttacked -= OnAttacked;
                shipBlock.OnDepleted -= OnDepleted;
            }
            
            shipBlocks.Clear();
        }
        
        private void OnAttacked(ShipBlock shipBlock, float damages)
        {
            currentHealth -= damages;
            
            OnHealthChanged(currentHealth, totalHealth, shipBlock);
        }

        private void OnDepleted(ShipBlock shipBlock)
        {
            currentBlocks--;
            
            OnBlockRemoved(currentBlocks, totalBlocks, shipBlock);
        }
        
        protected abstract void OnHealthChanged(float currentHealth, float totalHealth, ShipBlock shipBlock);
        
        protected abstract void OnBlockRemoved(int currentBlocks, int totalBlocks, ShipBlock shipBlock);
        
        #endregion
        
        #region Components
        
        private void OnThrusterThrust(float force) => currentThrustForce += force;
        
        private void RefreshComponents()
        {
            ClearComponents();
            
            var componentsCoordinates = componentsGridManager.GetByAvailability(Ship.Availability.Occupied);

            foreach (var coordinates in componentsCoordinates)
            {
                var component = componentsGridManager.GetAtCoordinates(coordinates);
                OnRefreshComponent(component);
            }
            
            OnComponentsChanged();
        }
        
        protected virtual void ClearComponents() { }

        protected virtual void OnRefreshComponent(AComponent component)
        {
            if(component.IsThruster)
                component.ThrusterBehaviour.Initialize(TryGetEnergy, OnThrusterThrust);
            
            if(component.IsMiner)
                component.MinerBehaviour.Initialize(TryGetEnergy, GetCurrentMinableTarget);
            
            if(component.IsWeapon)
                component.WeaponBehaviour.Initialize(TryGetEnergy, GetCurrentAttackableTarget, transform, EnemyLayer);
        }
        
        protected abstract void OnComponentRemoved(Component component);

        protected abstract bool TryGetEnergy(float requestedEnergy, bool needAllEnergy, out float collectedEnergy);
        
        public abstract IMinable GetCurrentMinableTarget();

        public abstract IAttackable GetCurrentAttackableTarget();
        
        #endregion
    }
}

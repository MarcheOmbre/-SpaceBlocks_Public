using System;
using Interactions.Interfaces;
using TravelMind.Plugins.Utils;
using UnityEngine;

namespace TravelMind.Blocks.Core
{
    public class ShipBlock : IdentifiableMonoBehaviour, IAttackable
    {
        public event Action<ShipBlock, float> OnAttacked = delegate { };
        public event Action<ShipBlock> OnDepleted = delegate { };
        
        public bool IsDestructible => isDestructible;
        
        public bool IsConstructible => isConstructible;

        public bool IsTargettable => isActiveAndEnabled && CurrentHealth > 0;
        
        public Vector2 Position => transform.position;

        public float MaxHealth => maxHealth;
        
        public float CurrentHealth { get; private set; }


        [SerializeField] private bool isDestructible;
        [SerializeField] private bool isConstructible;
        [SerializeField] [Min(0)] private float maxHealth = 100;

        private float currentHealth;

        private void OnEnable() => CurrentHealth = maxHealth;

        internal void Kill()
        {
            OnDepleted.Invoke(this);
            CurrentHealth = 0;
        }

        public void Attack(float damage)
        {
            if (CurrentHealth <= 0)
                return;
            
            CurrentHealth -= damage;
            OnAttacked.Invoke(this, damage);
            
            if (CurrentHealth > 0)
                return;
            
            Kill();
        }
    }
}

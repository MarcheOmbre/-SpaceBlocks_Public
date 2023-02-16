using UnityEngine;

namespace Interactions.Interfaces
{
    public interface IAttackable
    {
        public bool IsTargettable { get; }
        
        public Vector2 Position { get; }
        
        public float MaxHealth { get; }
        
        public float CurrentHealth { get; }


        public void Attack(float damage);
    }
}

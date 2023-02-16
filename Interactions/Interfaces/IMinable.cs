using UnityEngine;

namespace Interactions.Interfaces
{
    public interface IMinable
    {
        public bool IsTargettable { get; }
        
        public Vector2 Position { get; }
        
        public float MaxHealth { get; }
        
        public float CurrentHealth { get; }


        public void Mint(float damage);
    }
}

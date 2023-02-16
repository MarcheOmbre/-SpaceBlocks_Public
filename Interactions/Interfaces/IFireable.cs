using UnityEngine;

namespace Interactions.Interfaces
{
    public interface IFireable
    {
        public void Initialize(Vector2 direction, IAttackable target, float maxDistance);
    }
}

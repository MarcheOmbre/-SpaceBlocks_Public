using Interactions.Interfaces;
using UnityEngine;

namespace Interactions.Abstracts
{
    public abstract class AFireableMonoBehaviour : MonoBehaviour, IFireable
    {
        public abstract void Initialize(Vector2 direction, IAttackable target, float maxDistance);
    }
}

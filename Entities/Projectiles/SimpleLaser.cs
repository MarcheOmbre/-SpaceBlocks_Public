using Interactions.Interfaces;
using UnityEngine;

namespace TravelMind.Entities.Projectiles
{
    public class SimpleLaser : AProjectile
    {
        [SerializeField] [Min(0.1f)] private float speed;
        [SerializeField] [Min(0)] private float damages;
        
        private Vector2 currentDirection;


        protected override void Update()
        {
            base.Update();
            
            var distance = speed * Time.deltaTime;
            transform.position += (Vector3) (currentDirection * distance);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out IAttackable attackable))
                return;
            
            //Deal damages to the target and despawn
            attackable.Attack(damages);
            Despawn();
        }

        public override void Initialize(Vector2 direction, IAttackable target, float maxDistance)
        {
            base.Initialize(direction, target, maxDistance);
            currentDirection = direction.normalized;
        }
    }
}

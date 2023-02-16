using UnityEngine;

namespace TravelMind.Entities.Debris
{
    public class DebrisContent : MonoBehaviour
    {
        public Rigidbody2D Rigidbody2D => rigidbody2D;
        
        
        [SerializeField] private new Rigidbody2D rigidbody2D;
    }
}

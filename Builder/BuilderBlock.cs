using UnityEngine;

namespace TravelMind.Builder
{
    public class BuilderBlock : MonoBehaviour
    {
        public Vector2Int Coordinates { get; private set; }
        
        public void Initialize(Vector2Int coordinates)
        {
            Coordinates = coordinates;
        }
    }
}

using UnityEngine;

namespace TravelMind.Shared
{
    public static class Ship
    {
        public const int SnapDistance = 1;
        //Target behaviour
        public const int TargetRefreshRate = 1;
        
        public enum Availability
        {
            Empty,
            Occupied,
            Both
        }
        
        public static Vector3 IndexToLocalPosition(Vector2Int coordinates, Vector2Int size)
        {
            var position = Vector3.zero;

            position.x = (coordinates.x - size.x / 2) * SnapDistance;
            position.y = -(coordinates.y - size.y / 2) * SnapDistance;
            
            return position;
        }
    }
}
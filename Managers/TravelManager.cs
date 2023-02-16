using System;
using TravelMind.Entities.Abstracts;
using TravelMind.Plugins.Utils;
using UnityEngine;

namespace TravelMind.Managers
{
    public class TravelManager : MonoBehaviourSingleton<TravelManager>
    {
        public event Action OnGoalReached = delegate { };
        
        public float GoalDistance => goalDistance;
        
        public float DistanceTravelled { get; private set; }

        
        [SerializeField] private float goalDistance = 100f;
        [SerializeField] private AShip ship;
        
        private bool goalReached;
        
        
        private void OnEnable()
        {
            ship.OnThrust += MoveForward;
        }

        private void OnDisable()
        {
            ship.OnThrust -= MoveForward;
        }

        private void MoveForward(float distance)
        {
            DistanceTravelled = Mathf.Clamp(DistanceTravelled + distance, 0, GoalDistance);
            
            if(goalReached || DistanceTravelled < GoalDistance) 
                return;

            OnGoalReached();
            goalReached = true;
        }
    }
}

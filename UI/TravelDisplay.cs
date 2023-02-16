using TMPro;
using TravelMind.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace TravelMind.UI
{
    public class TravelDisplay : MonoBehaviour
    {
        [SerializeField] private TravelManager travelManager;
        [SerializeField] private Slider progressionSlider;
        [SerializeField] private TMP_Text distanceText;
        [SerializeField] [Min(0.1f)] private float resultMultiplier = 1f;
        [SerializeField] private string distanceTextFormat = "F2 m";

        private void OnEnable() => RefreshDisplay();

        private void Update() => RefreshDisplay();

        private void RefreshDisplay()
        {
            progressionSlider.maxValue = travelManager.GoalDistance;
            progressionSlider.value = travelManager.DistanceTravelled;

            var remainingDistance = travelManager.GoalDistance - travelManager.DistanceTravelled;
            distanceText.SetText((remainingDistance * resultMultiplier).ToString(distanceTextFormat));
        }
    }
}

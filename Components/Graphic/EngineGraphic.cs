using TravelMind.Components.Core.Implementations;
using UnityEngine;
using UnityEngine.UI;

namespace TravelMind.Components.Graphic
{
    public class EngineGraphic : MonoBehaviour
    {
        [SerializeField] private SimpleEngine simpleEngine;
        [SerializeField] private Image notEmptyBackgroundImage;
        [SerializeField] private Image emptyBackgroundImage;
        [SerializeField] private Image fillImage;

        private void OnEnable() => RefreshEffect();

        private void Update() => RefreshEffect();

        private void RefreshEffect()
        {
            var capacityRate = Mathf.Clamp01(simpleEngine.BatteryCurrentCapacity / simpleEngine.BatteryTotalCapacity);
            notEmptyBackgroundImage.enabled = capacityRate > 0;
            emptyBackgroundImage.enabled = capacityRate <= 0;
            fillImage.fillAmount = capacityRate;
        }
    }
}

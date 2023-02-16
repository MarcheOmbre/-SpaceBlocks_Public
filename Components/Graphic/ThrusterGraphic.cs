using TravelMind.Components.Core.Implementations;
using UnityEngine;

namespace TravelMind.Components.Graphic
{
    public class ThrusterGraphic : MonoBehaviour
    {
        [SerializeField] private SimpleThruster simpleThruster;
        [SerializeField] private new ParticleSystem particleSystem;

        private float initialEmissionRateOverTimeMultiplier;

        private void Awake()
        {
            initialEmissionRateOverTimeMultiplier = particleSystem.emission.rateOverTimeMultiplier;
        }

        private void OnEnable() => RefreshEffect();

        private void Update() => RefreshEffect();

        private void RefreshEffect()
        {
            var thrusterEmissionRate = simpleThruster.CurrentNormalizedEnergyConsumption;
            var emission = particleSystem.emission;
            emission.rateOverTimeMultiplier = initialEmissionRateOverTimeMultiplier * thrusterEmissionRate;
        }
    }
}

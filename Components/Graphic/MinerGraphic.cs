using TravelMind.Components.Core.Implementations;
using UnityEngine;

namespace TravelMind.Components.Graphic
{
    public class MinerGraphic : MonoBehaviour
    {
        [SerializeField] private SimpleMiner simpleMiner;
        [SerializeField] private LineRenderer lineRenderer;

        private void Update()
        {
            var currentMiningTarget = simpleMiner.CurrentTarget;

            var hasTarget = currentMiningTarget != null;
            lineRenderer.gameObject.SetActive(hasTarget);
            if (!hasTarget)
                return;
            
            lineRenderer.SetPositions(new []{simpleMiner.transform.position, (Vector3)currentMiningTarget.Position});
            lineRenderer.widthMultiplier = simpleMiner.CurrentNormalizedEnergyConsumption;
        }
    }
}

using TravelMind.Entities.Abstracts;
using TravelMind.Plugins.Interactions;
using UnityEngine;

namespace TravelMind.Entities.UI
{
    public class ShipDisplay : MonoBehaviour
    {
        [SerializeField] private AShip ship;
        [SerializeField] private RectTransform miningAimingArea;
        [SerializeField] private RectTransform combatAimingArea;

        private void Update()
        {
            RefreshMiningAimingArea();

            RefreshCombatAimingArea();
        }
        
        private void RefreshMiningAimingArea()
        {
            var minableTarget = ship.GetCurrentMinableTarget();
            var isMiningActive = minableTarget != null;
            miningAimingArea.gameObject.SetActive(isMiningActive);

            if (!isMiningActive)
                return;

            miningAimingArea.position = InteractionsManager.Instance.InteractionCamera
                .WorldToScreenPoint(minableTarget.Position);
        }
        
        private void RefreshCombatAimingArea()
        {
            var attackableTarget = ship.GetCurrentAttackableTarget();
            var isCombatActive = attackableTarget != null;
            combatAimingArea.gameObject.SetActive(isCombatActive);

            if (!isCombatActive)
                return;

            combatAimingArea.position = InteractionsManager.Instance.InteractionCamera
                .WorldToScreenPoint(attackableTarget.Position);
        }
    }
}

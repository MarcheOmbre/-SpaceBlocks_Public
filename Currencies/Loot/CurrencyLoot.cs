using DG.Tweening;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Currencies.Loot
{
    public class CurrencyLoot : MonoBehaviour
    {
        [SerializeField] private Currency currency;
        [SerializeField] private float moveDuration = 0.5f;
        [SerializeField] private Ease moveEase = Ease.OutQuad;

        private void OnEnable()
        {
            transform.DOMove(LootPointsManager.Instance.PlayerPosition, moveDuration)
                .SetEase(moveEase)
                .OnComplete(OnSequenceFinished);
        }

        private void OnDisable() => transform.DOKill();

        private void OnSequenceFinished()
        {
            Pools.Despawn(gameObject, Pools.PoolType.Currency);
            CurrenciesManager.Instance.AddValue(currency, 1);
        }
    }
}

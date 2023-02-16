using TMPro;
using UnityEngine;

namespace TravelMind.Currencies.UI
{
    public class CurrencyDisplay : MonoBehaviour
    {
        [SerializeField] private Currency currency;
        [SerializeField] private TMP_Text currencyText;

        private void OnEnable()
        {
            if(CurrenciesManager.Instance)
                CurrenciesManager.Instance.SubscribeCurrencyChanged(currency, Refresh);
        }

        private void OnDisable()
        {
            if(CurrenciesManager.Instance)
                CurrenciesManager.Instance.UnsubscribeCurrencyChanged(currency, Refresh);
        }

        private void Refresh(int value) => currencyText.text = value.ToString();
    }
}

using System;
using System.Collections.Generic;
using TravelMind.Plugins.Utils;
using UnityEngine;

namespace TravelMind.Currencies
{
    public class CurrenciesManager : MonoBehaviourSingleton<CurrenciesManager>
    {
        private readonly Dictionary<Currency, Action<int>> currencyChangedActions = new();
        private readonly Dictionary<Currency, int> resources = new();
        
        private void Awake()
        {
            foreach (var currency in Resources.LoadAll<Currency>("Currencies/"))
            {
                if (resources.ContainsKey(currency))
                {
                    Debug.LogWarning("Currency with id " + currency.Id + " already exists");
                    continue;
                }
                
                resources.Add(currency, 0);
            }
        }
        
        public void SubscribeCurrencyChanged(Currency currency, Action<int> action)
        {
            if (!currencyChangedActions.ContainsKey(currency))
                currencyChangedActions.Add(currency, delegate {  });

            currencyChangedActions[currency] += action;
        }
        
        public void UnsubscribeCurrencyChanged(Currency currency, Action<int> action)
        {
            if (!currencyChangedActions.ContainsKey(currency))
                return;

            currencyChangedActions[currency] -= action;
        }

        public int GetValue(Currency currency)
        {
            resources.TryGetValue(currency, out var value);
            return value;
        }
        
        public void AddValue(Currency currency, int value)
        {
            if (!resources.ContainsKey(currency))
            {
                Debug.LogWarning("Currency with id " + currency.Id + " not found");
                return;
            }
            
            var newValue = resources[currency] + value;
            resources[currency] = newValue;

            if (currencyChangedActions.TryGetValue(currency, out var action))
                action.Invoke(newValue);
        }
        
        public void SubtractValue(Currency currency, int value)
        {
            if (!resources.TryGetValue(currency, out var currentValue))
            {
                Debug.LogWarning("Currency with id " + currency.Id + " not found");
                return;
            }

            var newValue = currentValue - value;
            if (newValue < 0)
            {
                Debug.LogWarning("Not enough Currency " + currency.Id);
                return;   
            }

            resources[currency] = newValue;
            
            if (currencyChangedActions.TryGetValue(currency, out var action))
                action.Invoke(newValue);
        }
    }
}

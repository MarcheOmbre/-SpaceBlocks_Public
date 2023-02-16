using System;
using Interactions.Interfaces;
using UnityEngine;

namespace TravelMind.Components.Core.Interfaces
{
    public interface IWeaponBehaviour
    {
        public void Initialize(Shared.EnergyCollector energyCollector, Func<IAttackable> attackableCollector,
            Transform shipTransform, int enemyLayer);
    }
}
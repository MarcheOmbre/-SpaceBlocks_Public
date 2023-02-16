using System;
using Interactions.Interfaces;

namespace TravelMind.Components.Core.Interfaces
{
    public interface IMinerBehaviour
    {
        public IMinable CurrentTarget { get; }
        
        public void Initialize(Shared.EnergyCollector energyCollector, Func<IMinable> targetGetter);
    }
}

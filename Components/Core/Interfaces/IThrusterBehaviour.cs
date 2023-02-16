using System;

namespace TravelMind.Components.Core.Interfaces
{
    public interface IThrusterBehaviour
    {
        public void Initialize(Shared.EnergyCollector energyCollectorCollector, Action<float> onThrust);
    }
}
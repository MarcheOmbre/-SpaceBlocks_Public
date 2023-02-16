namespace TravelMind.Components.Core
{
    public static class Shared
    {
        public delegate bool EnergyCollector(float requested, bool needAll, out float collected);
    }
}

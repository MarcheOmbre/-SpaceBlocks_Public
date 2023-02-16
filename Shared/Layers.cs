using UnityEngine;

namespace TravelMind.Shared
{
    public static class Layers
    {
        public static int PlayerLayer => LayerMask.NameToLayer("Player");
        public static int DebrisLayer => LayerMask.NameToLayer("Debris");
        public static int EnemiesLayer => LayerMask.NameToLayer("Enemies");
    }
}

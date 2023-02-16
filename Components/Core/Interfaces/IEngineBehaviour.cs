namespace TravelMind.Components.Core.Interfaces
{
    public interface IEngineBehaviour
    {
        public float BatteryTotalCapacity { get; }
        
        public float BatteryCurrentCapacity { get; }

        public void GetEnergy(float expectedEnergy, out float energyTaken);
    }
}

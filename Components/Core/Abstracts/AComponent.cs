using TravelMind.Components.Core.Interfaces;
using TravelMind.Plugins.Utils;

namespace TravelMind.Components.Core.Abstracts
{
    public abstract class AComponent : IdentifiableMonoBehaviour
    {
        public bool IsEngine => EngineBehaviour != null;
        public bool IsShield => ShieldBehaviour != null;
        public bool IsMiner => MinerBehaviour != null;
        public bool IsThruster => ThrusterBehaviour != null;
        public bool IsWeapon => WeaponBehaviour != null;
        
        public abstract IEngineBehaviour EngineBehaviour { get; }
        public abstract IShieldBehaviour ShieldBehaviour { get; }
        public abstract IMinerBehaviour MinerBehaviour { get; }
        public abstract IThrusterBehaviour ThrusterBehaviour { get; }
        public abstract IWeaponBehaviour WeaponBehaviour { get; }
        
        
        public virtual bool IsEnabled => IsSystemOn;
        
        public bool IsSystemOn { get; set; } = true;

        public abstract void Use();
    }
}
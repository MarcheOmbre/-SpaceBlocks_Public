using TravelMind.Components.Core.Abstracts;
using TravelMind.Components.Core.Interfaces;

namespace TravelMind.Components.Core.Implementations
{
    public class SimpleShield : AComponent, IShieldBehaviour
    {
        public override IEngineBehaviour EngineBehaviour => null;
        public override IShieldBehaviour ShieldBehaviour => this;
        public override IMinerBehaviour MinerBehaviour => null;
        public override IThrusterBehaviour ThrusterBehaviour => null;
        public override IWeaponBehaviour WeaponBehaviour => null;
        
        
        public override void Use() { }
    }
}

using Interactions.Interfaces;
using TravelMind.Components.Core.Abstracts;
using TravelMind.Plugins.Interactions;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Player.Controllers
{
    public class PlayerShipController : MonoBehaviour
    {
        private const int MaxOverlappedColliders = 2;
        
        [SerializeField] private PlayerShip playerShip;

        private readonly Collider2D[] overlappedColliders = new Collider2D[MaxOverlappedColliders];

        private void OnEnable()
        {
            if (InteractionsManager.Instance) 
                InteractionsManager.Instance.OnTap += OnTap;
        }
        
        private void OnDisable()
        {
            if (InteractionsManager.Instance) 
                InteractionsManager.Instance.OnTap -= OnTap;
        }

        private void OnTap(InteractionsManager.TapData tapData)
        {
            var position = InteractionsManager.Instance.InteractionCamera.ScreenToWorldPoint(tapData.Position);

            var lenght = Physics2D.OverlapPointNonAlloc(position, overlappedColliders);
            for (var i = 0; i < lenght; i++)
            {
                var go = overlappedColliders[i].gameObject;
                var goLayer = go.layer;
                
                if(goLayer == Layers.PlayerLayer)
                {
                    if (go.TryGetComponent<AComponent>(out var component))
                        component.Use();
                }
                else if (goLayer == Layers.DebrisLayer)
                {
                    if (go.TryGetComponent<IMinable>(out var minable)) 
                        playerShip.SetMinableTarget(minable);
                }
                else if (goLayer == Layers.EnemiesLayer)
                {
                    if(go.TryGetComponent<IAttackable>(out var attackable))
                        playerShip.SetAttackableTarget(attackable);
                }   
            }
        }
    }
}

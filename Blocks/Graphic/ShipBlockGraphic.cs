using DG.Tweening;
using TravelMind.Blocks.Core;
using UnityEngine;

namespace TravelMind.Blocks.Graphic
{
    public class ShipBlockGraphic : MonoBehaviour
    {
        [SerializeField] private ShipBlock shipBlock;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color attackedColor;
        [SerializeField] [Min(0)] private float animationDuration = 0.05f;
        [SerializeField] [Min(0)] private int animationLoops = 2;

        private Color initialColor;

        private void Awake()
        {
            initialColor = spriteRenderer.color;
        }

        private void OnEnable()
        {
            shipBlock.OnAttacked += OnAttacked;
            shipBlock.OnDepleted += OnDepleted;
        }

        private void OnDisable()
        {
            shipBlock.OnAttacked -= OnAttacked;
            shipBlock.OnDepleted -= OnDepleted;
        }

        private void OnAttacked(ShipBlock _, float damages)
        {
            DOVirtual.Color(initialColor, attackedColor, animationDuration, x => spriteRenderer.color = x)
                .onComplete += () => DOVirtual.Color(attackedColor, initialColor, animationDuration, x => spriteRenderer.color = x)
                .SetLoops(animationLoops);
        }
        
        private void OnDepleted(ShipBlock _) { }
    }
}

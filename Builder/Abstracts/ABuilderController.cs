using System;
using System.Collections.Generic;
using System.Linq;
using TravelMind.Plugins.Interactions;
using UnityEngine;

namespace TravelMind.Builder.Abstracts
{
    public abstract class ABuilderController : MonoBehaviour
    {
        public event Action<BuilderBlock> OnBlockSelected = delegate { };

        
        protected abstract IReadOnlyList<BuilderBlock> SpawnedPrefabList { get; }

        protected virtual void OnEnable()
        {
            if(InteractionsManager.Instance)
                InteractionsManager.Instance.OnTap += OnTap;
        }

        protected virtual void OnDisable()
        {
            if(InteractionsManager.Instance)
                InteractionsManager.Instance.OnTap -= OnTap;
        }

        private void OnTap(InteractionsManager.TapData tapData)
        {
            var position = InteractionsManager.Instance.InteractionCamera.ScreenToWorldPoint(tapData.Position);
            var overlappedCollider = Physics2D.OverlapPoint(position);

            if (!overlappedCollider || !overlappedCollider.TryGetComponent<BuilderBlock>(out var builderBlock) || !SpawnedPrefabList.Contains(builderBlock))
                return;
            
            OnBlockSelected(builderBlock);
        }
        
    }
}

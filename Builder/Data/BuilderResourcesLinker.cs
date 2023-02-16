using TravelMind.Blocks.Core;
using TravelMind.Components.Core;
using TravelMind.Shared;
using UnityEngine;

namespace TravelMind.Builder.Data
{
    //Save/Load data process need to be effective before the other scripts
    [DefaultExecutionOrder(-1)]
    public class BuilderResourcesLinker : MonoBehaviour
    {
        [SerializeField] private BuilderDataContainer enableBuildContainer;
        [SerializeField] private BlocksGridManager blocksGridManager;
        [SerializeField] private ComponentsGridManager componentsGridManager;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying || !blocksGridManager)
                return;

            Gizmos.color = Color.red;
           Gizmos.DrawWireCube(blocksGridManager.SpawnParent.position, Vector3.one * Ship.SnapDistance);
        }
#endif

        private void OnEnable() => LoadFromContainer(enableBuildContainer);

        public void LoadFromContainer(BuilderDataContainer builderDataContainer)
        {
            if(!builderDataContainer)
                return;
            
            var data = builderDataContainer.LoadData();
            blocksGridManager.Initialize(data.Blocks);
            componentsGridManager.Initialize(data.Components);
        }
    }
}

using TravelMind.Builder;
using TravelMind.Builder.Controllers;
using TravelMind.Plugins.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TravelMind.UI
{
    public class StationWindow : Window
    {
      
        [SerializeField] private Toggle componentsBuilderManagementToggle;
        [SerializeField] private ComponentsBuilderController componentsBuilderController;
        [SerializeField] private ComponentBuilderWindow componentBuilderWindow;

        private ComponentsBuilderController currentComponentsBuilderController;
        
        private void OnEnable()
        {
            componentsBuilderManagementToggle.isOn = false;
            componentsBuilderManagementToggle.onValueChanged.AddListener(OnManageExtensionToolSelected);
            componentsBuilderController.OnBlockSelected += OnBlockSelected;
        }

        private void OnDisable()
        {
            componentsBuilderController.OnBlockSelected -= OnBlockSelected;
            componentsBuilderManagementToggle.onValueChanged.RemoveListener(OnManageExtensionToolSelected);
        }

        private void OnManageExtensionToolSelected(bool value)
        {
            componentsBuilderController.enabled = value;
        }

        private void OnBlockSelected(BuilderBlock previewBlock)
        {
            componentBuilderWindow.Open(componentsBuilderController, previewBlock);
        }
    }
}

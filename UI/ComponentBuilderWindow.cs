using TravelMind.Builder;
using TravelMind.Builder.Controllers;
using TravelMind.Plugins.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TravelMind.UI
{
    public class ComponentBuilderWindow : Window
    {
        [SerializeField] private Button engineButton;
        [SerializeField] private Button thrusterButton;
        [SerializeField] private Button shieldButton;
        [SerializeField] private Button weaponButton;
        [SerializeField] private Button deleteButton;

        private ComponentsBuilderController currentComponentsBuilderController;
        private BuilderBlock currentPreviewBlock;

        private void OnEnable()
        {
            engineButton.onClick.AddListener(OnEngineButtonClicked);
            thrusterButton.onClick.AddListener(OnThrusterButtonClicked);
            shieldButton.onClick.AddListener(OnShieldButtonClicked);
            weaponButton.onClick.AddListener(OnWeaponButtonClicked);
            
            deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        }

        private void OnDisable()
        {
            deleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
            
            weaponButton.onClick.RemoveListener(OnWeaponButtonClicked);
            shieldButton.onClick.RemoveListener(OnShieldButtonClicked);
            thrusterButton.onClick.RemoveListener(OnThrusterButtonClicked);
            engineButton.onClick.RemoveListener(OnEngineButtonClicked);

            currentComponentsBuilderController = null;
            currentPreviewBlock = null;
        }

        //TODO : Integrate System
        private void OnEngineButtonClicked()
        {
            currentComponentsBuilderController.AddComponent(currentPreviewBlock, "Builder_Component_Engine_A");
            Close();
        }

        private void OnThrusterButtonClicked()
        {
            currentComponentsBuilderController.AddComponent(currentPreviewBlock, "Builder_Component_Thruster_A");
            Close();
        }

        private void OnShieldButtonClicked()
        {
            currentComponentsBuilderController.AddComponent(currentPreviewBlock, "Builder_Component_Shield_A");
            Close();
        }

        private void OnWeaponButtonClicked()
        {
            currentComponentsBuilderController.AddComponent(currentPreviewBlock, "Builder_Component_Weapon_A");
            Close();
        }

        private void OnDeleteButtonClicked()
        {
            currentComponentsBuilderController.RemoveComponent(currentPreviewBlock);
            Close();
        }


        public void Open(ComponentsBuilderController componentsBuilderController, BuilderBlock previewBlock)
        {
            if(componentsBuilderController == null || previewBlock == null)
                return;

            currentComponentsBuilderController = componentsBuilderController;
            currentPreviewBlock = previewBlock;

            var componentExists = currentComponentsBuilderController
                .ComponentsGridManager.GetAtCoordinates(currentPreviewBlock.Coordinates) != null;
            
            deleteButton.gameObject.SetActive(componentExists);
            
            Open();
        }
    }
}

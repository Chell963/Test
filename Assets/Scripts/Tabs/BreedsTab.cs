using System;
using System.Collections.Generic;
using Models;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Tabs
{
    public class BreedsTab : Tab
    {
        public event Action<string> OnBreedAttributesRequested;
        
        [SerializeField] private List<BreedButton> breedButtons;
        [SerializeField] private BreedInfoPopup    breedPopup;
        [SerializeField] private Image loadingImage;

        public void UpdateBreeds(List<BreedModel> breedModels)
        {
            loadingImage.gameObject.SetActive(false);
            for (int index = 0; index < breedButtons.Count; index++)
            {
                var breedButton = breedButtons[index];
                if (index < breedModels.Count)
                {
                    var breedModel = breedModels[index];
                    breedButton.InitializeBreedButton(index, breedModel.Attributes.Name, breedModel.Id);
                    breedButton.gameObject.SetActive(true);
                    breedButton.OnButtonClicked += OnBreedButtonClicked;
                    continue;
                }
                breedButton.gameObject.SetActive(false);
            }
        }
        
        public void UpdateAttributes(BreedAttributesModel breedAttributesModel)
        {
            var description = breedAttributesModel.Description;
            var life = $"Life: min - {breedAttributesModel.Life["min"]}, max - {breedAttributesModel.Life["max"]}";
            var maleWeight = $"Male Weight: min - {breedAttributesModel.MaleWeight["min"]}, max - {breedAttributesModel.MaleWeight["max"]}";
            var femaleWeight = $"Female Weight: min - {breedAttributesModel.FemaleWeight["min"]}, max - {breedAttributesModel.FemaleWeight["max"]}";
            var hypoallergenic = breedAttributesModel.Hypoallergenic;
            breedPopup.OpenPopup(breedAttributesModel.Name, description,
                life, maleWeight, femaleWeight, hypoallergenic.ToString());
            var breedButton = breedButtons.Find(button => button.BreedName == breedAttributesModel.Name);
            breedButton.SwitchLoading(false);
        }

        public override void OpenTab()
        {
            base.OpenTab();
            breedButtons.ForEach(button => button.gameObject.SetActive(false));
            loadingImage.gameObject.SetActive(true);
            Debug.Log("BreedsTab opened!");
        }

        public override void CloseTab()
        {
            breedButtons.ForEach(button => button.OnButtonClicked -= OnBreedButtonClicked);
            base.CloseTab();
        }

        private void OnBreedButtonClicked(string breedId)
        {
            OnBreedAttributesRequested?.Invoke(breedId);
        }

        private async void Start()
        {
            
        }
    }
}

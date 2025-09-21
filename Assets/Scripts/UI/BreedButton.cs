using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BreedButton : MonoBehaviour
    {
        public event Action<string> OnButtonClicked;

        public string BreedName { get; private set;  }
        public string BreedId { get; private set; }

        [SerializeField] private Button          breedButton;
        [SerializeField] private TextMeshProUGUI breedText;
        [SerializeField] private Image           loadingImage;

        private int _number;

        public void InitializeBreedButton(int index, string breedName, string breedId)
        {
            BreedName = breedName;
            BreedId = breedId;
            _number = index + 1;
            breedText.text = $"{_number} - {breedName}";
        }

        public void SwitchLoading(bool isLoading)
        {
            loadingImage.gameObject.SetActive(isLoading);
        }

        private void BreedButtonClicked()
        {
            OnButtonClicked?.Invoke(BreedId);
            SwitchLoading(true);
        }

        private void Start()
        {
            breedButton.onClick.AddListener(BreedButtonClicked);
            SwitchLoading(false);
        }
    }
}

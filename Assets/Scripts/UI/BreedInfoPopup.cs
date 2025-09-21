using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BreedInfoPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI breedTitle;
        [SerializeField] private TextMeshProUGUI breedDescription;
        [SerializeField] private Button closeButton;

        public void OpenPopup(string title, string description, string life, 
            string maleWight, string femaleWeigh, string hypoallergenic)
        {
            gameObject.SetActive(true);
            breedTitle.text = title;
            breedDescription.text = $"1 - {description}\n" +
                                    $"2 - {life}\n" +
                                    $"3 - {maleWight}\n" +
                                    $"4 - {femaleWeigh}\n" +
                                    $"5 - Is Hypoallergenic: {hypoallergenic}\n";
        }

        private void ClosePopup()
        {
            gameObject.SetActive(false);
        }

        private void Start()
        {
            closeButton.onClick.AddListener(ClosePopup);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(ClosePopup);
        }
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Tabs
{
    public class WeatherTab : Tab
    {
        [SerializeField] private TextMeshProUGUI weatherText;
        [SerializeField] private Image weatherImage;

        public void UpdateWeather(string temperature, Sprite weatherSprite)
        {
            weatherText.text = $"Сегодня - {temperature}F";
            weatherImage.sprite = weatherSprite;
        }

        public override void OpenTab()
        {
            base.OpenTab();
            Debug.Log("WeatherTab opened!");
        }
    }
}

using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tabs
{
    public class ClickerTab : Tab
    {
        public event Action OnClickerButtonClicked; 

        [SerializeField] private TextMeshProUGUI currencyText;
        [SerializeField] private TextMeshProUGUI energyText;
        
        [SerializeField] private Button         clickerButton;
        [SerializeField] private ParticleSystem clickerParticleSystem;
        [SerializeField] private AudioSource    clickerAudioSource;
        [SerializeField] private Animation      clickerAnimation;
        
        [SerializeField] private float buttonClickedDuration = 0.15f;
        [SerializeField] private float particleLifetime = 1f;

        private bool _isDisposed;

        public void UpdateCurrency(int value, bool isClicked)
        {
            currencyText.text = value.ToString();
            SimulateClickVFX(isClicked);
        }
        
        public void UpdateEnergy(int energy)
        {
            energyText.text = energy.ToString();
            clickerButton.interactable = energy != 0;
        }
        
        public override void OpenTab()
        {
            base.OpenTab();
            clickerAudioSource.mute = false;
            Debug.Log("ClickerTab opened!");
        }

        public override void CloseTab()
        {
            base.CloseTab();
            clickerAudioSource.mute = true;
        }
        
        private void ClickerButtonClicked()
        {
            OnClickerButtonClicked?.Invoke();
        }

        private async void SimulateButtonClick()
        {
            ExecuteEvents.Execute(clickerButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            await Task.Delay(TimeSpan.FromSeconds(buttonClickedDuration));
            if (_isDisposed) return;
            ExecuteEvents.Execute(clickerButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
        }

        //В UIParticleSystem баг с отсутствием отображения частиц без флага loop, поэтому частицы переключаются вручную
        private async void SimulateClickVFX(bool isClicked)
        {
            if (!isClicked)
            {
                SimulateButtonClick();
            }
            clickerAudioSource.Play();
            clickerAnimation.Play();
            clickerParticleSystem.Play();
            await Task.Delay(TimeSpan.FromSeconds(particleLifetime));
            if (_isDisposed) return;
            clickerParticleSystem.Stop();
        }

        private void Start()
        {
            clickerButton.onClick.AddListener(ClickerButtonClicked);
        }

        private void OnDestroy()
        {
            _isDisposed = true;
        }
    }
}

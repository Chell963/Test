using System;
using System.Collections.Generic;
using Models;
using Tabs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MVP
{
    public class TabsView : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private List<Tab> tabs;

        [SerializeField] private Button prevButton;
        [SerializeField] private Button nextButton;

        private TabsPresenter _tabsPresenter;

        private int _tabIndex = 0;

        public void Initialize()
        {
            prevButton.onClick.AddListener(SwitchToPrevTab);
            nextButton.onClick.AddListener(SwitchToNextTabTab);

            _tabsPresenter.OnEnergyPassed += UpdateClickerEnergy;
            _tabsPresenter.OnCurrencyPassed += UpdateClickerCurrency;
            _tabsPresenter.OnWeatherPassed += UpdateWeatherInfo;
            _tabsPresenter.OnBreedsPassed += UpdateBreedsInfo;
            _tabsPresenter.OnAttributesPassed += UpdateAttributesInfo;
            
            GetTab<ClickerTab>().OnClickerButtonClicked += _tabsPresenter.ClickerButtonClicked;
            
            GetTab<WeatherTab>().OnTabOpened += _tabsPresenter.WeatherTabOpened;
            GetTab<WeatherTab>().OnTabClosed += _tabsPresenter.WeatherTabClosed;

            GetTab<BreedsTab>().OnTabOpened += _tabsPresenter.BreedsTabOpened;
            GetTab<BreedsTab>().OnTabClosed += _tabsPresenter.BreedsTabClosed;
            GetTab<BreedsTab>().OnTabClosed += _tabsPresenter.AttributesPopupClosed;

            GetTab<BreedsTab>().OnBreedAttributesRequested += _tabsPresenter.AttributesPopupOpened;
            
            _tabsPresenter.Initialize();
            
            foreach (var tab in tabs)
            {
                tab.gameObject.SetActive(true);
            }
            
            CheckTabIndexLimits();
        }
        
        public void Dispose()
        {
            _tabsPresenter.OnEnergyPassed -= UpdateClickerEnergy;
            _tabsPresenter.OnCurrencyPassed -= UpdateClickerCurrency;
            _tabsPresenter.OnWeatherPassed -= UpdateWeatherInfo;
            _tabsPresenter.OnBreedsPassed -= UpdateBreedsInfo;
            _tabsPresenter.OnAttributesPassed -= UpdateAttributesInfo;
            
            GetTab<ClickerTab>().OnClickerButtonClicked -= _tabsPresenter.ClickerButtonClicked;

            GetTab<WeatherTab>().OnTabOpened -= _tabsPresenter.WeatherTabOpened;
            GetTab<WeatherTab>().OnTabClosed -= _tabsPresenter.WeatherTabClosed;
            
            GetTab<BreedsTab>().OnTabOpened -= _tabsPresenter.BreedsTabOpened;
            GetTab<BreedsTab>().OnTabClosed -= _tabsPresenter.BreedsTabClosed;
            GetTab<BreedsTab>().OnTabClosed -= _tabsPresenter.AttributesPopupClosed;
            
            GetTab<BreedsTab>().OnBreedAttributesRequested -= _tabsPresenter.AttributesPopupOpened;
            
            _tabsPresenter.Dispose();
        }

        private T GetTab<T>() where T : Tab
        {
            return (T)tabs.Find(tab => tab is T);
        }

        private void UpdateClickerCurrency(int currency, bool isClicked)
        {
            GetTab<ClickerTab>().UpdateCurrency(currency, isClicked);
        }
        
        private void UpdateClickerEnergy(int energy)
        {
            GetTab<ClickerTab>().UpdateEnergy(energy);
        }
        
        private void UpdateWeatherInfo(string temperature, Sprite weatherSprite)
        {
            GetTab<WeatherTab>().UpdateWeather(temperature, weatherSprite);
        }
        
        private void UpdateBreedsInfo(List<BreedModel> breedModels)
        {
            GetTab<BreedsTab>().UpdateBreeds(breedModels);
        }
        
        private void UpdateAttributesInfo(BreedAttributesModel breedAttributesModel)
        {
            GetTab<BreedsTab>().UpdateAttributes(breedAttributesModel);
        }

        private void SwitchToPrevTab()
        {
            if (_tabIndex > 0)
            {
                tabs[_tabIndex].CloseTab();
                _tabIndex--;
                tabs[_tabIndex].OpenTab();
            }
            CheckTabIndexLimits();
        }
        
        private void SwitchToNextTabTab()
        {
            if (_tabIndex < tabs.Count - 1)
            {
                tabs[_tabIndex].CloseTab();
                _tabIndex++;
                tabs[_tabIndex].OpenTab();
            }
            CheckTabIndexLimits();
        }

        private void CheckTabIndexLimits()
        {
            var isFirstTab = _tabIndex == 0;
            var isLastTab = _tabIndex == tabs.Count - 1;
            prevButton.interactable = true;
            nextButton.interactable = true;
            if (isFirstTab)
            {
                prevButton.interactable = false;
            }
            if (isLastTab)
            {
                nextButton.interactable = false;
            }
            var curTab = tabs[_tabIndex];
            curTab.transform.SetAsLastSibling();
        }

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Dispose();
        }
        
        [Inject]
        private void Construct(TabsPresenter tabsPresenter)
        {
            _tabsPresenter = tabsPresenter;
        }
    }
}

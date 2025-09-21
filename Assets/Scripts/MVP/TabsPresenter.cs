using System;
using System.Collections.Generic;
using Models;
using UnityEngine;
using Zenject;

namespace MVP
{
    public class TabsPresenter : IInitializable, IDisposable
    {
        public event Action<int> OnEnergyPassed; 
        public event Action<int, bool> OnCurrencyPassed;
        public event Action<string, Sprite> OnWeatherPassed;
        public event Action<List<BreedModel>> OnBreedsPassed;
        public event Action<BreedAttributesModel> OnAttributesPassed;

        public event Action OnWeatherRequested;
        public event Action OnWeatherCancelled;
        
        public event Action OnBreedsRequested;
        public event Action OnBreedsCancelled;
        
        public event Action<string> OnAttributeRequested;
        public event Action OnAttributeCancelled;

        private TabsModel _tabsModel;
        
        public void Initialize()
        {
            _tabsModel.OnCurrencyChanged += PassClickerCurrency;
            _tabsModel.OnEnergyChanged += PassClickerEnergy;
            _tabsModel.OnWeatherChanged += PassWeatherInfo;
            _tabsModel.OnBreedsChanged += PassBreadsInfo;
            _tabsModel.OnAttributesChanged += PassAttributesInfo;

            OnWeatherRequested += _tabsModel.EnqueueWeather;
            OnWeatherCancelled += _tabsModel.DequeueWeather;

            OnBreedsRequested += _tabsModel.EnqueueBreeds;
            OnBreedsCancelled += _tabsModel.DequeueBreeds;

            OnAttributeRequested += _tabsModel.EnqueueAttributes;
            OnAttributeCancelled += _tabsModel.DequeueAttributes;
            
            _tabsModel.Initialize();
        }

        public void Dispose()
        {
            _tabsModel.OnCurrencyChanged -= PassClickerCurrency;
            _tabsModel.OnEnergyChanged -= PassClickerEnergy;
            _tabsModel.OnWeatherChanged -= PassWeatherInfo;
            _tabsModel.OnBreedsChanged -= PassBreadsInfo;
            _tabsModel.OnAttributesChanged -= PassAttributesInfo;

            OnWeatherRequested -= _tabsModel.EnqueueWeather;
            OnWeatherCancelled -= _tabsModel.DequeueWeather;
            
            OnBreedsRequested -= _tabsModel.EnqueueBreeds;
            OnBreedsCancelled -= _tabsModel.DequeueBreeds;

            OnAttributeRequested -= _tabsModel.EnqueueAttributes;
            OnAttributeCancelled -= _tabsModel.DequeueAttributes;
            
            _tabsModel.Dispose();
        }

        public void ClickerButtonClicked()
        {
            _tabsModel.GetCurrency(true);
        }
        
        public void WeatherTabOpened()
        {
            OnWeatherRequested?.Invoke();
        }

        public void WeatherTabClosed()
        {
            OnWeatherCancelled?.Invoke();
        }
        
        public void BreedsTabOpened()
        {
            OnBreedsRequested?.Invoke();
        }

        public void BreedsTabClosed()
        {
            OnBreedsCancelled?.Invoke();
        }
        
        public void AttributesPopupOpened(string breedId)
        {
            OnAttributeRequested?.Invoke(breedId);
        }

        public void AttributesPopupClosed()
        {
            OnAttributeCancelled?.Invoke();
        }

        private void PassClickerEnergy(int energy)
        {
            OnEnergyPassed?.Invoke(energy);
        }

        private void PassClickerCurrency(int value, bool isClicked)
        {
            OnCurrencyPassed?.Invoke(value, isClicked);
        }

        private void PassWeatherInfo(string temperature, Sprite weatherSprite)
        {
            OnWeatherPassed?.Invoke(temperature, weatherSprite);
        }

        private void PassBreadsInfo(List<BreedModel> breedModels)
        {
            OnBreedsPassed?.Invoke(breedModels);
        }
        
        private void PassAttributesInfo(BreedAttributesModel breedAttributesModel)
        {
            OnAttributesPassed?.Invoke(breedAttributesModel);
        }
        
        [Inject]
        private void Construct(TabsModel tabsModel)
        {
            _tabsModel = tabsModel;
        }
    }
}

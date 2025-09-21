using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace MVP
{
    public class TabsModel : IInitializable, IDisposable
    {
        public event Action<int> OnEnergyChanged;
        public event Action<int, bool> OnCurrencyChanged;

        public event Action<string, Sprite> OnWeatherChanged;
        public event Action<List<BreedModel>> OnBreedsChanged;
        public event Action<BreedAttributesModel> OnAttributesChanged; 

        private event Action<UnityWebRequest> OnRequestEnqueued; 

        public int EnergyRegen { get; private set; }
        public int EnergyCooldown { get; private set; }
        public int MaxEnergy { get; private set; }
        public int EnergyCost { get; private set; }
        public int CurrencyAward { get; private set; }
        public int CurrencyCooldown { get; private set; }
        public int Energy { get; private set; }
        public int Currency { get; private set; }

        private ClickerData _clickerData;

        private int _weatherTimer = 5;

        private CancellationTokenSource _clickerDisposeToken = new CancellationTokenSource();
        private CancellationTokenSource _weatherDisposeToken = new CancellationTokenSource();
        
        private const string BreedsUrl = "https://dogapi.dog/api/v2/breeds";
        private const string WeatherUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";

        private Queue<UnityWebRequest> _requestQueue = new Queue<UnityWebRequest>();
        private bool _isProcessing;

        public void Initialize()
        {
            EnergyRegen = _clickerData.EnergyRegen;
            EnergyCooldown = _clickerData.EnergyCooldown;
            MaxEnergy = _clickerData.MaxEnergy;
            EnergyCost = _clickerData.EnergyCost;
            CurrencyAward = _clickerData.ValueAward;
            CurrencyCooldown = _clickerData.ValueCooldown;
            
            Currency = 0;
            Energy = MaxEnergy;
            
            OnRequestEnqueued += ProcessQueue;
            
            ManageClickerEnergy();
            ManageClickerCurrency();
            
            OnEnergyChanged?.Invoke(MaxEnergy);
        }
        
        public void Dispose()
        {
            OnRequestEnqueued -= ProcessQueue;
            _clickerDisposeToken?.Cancel();
            _clickerDisposeToken?.Dispose();
        }

        public void GetCurrency(bool isClicked)
        {
            Currency += CurrencyAward;
            OnCurrencyChanged?.Invoke(Currency, isClicked);
            SpendEnergy();
        }

        public async void EnqueueWeather()
        {
            _weatherDisposeToken = new CancellationTokenSource();
            while (!_weatherDisposeToken.IsCancellationRequested)
            {
                var weatherRequest = UnityWebRequest.Get(WeatherUrl);
                OnRequestEnqueued?.Invoke(weatherRequest);
                while (!weatherRequest.isDone)
                {
                    await Task.Yield();
                }
                if (weatherRequest.result != UnityWebRequest.Result.Success) return;
                string jsonResponse = weatherRequest.downloadHandler.text;
                var j = JObject.Parse(jsonResponse);
                var periodsValue = (JArray)j["properties"]["periods"];
                if (periodsValue != null)
                {
                    var todayValue = periodsValue[0];
                    var todayWeather = JsonConvert.DeserializeObject<WeatherModel>(todayValue.ToString());
                    var weatherIconUrl = todayWeather.Icon;
                    var textureRequest = UnityWebRequestTexture.GetTexture(weatherIconUrl);
                    OnRequestEnqueued?.Invoke(textureRequest);
                    while (!textureRequest.isDone)
                    {
                        await Task.Yield();
                    }
                    if (textureRequest.result != UnityWebRequest.Result.Success) return;
                    var texture = DownloadHandlerTexture.GetContent(textureRequest);
                    var rect = new Rect(0, 0, 86f, 86f);
                    var sprite = Sprite.Create(texture, rect,new Vector2(0.5f,0.5f));
                    OnWeatherChanged?.Invoke(todayWeather.Temperature, sprite);
                }
                
                var weatherTimer = (float)_weatherTimer;
                while (weatherTimer > 0)
                {
                    weatherTimer -= Time.deltaTime;
                    await Task.Yield();
                }

                if (_weatherDisposeToken.IsCancellationRequested)
                {
                    _weatherDisposeToken?.Dispose();
                    return;
                }
                
                Debug.Log("weather timer passed 5");
            }
        }
        
        public void DequeueWeather()
        {
            _weatherDisposeToken?.Cancel();
            _weatherDisposeToken?.Dispose();
            //Реализовать удаление из очереди
        }

        public async void EnqueueBreeds()
        {
            var breeds = UnityWebRequest.Get(BreedsUrl);
            OnRequestEnqueued?.Invoke(breeds);
            while (!breeds.isDone)
            {
                await Task.Yield();
            }
            string jsonBreeds = breeds.downloadHandler.text;
            var jBreeds = JObject.Parse(jsonBreeds);
            var breedsValue = (JArray)jBreeds["data"];
            if (breedsValue != null)
            {
                var breedModels = JsonConvert.DeserializeObject<List<BreedModel>>(breedsValue.ToString());
                OnBreedsChanged?.Invoke(breedModels);
            }
        }
        
        public void DequeueBreeds()
        {
            //Реализовать удаление из очереди
        }
        
        public async void EnqueueAttributes(string breedId)
        {
            var attributesUrl = $"{BreedsUrl}/{breedId}";
            var attributes = UnityWebRequest.Get(attributesUrl);
            OnRequestEnqueued?.Invoke(attributes);
            while (!attributes.isDone)
            {
                await Task.Yield();
            }
            string jsonAttributes = attributes.downloadHandler.text;
            var jAttributes = JObject.Parse(jsonAttributes);
            var attributesValue = jAttributes["data"]?["attributes"];
            if (attributesValue != null)
            {
                var attributesDeserialized = JsonConvert.DeserializeObject<BreedAttributesModel>(attributesValue.ToString());
                OnAttributesChanged?.Invoke(attributesDeserialized);
            }
        }
        
        public void DequeueAttributes()
        {
            //Реализовать удаление из очереди
        }

        private async void ProcessQueue(UnityWebRequest webRequest)
        {
            _requestQueue.Enqueue(webRequest);
            if (_isProcessing) return;
            _isProcessing = true;
            Debug.Log("Queue processing started!");
            while (_requestQueue.Count > 0)
            {
                var request = _requestQueue.Dequeue();
                var requestTask = request.SendWebRequest();
                while (!requestTask.isDone)
                {
                    await Task.Yield();
                }
            }
            _isProcessing = false;
            Debug.Log("Queue processing finished!");
        }

        private async void ManageClickerEnergy()
        {
            while (!_clickerDisposeToken.IsCancellationRequested)
            {
                if (Energy >= MaxEnergy)
                {
                    Energy = MaxEnergy;
                    await Task.Yield();
                    continue;
                }
                
                var energyTimer = (float)EnergyCooldown;
                while (energyTimer > 0)
                {
                    energyTimer -= Time.deltaTime;
                    await Task.Yield();
                }
                if (_clickerDisposeToken.IsCancellationRequested) return;
                GetEnergy();
            }
        }
        
        private async void ManageClickerCurrency()
        {
            while (!_clickerDisposeToken.IsCancellationRequested)
            {
                var currencyTimer = (float)CurrencyCooldown;
                while (currencyTimer > 0)
                {
                    if (Energy != 0)
                    {
                        currencyTimer -= Time.deltaTime;
                    }
                    await Task.Yield();
                }
                if (_clickerDisposeToken.IsCancellationRequested) return;
                GetCurrency(false);
            }
        }
        
        private void GetEnergy()
        {
            Energy = Energy + EnergyRegen >= MaxEnergy ?  MaxEnergy : Energy + EnergyRegen;
            OnEnergyChanged?.Invoke(Energy);
        }
        
        private void SpendEnergy()
        {
            Energy = Energy - EnergyCost >= 0 ? Energy - EnergyCost : 0;
            OnEnergyChanged?.Invoke(Energy);
        }

        [Inject]
        private void Construct(ClickerData clickerData)
        {
            _clickerData = clickerData;
        }
    }
}

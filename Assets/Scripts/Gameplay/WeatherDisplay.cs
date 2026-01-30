using Logbound.Data;
using Logbound.Services;
using Logbound.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Logbound.Gameplay
{
    public class WeatherDisplay : MonoBehaviour
    {
        [SerializeField] private WeatherIconsData _weatherIconsData;
        [SerializeField] private TMPro.TextMeshProUGUI _weatherStateText;
        [SerializeField] private TMPro.TextMeshProUGUI _temperatureText;
        [SerializeField] private Image _weatherIcon;
        [SerializeField] private WeatherUtility.WeatherTimeState _weatherTimeState;
        [SerializeField] private bool _displayInCelsius = true;
        
        private bool _isInitialized = false;

        private void Start()
        {
            _isInitialized = ForecastService.Instance != null;
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            var currentWeatherState = ForecastService.Instance.GetWeatherState(_weatherTimeState);
            float currentTemperature = ForecastService.Instance.GetTemperature(_weatherTimeState);

            _weatherStateText.text = currentWeatherState.ToString();
            _temperatureText.text = WeatherUtility.GetTemperatureString(currentTemperature, _displayInCelsius);
            _weatherIcon.sprite = _weatherIconsData.GetIcon(currentWeatherState);
        }
    }
}

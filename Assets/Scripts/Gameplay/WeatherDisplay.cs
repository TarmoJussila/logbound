using System;
using Logbound.Data;
using Logbound.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Logbound.Gameplay
{
    public class WeatherDisplay : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _weatherStateText;
        [SerializeField] private TMPro.TextMeshProUGUI _temperatureText;
        [SerializeField] private Image _weatherIcon;
        [SerializeField] private bool _displayInCelsius = true;
        
        private bool _isInitialized = false;

        private void Start()
        {
            _isInitialized = WeatherTransitionController.Instance != null;
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            WeatherState currentWeatherState = WeatherTransitionController.Instance.GetCurrentWeatherState();
            float currentTemperature = WeatherTransitionController.Instance.GetCurrentTemperature();

            _weatherStateText.text = currentWeatherState.ToString();
            _temperatureText.text = WeatherUtility.GetTemperatureString(currentTemperature, _displayInCelsius);
            
            switch (currentWeatherState)
            {
                case WeatherState.Clear:
                    _weatherIcon.color = Color.yellow;
                    break;
                case WeatherState.Cloudy:
                    _weatherIcon.color = Color.gray;
                    break;
                case WeatherState.Rain:
                    _weatherIcon.color = Color.blue;
                    break;
                case WeatherState.Snowfall:
                    _weatherIcon.color = Color.white;
                    break;
                case WeatherState.Thunderstorm:
                    _weatherIcon.color = Color.magenta;
                    break;
                case WeatherState.Fog:
                    _weatherIcon.color = Color.green;
                    break;
                default:
                    _weatherIcon.color = Color.clear;
                    break;
            }
        }
    }
}

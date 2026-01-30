using Logbound.Data;
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
            
            var currentWeatherState = WeatherTransitionController.Instance.GetCurrentWeatherState();
            float currentTemperature = WeatherTransitionController.Instance.GetCurrentTemperature();

            _weatherStateText.text = currentWeatherState.ToString();
            _temperatureText.text = WeatherUtility.GetTemperatureString(currentTemperature, _displayInCelsius);
            _weatherIcon.sprite = _weatherIconsData.GetIcon(currentWeatherState);
        }
    }
}

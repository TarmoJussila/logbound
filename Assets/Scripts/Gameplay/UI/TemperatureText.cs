using Logbound.Utilities;
using TMPro;
using UnityEngine;

namespace Logbound.Gameplay.UI
{
    public class TemperatureText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _temperatureText;
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

            float currentTemperature = WeatherTransitionController.Instance.GetCurrentTemperature();
            _temperatureText.text = WeatherUtility.GetTemperatureString(currentTemperature, _displayInCelsius);
        }
    }
}

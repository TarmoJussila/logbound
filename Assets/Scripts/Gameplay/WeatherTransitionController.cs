using Logbound.Data;
using Logbound.Services;
using UnityEngine;

namespace Logbound.Gameplay
{
    public class WeatherTransitionController : MonoBehaviour
    {
        [SerializeField] private WeatherState _targetWeatherState;
        [SerializeField] private float _targetTemperature;
        [SerializeField] private float _weatherTransitionDuration = 5f;
        [SerializeField] private float _temperatureTransitionDuration = 10f;

        private WeatherState _initialWeatherState;
        private float _initialTemperature;
        private float _transitionProgressWeather;
        private float _transitionProgressTemperature;
        private bool _isTransitioningWeather;
        private bool _isTransitioningTemperature;

        private void Awake()
        {
            _initialWeatherState = WeatherService.Instance.GetWeatherState();
        }

        private void OnEnable()
        {
            WeatherService.Instance.OnTemperatureChanged += StartTemperatureTransition;
            WeatherService.Instance.OnWeatherChanged += StartWeatherTransition;
        }

        private void OnDestroy()
        {
            WeatherService.Instance.OnTemperatureChanged -= StartTemperatureTransition;
            WeatherService.Instance.OnWeatherChanged -= StartWeatherTransition;
        }

        private void Update()
        {
            if (_isTransitioningWeather)
            {
                _transitionProgressWeather += Time.deltaTime / _weatherTransitionDuration;
                if (_transitionProgressWeather >= 1f)
                {
                    _transitionProgressWeather = 1f;
                    _isTransitioningWeather = false;
                    _initialWeatherState = _targetWeatherState;
                }
                
                Debug.Log($"Transitioning from {_initialWeatherState} to {_targetWeatherState}: {_transitionProgressWeather * 100f}%");
            }
            
            if (_isTransitioningTemperature)
            {
                _transitionProgressTemperature += Time.deltaTime / _weatherTransitionDuration;
                if (_transitionProgressTemperature >= 1f)
                {
                    _transitionProgressTemperature = 1f;
                    _isTransitioningTemperature = false;
                    _initialTemperature = _targetTemperature;
                }
                
                Debug.Log($"Transitioning temperature from {_initialTemperature} to {_targetTemperature}: {_transitionProgressTemperature * 100f}%");
            }
        }
        
        private void StartWeatherTransition(WeatherState weatherState)
        {
            _targetWeatherState = weatherState;
            if (_initialWeatherState == _targetWeatherState)
            {
                Debug.LogWarning("Initial and target weather states are the same. No transition needed.");
                return;
            }
            
            _isTransitioningWeather = true;
            _transitionProgressWeather = 0f;
        }
        
        private void StartTemperatureTransition(float newTemperature)
        {
            _targetTemperature = newTemperature;
            if (Mathf.Approximately(_initialTemperature, _targetTemperature))
            {
                Debug.LogWarning("Initial and target temperatures are the same. No transition needed.");
                return;
            }
            
            _isTransitioningTemperature = true;
            _transitionProgressTemperature = 0f;
        }
    }
}

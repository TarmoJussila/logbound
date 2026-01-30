using System;
using Logbound.Data;
using Logbound.Utilities;
using UnityEngine;

namespace Logbound.Services
{
    public class WeatherService : Singleton<WeatherService>
    {
        public event Action<WeatherState> OnTargetWeatherStateChanged;
        public event Action<float> OnTargetTemperatureChanged;

        private WeatherState _targetWeatherState;
        private float _targetTemperature;

        public void SetTargetWeatherState(WeatherState state)
        {
            if (_targetWeatherState != state)
            {
                _targetWeatherState = state;
                OnTargetWeatherStateChanged?.Invoke(state);
            }
        }

        public WeatherState GetTargetWeatherState()
        {
            return _targetWeatherState;
        }

        public void SetTargetTemperature(float temperature)
        {
            if (!Mathf.Approximately(temperature, _targetTemperature))
            {
                _targetTemperature = temperature;
                OnTargetTemperatureChanged?.Invoke(temperature);
            }
        }
        
        public float GetTargetTemperature(bool inCelsius = true)
        {
            return WeatherUtility.GetTemperature(_targetTemperature, inCelsius);
        }
    }
}
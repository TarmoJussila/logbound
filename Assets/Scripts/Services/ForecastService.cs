using System;
using Logbound.Data;
using Logbound.Utilities;
using UnityEngine;

namespace Logbound.Services
{
    public class ForecastService : Singleton<ForecastService>
    {
        public event Action<WeatherState, float> OnForecastUpdated;
        
        [SerializeField] private float _forecastIntervalDuration = 60f;
        
        private WeatherState _previousTargetState;
        private WeatherState _currentTargetState;
        private WeatherState _nextTargetState;
        
        private float _previousTargetTemperature;
        private float _currentTargetTemperature;
        private float _nextTargetTemperature;
        
        private float _forecastTimer;

        protected void Start()
        {
            InitializeForecast
            (
                WeatherUtility.GetRandomWeatherState(),
                WeatherUtility.GetRandomWeatherState(),
                WeatherUtility.GetRandomWeatherState(),
                WeatherUtility.GetRandomTemperatureCelsius(),
                WeatherUtility.GetRandomTemperatureCelsius(),
                WeatherUtility.GetRandomTemperatureCelsius()
            );
        }

        private void Update()
        {
            _forecastTimer += Time.deltaTime;
            if (_forecastTimer >= _forecastIntervalDuration)
            {
                _forecastTimer = 0f;
                UpdateForecast(WeatherUtility.GetRandomWeatherState(), WeatherUtility.GetRandomTemperatureCelsius());
            }
        }

        private void InitializeForecast(WeatherState previousState, WeatherState currentState, WeatherState nextState,
            float previousTemperature, float currentTemperature, float nextTemperature)
        {
            _previousTargetState = previousState;
            _currentTargetState = currentState;
            _nextTargetState = nextState;
            
            _previousTargetTemperature = previousTemperature;
            _currentTargetTemperature = currentTemperature;
            _nextTargetTemperature = nextTemperature;
            
            OnForecastUpdated?.Invoke(_currentTargetState, _currentTargetTemperature);
        }
        
        private void UpdateForecast(WeatherState nextState, float nextTemperature)
        {
            _previousTargetState = _currentTargetState;
            _currentTargetState = _nextTargetState;
            _nextTargetState = nextState;
            
            _previousTargetTemperature = _currentTargetTemperature;
            _currentTargetTemperature = _nextTargetTemperature;
            _nextTargetTemperature = nextTemperature;
            
            OnForecastUpdated?.Invoke(_currentTargetState, _currentTargetTemperature);
        }
        
        public WeatherState GetWeatherState(WeatherUtility.WeatherTimeState weatherTimeState)
        {
            return weatherTimeState switch
            {
                WeatherUtility.WeatherTimeState.Previous => _previousTargetState,
                WeatherUtility.WeatherTimeState.Current => _currentTargetState,
                WeatherUtility.WeatherTimeState.Next => _nextTargetState,
                _ => _currentTargetState
            };
        }
        
        public float GetTemperature(WeatherUtility.WeatherTimeState weatherTimeState)
        {
            return weatherTimeState switch
            {
                WeatherUtility.WeatherTimeState.Previous => _previousTargetTemperature,
                WeatherUtility.WeatherTimeState.Current => _currentTargetTemperature,
                WeatherUtility.WeatherTimeState.Next => _nextTargetTemperature,
                _ => _currentTargetTemperature
            };
        }
        
        public float GetForecastProgress()
        {
            return _forecastTimer / _forecastIntervalDuration;
        }
    }
}

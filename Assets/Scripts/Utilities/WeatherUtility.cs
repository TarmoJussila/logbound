using Logbound.Data;
using UnityEngine;

namespace Logbound.Utilities
{
    public static class WeatherUtility
    {
        public enum WeatherTimeState
        {
            Previous = -1,
            Current = 0,
            Next = 1
        }
        
        public static float MinTemperatureCelsius = -50f;
        public static float MaxTemperatureCelsius = 10f;
        
        public static float GetRandomTemperatureCelsius()
        {
            return UnityEngine.Random.Range(MinTemperatureCelsius, MaxTemperatureCelsius);
        }
        
        public static WeatherState GetRandomWeatherState()
        {
            var values = System.Enum.GetValues(typeof(WeatherState));
            return (WeatherState)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        }
        
        public static float GetTemperature(float temperature, bool inCelsius = true)
        {
            if (inCelsius)
            {
                return temperature;
            }
            else
            {
                return (temperature * 9 / 5) + 32;
            }
        }
        
        public static string GetTemperatureString(float temperature, bool inCelsius = true, bool round = true)
        {
            if (inCelsius)
            {
                return GetTemperatureCelsiusString(temperature, round);
            }
            else
            {
                return GetTemperatureFahrenheitString(temperature, round);
            }
        }
        
        private static string GetTemperatureCelsiusString(float temperature, bool round = true)
        {
            var temp = GetTemperature(temperature, true);
            return round ? $"{Mathf.Round(temp)}°C" : $"{temp}°C";
        }
        
        private static string GetTemperatureFahrenheitString(float temperature, bool round = true)
        {
            var temp = GetTemperature(temperature, false);
            return round ? $"{Mathf.Round(temp)}°F" : $"{temp}°F";
        }
    }
}

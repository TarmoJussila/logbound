namespace Logbound.Utilities
{
    public static class WeatherUtility
    {
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
        
        public static string GetTemperatureString(float temperature, bool inCelsius = true)
        {
            if (inCelsius)
            {
                return GetTemperatureCelsiusString(temperature);
            }
            else
            {
                return GetTemperatureFahrenheitString(temperature);
            }
        }
        
        private static string GetTemperatureCelsiusString(float temperature)
        {
            return $"{GetTemperature(temperature, true)} °C";
        }
        
        private static string GetTemperatureFahrenheitString(float temperature)
        {
            return $"{GetTemperature(temperature, false)} °F";
        }
    }
}

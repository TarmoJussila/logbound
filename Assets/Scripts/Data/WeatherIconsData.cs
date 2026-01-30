using System;
using UnityEngine;

namespace Logbound.Data
{
    [CreateAssetMenu(fileName = "WeatherIconsData", menuName = "Logbound/Weather Icons Data")]
    public class WeatherIconsData : ScriptableObject
    {
        [Serializable]
        public class WeatherIcon
        {
            public WeatherState _weatherState;
            public Sprite _icon;
        }

        [SerializeField] private WeatherIcon[] _weatherIcons;

        public Sprite GetIcon(WeatherState state)
        {
            foreach (var weatherIcon in _weatherIcons)
            {
                if (weatherIcon._weatherState == state)
                {
                    return weatherIcon._icon;
                }
            }
            return null;
        }
    }
}


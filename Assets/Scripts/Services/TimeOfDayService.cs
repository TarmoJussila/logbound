using System;
using Logbound.Utilities;
using UnityEngine;

namespace Logbound.Services
{
    public class TimeOfDayService : Singleton<TimeOfDayService>
    {
        [Header("References")]
        [SerializeField] private Light _directionalLight;
        
        [Header("Day/Night Cycle Settings")]
        [SerializeField] private float _dayDurationInSeconds = 120f;
        [SerializeField] private float _nightDurationInSeconds = 60f;
        
        [Header("Day Settings")]
        [SerializeField] private Color _dayColor = new Color(1f, 0.95f, 0.84f);
        [SerializeField] private float _dayIntensity = 1f;
        
        [Header("Night Settings")]
        [SerializeField] private Color _nightColor = new Color(0.2f, 0.2f, 0.4f);
        [SerializeField] private float _nightIntensity = 0.2f;
        
        [Header("Current State")]
        [SerializeField] private float _currentTime; // 0-1 range, 0 = midnight, 0.5 = noon
        [SerializeField] private bool _isPaused;

        public event Action<float> OnTimeChanged;
        public event Action<bool> OnDayNightChanged;

        public float CurrentTime => _currentTime;
        public bool IsDay => _currentTime >= 0.25f && _currentTime < 0.75f;
        public bool IsPaused => _isPaused;

        private bool _wasDay;

        private void Start()
        {
            _wasDay = IsDay;
            UpdateLighting();
        }

        private void Update()
        {
            if (_isPaused) return;
            
            AdvanceTime();
            UpdateLighting();
        }

        private void AdvanceTime()
        {
            float cycleDuration = IsDay ? _dayDurationInSeconds : _nightDurationInSeconds;
            float timeIncrement = Time.deltaTime / (cycleDuration * 2f);
            
            _currentTime += timeIncrement;
            if (_currentTime >= 1f)
            {
                _currentTime -= 1f;
            }
            
            OnTimeChanged?.Invoke(_currentTime);
            
            if (_wasDay != IsDay)
            {
                _wasDay = IsDay;
                OnDayNightChanged?.Invoke(IsDay);
            }
        }

        private void UpdateLighting()
        {
            if (_directionalLight == null) return;
            
            // Calculate blend factor (0 = full night, 1 = full day)
            float blendFactor = CalculateBlendFactor();
            
            _directionalLight.color = Color.Lerp(_nightColor, _dayColor, blendFactor);
            _directionalLight.intensity = Mathf.Lerp(_nightIntensity, _dayIntensity, blendFactor);
        }

        private float CalculateBlendFactor()
        {
            // Smooth transition using sine wave
            // 0.25 = sunrise, 0.5 = noon, 0.75 = sunset, 0/1 = midnight
            float normalizedTime = _currentTime * 2f * Mathf.PI;
            return (Mathf.Sin(normalizedTime - Mathf.PI / 2f) + 1f) / 2f;
        }

        public void SetTime(float time)
        {
            _currentTime = Mathf.Clamp01(time);
            UpdateLighting();
            OnTimeChanged?.Invoke(_currentTime);
        }

        public void SetPaused(bool paused)
        {
            _isPaused = paused;
        }

        public void SetDayDuration(float seconds)
        {
            _dayDurationInSeconds = Mathf.Max(1f, seconds);
        }

        public void SetNightDuration(float seconds)
        {
            _nightDurationInSeconds = Mathf.Max(1f, seconds);
        }

        public void SetDaySettings(Color color, float intensity)
        {
            _dayColor = color;
            _dayIntensity = intensity;
            UpdateLighting();
        }

        public void SetNightSettings(Color color, float intensity)
        {
            _nightColor = color;
            _nightIntensity = intensity;
            UpdateLighting();
        }
    }
}

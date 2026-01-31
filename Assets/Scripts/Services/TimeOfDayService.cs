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
        [SerializeField] private float _dayRotationX = 30f;
        
        [Header("Night Settings")]
        [SerializeField] private Color _nightColor = new Color(0.2f, 0.2f, 0.4f);
        [SerializeField] private float _nightIntensity = 0.2f;
        [SerializeField] private float _nightRotationX = -30f;
        
        [Header("Current State")]
        [SerializeField] private float _currentTime;
        [SerializeField] private bool _isPaused;

        public float CurrentTime => _currentTime;
        public bool IsDay => _currentTime >= 0.25f && _currentTime < 0.75f;

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
            
            if (_wasDay != IsDay)
            {
                _wasDay = IsDay;
            }
        }

        private void UpdateLighting()
        {
            if (_directionalLight == null) return;
            
            float blendFactor = CalculateBlendFactor();
            
            _directionalLight.color = Color.Lerp(_nightColor, _dayColor, blendFactor);
            _directionalLight.intensity = Mathf.Lerp(_nightIntensity, _dayIntensity, blendFactor);
            
            float rotationX = Mathf.Lerp(_nightRotationX, _dayRotationX, blendFactor);
            Vector3 currentRotation = _directionalLight.transform.eulerAngles;
            _directionalLight.transform.rotation = Quaternion.Euler(rotationX, currentRotation.y, currentRotation.z);
        }

        private float CalculateBlendFactor()
        {
            float normalizedTime = _currentTime * 2f * Mathf.PI;
            return (Mathf.Sin(normalizedTime - Mathf.PI / 2f) + 1f) / 2f;
        }

        public void SetTime(float time)
        {
            _currentTime = Mathf.Clamp01(time);
            UpdateLighting();
        }
    }
}

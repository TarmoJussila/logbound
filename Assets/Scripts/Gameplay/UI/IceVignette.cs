using UnityEngine;
using UnityEngine.UI;

namespace Logbound.Gameplay.UI
{
    public class IceVignette : MonoBehaviour
    {
        [SerializeField] private Image _vignetteImage;
        
        [Header("Temperature Settings")]
        [SerializeField] private float _minCelsius = -20f;
        [SerializeField] private float _maxCelsius = 0f;
        
        [Header("Scale Settings")]
        [SerializeField] private float _minScale = 0.5f;
        [SerializeField] private float _maxScale = 1.5f;
        
        [Header("Alpha Settings")]
        [SerializeField] private float _minAlpha = 0f;
        [SerializeField] private float _maxAlpha = 1f;

        private void Update()
        {
            if (_vignetteImage == null)
            {
                return;
            }

            float temperature = WeatherTransitionController.Instance.GetCurrentTemperature();
            float t = Mathf.InverseLerp(_maxCelsius, _minCelsius, temperature);
            
            // Scale
            float scale = Mathf.Lerp(_minScale, _maxScale, t);
            transform.localScale = Vector3.one * scale;
            
            // Alpha
            float alpha = Mathf.Lerp(_minAlpha, _maxAlpha, t);
            Color color = _vignetteImage.color;
            color.a = alpha;
            _vignetteImage.color = color;
        }
    }
}


using Logbound.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Logbound.Gameplay.UI
{
    public class ForecastTimer : MonoBehaviour
    {
        [SerializeField] private Image _timerImage;
        
        private void Update()
        {
            if (_timerImage == null)
            {
                return;
            }
            _timerImage.fillAmount = ForecastService.Instance.GetForecastProgress();
        }
    }
}

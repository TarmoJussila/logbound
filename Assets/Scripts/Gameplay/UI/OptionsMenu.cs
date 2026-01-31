using UnityEngine;
using UnityEngine.UI;

namespace Logbound.Gameplay.UI
{
    public class OptionsMenu : MonoBehaviour
    {
        [SerializeField] private Toggle _invertVerticalLookToggle;

        public bool InvertVerticalLook { get; private set; }

        private void OnEnable()
        {
            _invertVerticalLookToggle.isOn = InvertVerticalLook;
        }

        private void Start()
        {
            _invertVerticalLookToggle.onValueChanged.AddListener(OnInvertVerticalLookChanged);
        }

        private void OnDestroy()
        {
            _invertVerticalLookToggle.onValueChanged.RemoveListener(OnInvertVerticalLookChanged);
        }

        private void OnInvertVerticalLookChanged(bool arg0)
        {
            InvertVerticalLook = arg0;
        }
    }
}
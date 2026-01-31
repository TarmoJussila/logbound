using Logbound.Services;
using UnityEngine;

namespace Logbound.Gameplay
{
    public class SkyboxController : MonoBehaviour
    {
        [Header("Blendable Skybox Material")]
        [SerializeField] private Material _blendableSkybox;
        
        [Header("Cubemaps")]
        [SerializeField] private Cubemap _dayCubemap;
        [SerializeField] private Cubemap _nightCubemap;

        private static readonly int TexA = Shader.PropertyToID("_TexA");
        private static readonly int TexB = Shader.PropertyToID("_TexB");
        private static readonly int Blend = Shader.PropertyToID("_Blend");

        private void Start()
        {
            if (_blendableSkybox == null) return;
            
            RenderSettings.skybox = _blendableSkybox;
            _blendableSkybox.SetTexture(TexA, _nightCubemap);
            _blendableSkybox.SetTexture(TexB, _dayCubemap);
        }

        private void Update()
        {
            if (_blendableSkybox == null) return;

            float blendFactor = CalculateBlendFactor();
            _blendableSkybox.SetFloat(Blend, blendFactor);
        }

        private float CalculateBlendFactor()
        {
            float currentTime = TimeOfDayService.Instance.CurrentTime;
            float normalizedTime = currentTime * 2f * Mathf.PI;
            return (Mathf.Sin(normalizedTime - Mathf.PI / 2f) + 1f) / 2f;
        }
    }
}


using System.Collections;
using Logbound.Data;
using Logbound.Utilities;
using UnityEngine;

namespace Logbound.Services
{
    public class RadioService : Singleton<RadioService>
    {
        public class TaggedNewsAudioClip
        {
            public WeatherState[] WeatherStates;
            public float Temperature;
            public AudioClip AudioClip;
        }
        
        [Header("References")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _newsSource;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GameObject _radioLocation;
        
        [Header("Settings")]
        [SerializeField] [Range(0.1f, 10f)] private float _musicVolume;
        [SerializeField] [Range(0.1f, 10f)] private float _newsVolume;

        private void Start()
        {
            _musicSource.transform.position = _radioLocation.transform.position;
            _newsSource.transform.position = _radioLocation.transform.position;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                PlayNewsSnippet();
            }
        }

        private IEnumerator FadeCanvasCoroutine(float start, float end, float speed)
        {
            for (float time = 0; time < 1f; time += Time.deltaTime * speed)
            {
                _canvasGroup.alpha = Mathf.Lerp(start, end, time);
                yield return null;
            }
                
            _canvasGroup.alpha = end;
        }

        private IEnumerator FadeVolume(float start, float end, float speed)
        {
            for (float time = 0; time < 1f; time += Time.deltaTime * speed)
            {
                _musicSource.volume = Mathf.Lerp(start, end, time);
                _newsSource.volume = Mathf.Lerp(start, end, time);
                yield return null;
            }
                
            _musicSource.volume = end;
            _newsSource.volume = end;
        }

        private void PlayNewsSnippet()
        {
            StopAllCoroutines();
            
            IEnumerator Coroutine()
            {
                yield return FadeCanvasCoroutine(0, 1, 1);
                _newsSource.Play();
                yield return FadeVolume(0, _musicVolume, 1);
                while (_newsSource.isPlaying)
                {
                    yield return null;
                }
                
                yield return FadeVolume(_musicVolume, 0, 1);
                yield return FadeCanvasCoroutine(1, 0, 1);
            }
            
            StartCoroutine(Coroutine());
        }
    }
}

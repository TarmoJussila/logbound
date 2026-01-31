using System.Collections;
using System.Globalization;
using Logbound.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Logbound.UI
{
    public class PlayerHudController : MonoBehaviour
    {
        public SplitScreenPlayer Player { get; private set; }

        private PlayerInteraction _playerInteraction;
        private PlayerDamage _playerDamage;
        private PlayerMaskHelper _playerMaskHelper;

        [SerializeField] private TextMeshProUGUI _tooltipText;

        [SerializeField] private GameObject _aliveElements;
        [SerializeField] private GameObject _deadElements;
        [SerializeField] private TextMeshProUGUI _holdToQuitText;

        [SerializeField] private Image _healthBarFast;
        [SerializeField] private Image _healthBarSlow;
        [SerializeField] private float _healthBarAnimDuration;

        private Coroutine _healthBarCoroutine;

        public void Initialize(SplitScreenPlayer player)
        {
            Player = player;

            _playerInteraction = player.GetComponent<PlayerInteraction>();
            _playerDamage = player.GetComponent<PlayerDamage>();
            _playerMaskHelper = player.GetComponent<PlayerMaskHelper>();

            _playerInteraction.OnInteractableFound += OnInteractableFound;

            _playerDamage.OnPlayerHeal += OnPlayerHeal;
            _playerDamage.OnPlayerDead += OnPlayerDead;
            _playerDamage.OnPlayerTakeDamage += OnPlayerTakeDamage;
            _playerDamage.OnPlayerResurrect += OnPlayerResurrect;

            SetPlayerAlive(true);
        }

        private void Update()
        {
            var time = Player.CrouchHoldTimeRemaining;
            if (time < double.MaxValue)
            {
                _holdToQuitText.gameObject.SetActive(true);
                _holdToQuitText.text = $"HOLD TO QUIT: {time.ToString("F2", CultureInfo.InvariantCulture)}s";
                if (time <= double.Epsilon)
                {
                    SceneManager.LoadScene("MainMenuScene");
                }
            }
            else
            {
                _holdToQuitText.gameObject.SetActive(false);
            }
        }

        private void OnPlayerResurrect()
        {
            SetPlayerAlive(true);
        }

        private void OnPlayerDead()
        {
            SetPlayerAlive(false);
        }

        private void OnPlayerTakeDamage()
        {
            UpdateHealthBar();
        }

        private void OnPlayerHeal()
        {
            UpdateHealthBar();
        }

        private void SetPlayerAlive(bool alive)
        {
            _deadElements.SetActive(!alive);
            _aliveElements.SetActive(alive);
        }

        private void OnInteractableFound(InteractableItem interactableItem)
        {
            _tooltipText.gameObject.SetActive(interactableItem != null);

            if (interactableItem == null)
            {
                return;
            }

            bool carriable = interactableItem is CarryableItem && _playerInteraction.CurrentCarryItem == null;

            string button = Player.MouseInput ? "E" : "Y";
            string tooltip = "Press " + button + " to " + (carriable ? "pick up" : "interact");

            _tooltipText.text = tooltip;
        }

        private void UpdateHealthBar()
        {
            _healthBarFast.fillAmount = Mathf.Clamp01((float)_playerDamage.Health / _playerDamage.MaxHealth);

            if (_healthBarCoroutine != null)
            {
                StopCoroutine(_healthBarCoroutine);
            }

            _healthBarCoroutine = StartCoroutine(AnimateHealthBar());
        }

        private IEnumerator AnimateHealthBar()
        {
            float t = -1f;
            float originalFill = _healthBarSlow.fillAmount;
            float targetFill = Mathf.Clamp01((float)_playerDamage.Health / _playerDamage.MaxHealth);

            while (t < 1f)
            {
                t += (Time.deltaTime / _healthBarAnimDuration) * 2;

                _healthBarSlow.fillAmount = Mathf.Lerp(originalFill, targetFill, Mathf.Clamp01(t));
                yield return null;
            }

            _healthBarSlow.fillAmount = targetFill;
        }
    }
}
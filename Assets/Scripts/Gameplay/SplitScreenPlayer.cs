using Logbound.Gameplay.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Logbound.Gameplay
{
    [RequireComponent(typeof(PlayerInput))]
    public class SplitScreenPlayer : MonoBehaviour
    {
        /// <summary>UI navigation only allows one input method at a time.</summary>
        private static int _pauseMenuLockIndex = -1;

        private Transform _cameraTransform;
        private CharacterController _characterController;
        private PlayerInput _playerInput;

        private float _currentStamina;
        [SerializeField] private float _maxStamina;

        [SerializeField] private float _jumpBufferLength = 0.07f;
        private float _jumpBuffer = 0.0f;

        private float _verticalVelocity;
        [SerializeField] private float _lookSpeed = 1f;
        [SerializeField] private float _mouseSensitivity = 0.5f;
        [SerializeField] private float _walkSpeed = 1f;
        [SerializeField] private float _runSpeed = 1.5f;

        private float _verticalLookRotation = 0.0f;
        private float _airTime;
        [SerializeField] private float _airtimeThreshold;
        [SerializeField] private float _terminalVelocity;
        [SerializeField] private float _jumpVelocity;
        [SerializeField] private float _gravity;

        [SerializeField] private OptionsMenu _optionsMenu;

        private Vector2 _moveInput;
        private Vector2 _lookInput;

        public bool MouseInput => _playerInput.currentControlScheme.Equals("Keyboard&Mouse");

        private void Awake()
        {
            _cameraTransform = GetComponentInChildren<Camera>().transform;
            _characterController = GetComponentInChildren<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            GetComponentInChildren<PlayerHudController>().Initialize(this);
        }

        private void Update()
        {
            float moveY = 0.0f;
            float moveX = _moveInput.x;
            float moveZ = _moveInput.y;

            if (_jumpBuffer > 0.0f)
            {
                _jumpBuffer -= Time.deltaTime;
            }

            UpdateGroundedCheck();

            if (IsGrounded())
            {
                HandleGrounded();
            }
            else
            {
                HandleNotGrounded();
            }

            float moveSpeed = _walkSpeed; //_inputReceiver.sprint ? walk : run
            Vector3 move = transform.right * (moveX * _walkSpeed) + transform.forward * (moveZ * _walkSpeed) +
                           Vector3.up * _verticalVelocity;

            _characterController.Move(move * Time.deltaTime);
            
            
            // Apply horizontal look input + horizontal recoil
            float horizontalLook = _lookInput.x * _lookSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, horizontalLook);

            // Apply vertical look input + vertical recoil
            float verticalLookDelta = _lookInput.y * _lookSpeed * Time.deltaTime;
            _verticalLookRotation = Mathf.Clamp(_verticalLookRotation - verticalLookDelta, -89f, 89f);

            _cameraTransform.rotation =
                Quaternion.Euler(_verticalLookRotation, _cameraTransform.rotation.eulerAngles.y, 0);

            _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -89f, 89f);

            _cameraTransform.rotation =
                Quaternion.Euler(_verticalLookRotation, _cameraTransform.rotation.eulerAngles.y, 0);
        }

        private void HandleNotGrounded()
        {
            if (_verticalVelocity > _terminalVelocity)
            {
                _verticalVelocity += _gravity * Time.deltaTime;
            }
        }

        private void HandleGrounded()
        {
            if (GetJumpInput())
            {
                Jump();
            }
            else
            {
                _verticalVelocity = 0f;
            }
        }

        private void Jump()
        {
            _verticalVelocity = _jumpVelocity;
            _airTime = _airtimeThreshold;
        }

        private bool GetJumpInput()
        {
            if (_jumpBuffer > 0.01f)
            {
                _jumpBuffer = 0.0f;
                return true;
            }

            return false;
        }

        private bool IsGrounded()
        {
            return _characterController.isGrounded || _airTime < _airtimeThreshold;
        }

        private void UpdateGroundedCheck()
        {
            //IsGrounded is super inconsistent, so only consider being not grounded after not detecting it for a while
            if (!_characterController.isGrounded)
            {
                _airTime += Time.deltaTime;
            }
            else
            {
                _airTime = 0f;
            }
        }

        private void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
        }

        private void OnLook(InputValue value)
        {
            _lookInput = value.Get<Vector2>();
            if (MouseInput)
            {
                _lookInput.x *= _mouseSensitivity;
                _lookInput.y *= _mouseSensitivity;
            }

            if (_optionsMenu.InvertVerticalLook)
            {
                _lookInput.y = -_lookInput.y;
            }
        }

        private void OnJump(InputValue value)
        {
            Debug.Log("Jump");
            if (value.isPressed)
            {
                _jumpBuffer = _jumpBufferLength;
            }
        }

        private void OnInteract(InputValue value)
        {
            //Debug.Log("Interact");
            if (value.isPressed)
            {
              //  _playerCanvas.gameObject.SetActive(!_playerCanvas.gameObject.activeSelf);
            }
        }

        private void OnAttack(InputValue value)
        {
            Debug.Log("Attack");
        }

        private void OnPause(InputValue value)
        {
            if (!value.isPressed)
            {
                return;
            }

            if (_pauseMenuLockIndex >= 0)
            {
                if (_pauseMenuLockIndex != _playerInput.playerIndex)
                {
                    Debug.LogWarning($"Can't open pause menu for player index '{_playerInput.playerIndex}', Already locked by player index '{_pauseMenuLockIndex}'");
                    ClosePauseMenu();
                    return;
                }
            }

            if (_optionsMenu.gameObject.activeSelf)
            {
                ClosePauseMenu();
                _pauseMenuLockIndex = -1;
            }
            else
            {
                OpenPauseMenu();
                _pauseMenuLockIndex = _playerInput.playerIndex;
            }
        }

        private void OpenPauseMenu()
        {
            
            _optionsMenu.gameObject.SetActive(true);
            _playerInput.SwitchCurrentActionMap("UI");
        }

        private void ClosePauseMenu()
        {
            _optionsMenu.gameObject.SetActive(false);
            _playerInput.SwitchCurrentActionMap("Player");
        }
    }
}

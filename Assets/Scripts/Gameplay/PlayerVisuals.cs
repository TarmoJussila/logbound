using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Logbound.Gameplay
{
    public class PlayerVisuals : MonoBehaviour
    {
        [SerializeField] private float _animSpeed;

        [SerializeField] private int _playerIndex;

        public List<Sprite> WalkAnim;
        public List<Sprite> IdleAnim;
        public List<Sprite> JumpAnim;

        [SerializeField] private SpriteRenderer _rend;

        public Anim CurrentAnimation { get; private set; }

        private List<Sprite> _currentAnim;
        private int _currentFrame;
        private int _maxFrames;

        private float _frameTimer;

        private Transform _targetPlayerTransform;

        private void Awake()
        {
            PlayerJoinHelper.OnPlayerAdded += CheckPlayers;
            PlayerJoinHelper.OnPlayerRemoved += CheckPlayers;
        }

        private void OnDestroy()
        {
            PlayerJoinHelper.OnPlayerAdded -= CheckPlayers;
            PlayerJoinHelper.OnPlayerRemoved -= CheckPlayers;
        }

        private void Start()
        {
            SetAnimation(Anim.Walk);
            CheckPlayers();
        }

        private void CheckPlayers()
        {
            var inputs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);

            var matchingInput = inputs.FirstOrDefault(i => i.playerIndex == _playerIndex);

            PlayerInput self = GetComponentInParent<PlayerInput>();

            if (matchingInput != null)
            {
                _targetPlayerTransform = matchingInput.transform;
            }

            _rend.enabled = matchingInput != null && matchingInput != self;
        }

        private void Update()
        {
            _frameTimer += Time.deltaTime;

            if (_targetPlayerTransform != null)
            {
                transform.forward = _targetPlayerTransform.position - transform.position;
            }

            if (_frameTimer >= _animSpeed)
            {
                _currentFrame++;

                if (_currentFrame >= _maxFrames)
                {
                    _currentFrame = 0;
                }

                _rend.sprite = _currentAnim[_currentFrame];
                _frameTimer = 0f;
            }
        }

        public void SetAnimation(Anim anim)
        {
            _currentAnim = new(GetFrames(anim));
            CurrentAnimation = anim;
            _currentFrame = 0;
            _maxFrames = _currentAnim.Count;
        }

        private List<Sprite> GetFrames(Anim anim)
        {
            switch (anim)
            {
                case Anim.Idle: return IdleAnim;
                case Anim.Walk: return WalkAnim;
                case Anim.Jump: return JumpAnim;
                default: return IdleAnim;
            }
        }
    }

    public enum Anim
    {
        Idle,
        Walk,
        Jump
    }
}

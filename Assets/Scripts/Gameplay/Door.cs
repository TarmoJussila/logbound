using System;
using Logbound.Gameplay;
using UnityEngine;

namespace Logbound
{
    public class Door : InteractableItem
    {
        public bool IsOpen;

        private float _initialRotationY;
        private float _targetRotationY;
        private float _rotationY;

        private void Start()
        {
            _initialRotationY = transform.localRotation.eulerAngles.y;
            _targetRotationY = transform.localRotation.eulerAngles.y + 90f;
        }

        public override void Interact(PlayerInteraction playerInteraction)
        {
            IsOpen = !IsOpen;
        }

        private void Update()
        {
            _rotationY = Mathf.MoveTowardsAngle(_rotationY, IsOpen ? _targetRotationY : _initialRotationY, Time.deltaTime * 3 * 90);
            transform.localRotation = Quaternion.Euler(0, _rotationY, 0);
        }
    }
}

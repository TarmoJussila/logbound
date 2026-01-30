using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Logbound
{
    public class PlayerInteraction : MonoBehaviour
    {
        public static event Action<PlayerInteraction, InteractableItem> OnPlayerInteractableFound;

        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Transform _carryRoot;
        [SerializeField] private float _scanFrequency = 0.1f;
        [SerializeField] private float _interactRange;
        [SerializeField] private LayerMask _interactLayerMask;

        private float _scanTimer;

        public InteractableItem LastFoundInteractable { get; private set; }

        public CarryableItem CurrentCarryItem { get; private set; }

        public void InteractPressed()
        {
            if (LastFoundInteractable == null)
            {
                return;
            }

            if (LastFoundInteractable is CarryableItem carryable)
            {
                CurrentCarryItem = carryable;
                CurrentCarryItem.StartCarry(this);
                CurrentCarryItem.transform.SetParent(_carryRoot);
                CurrentCarryItem.transform.localPosition = Vector3.zero;
                CurrentCarryItem.transform.forward = _carryRoot.forward;
            }
            else
            {
                LastFoundInteractable.Interact(this);
            }

        }

        public void DropPressed()
        {
            if (CurrentCarryItem == null)
            {
                return;
            }
            
            CurrentCarryItem.StopCarry();
            CurrentCarryItem = null;
        }

        private void Update()
        {
            _scanTimer -= Time.deltaTime;

            if (_scanTimer <= 0f)
            {
                ScanInteractables();
                _scanTimer = _scanFrequency;
            }
        }

        private void ScanInteractables()
        {
            if (!Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, _interactRange, _interactLayerMask))
            {
                InvokeInteractableLost();
                return;
            }

            if (!hit.collider.TryGetComponent<InteractableItem>(out InteractableItem item))
            {
                InvokeInteractableLost();
                return;
            }

            LastFoundInteractable = item;
            
            OnPlayerInteractableFound?.Invoke(this, item);
        }

        private void InvokeInteractableLost()
        {
            if (LastFoundInteractable != null)
            {
                LastFoundInteractable = null;
                OnPlayerInteractableFound?.Invoke(this, null);
            }
        }
    }
}

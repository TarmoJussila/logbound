using System;
using UnityEngine;

namespace Logbound
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class CarryableItem : InteractableItem
    {
        private Rigidbody _rb;
        private Collider[] _colliders;

        private PlayerInteraction beingCarriedBy;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _colliders = GetComponentsInChildren<Collider>();
        }

        public void StartCarry(PlayerInteraction playerInteraction)
        {
            _rb.isKinematic = true;

            foreach (Collider col in _colliders)
            {
                col.enabled = false;
            }

            beingCarriedBy = playerInteraction;

            OnStartCarry();
        }

        public void StopCarry()
        {
            _rb.isKinematic = false;

            foreach (Collider col in _colliders)
            {
                col.enabled = true;
            }

            beingCarriedBy = null;

            OnStopCarry();
        }

        protected virtual void OnStartCarry() { }

        protected virtual void OnStopCarry() { }

        public override void Interact(PlayerInteraction playerInteraction)
        {
            throw new NotImplementedException();
        }
    }
}

using UnityEngine;
using UnityEngine.Serialization;

namespace Logbound
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class BasicMaskItem : InteractableItem
    {
        private Rigidbody _rb;
        private Collider[] _colliders;

        public PlayerInteraction BeingWornBy { get; private set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _colliders = GetComponentsInChildren<Collider>();
        }

        public override void Interact(PlayerInteraction playerInteraction)
        {
            playerInteraction.GetComponent<PlayerMaskHelper>().WearMask(this);
            
            StartWearing(playerInteraction);
        }

        public void StartWearing(PlayerInteraction playerInteraction)
        {
            _rb.isKinematic = true;

            foreach (Collider col in _colliders)
            {
                col.enabled = false;
            }

            BeingWornBy = playerInteraction;
        }

        public void StopCarry()
        {
            _rb.isKinematic = false;

            foreach (Collider col in _colliders)
            {
                col.enabled = true;
            }

            BeingWornBy = null;
        }
    }

    public enum MaskType
    {
        GAS,
        WELDING,
        SKIMASK,
        COVID,
        AIRSOFT,
        PROTECTIVE,
        SKINCARE,
        SLEEPING
    }
}

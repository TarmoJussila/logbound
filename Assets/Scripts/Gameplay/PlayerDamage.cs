using System;
using UnityEngine;

namespace Logbound
{
    public class PlayerDamage : MonoBehaviour
    {
        public static event Action<PlayerDamage> OnPlayerTakeDamage;
        public static event Action<PlayerDamage> OnPlayerDead;
        public static event Action<PlayerDamage> OnPlayerHeal;

        public int Health { get; private set; }

        [SerializeField] private int _maxHealth;

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag($"Hazard"))
            {
                return;
            }

            if (!other.TryGetComponent(out Hazard hazard))
            {
                return;
            }

            TakeDamage(hazard.DamagePerTick);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;

            OnPlayerTakeDamage?.Invoke(this);

            if (Health <= 0)
            {
                Kill();
            }
        }

        public void Heal(int health)
        {
            
            Health += health;

            OnPlayerHeal?.Invoke(this);
        }

        public void Kill()
        {
            OnPlayerDead?.Invoke(this);
        }
    }
}

using System;
using UnityEngine;

namespace Logbound
{
    public class PlayerDamage : MonoBehaviour
    {
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
            
            
        }
    }
}

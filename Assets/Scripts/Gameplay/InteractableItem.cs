using UnityEngine;

namespace Logbound
{
    public abstract class InteractableItem : MonoBehaviour
    {
        public abstract void Interact(PlayerInteraction playerInteraction);
    }
}

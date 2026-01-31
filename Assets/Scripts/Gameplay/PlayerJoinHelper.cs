using System;
using UnityEngine;

namespace Logbound.Gameplay
{
    public class PlayerJoinHelper : MonoBehaviour
    {
        public static event Action OnPlayerAdded;
        public static event Action OnPlayerRemoved;
        
        private void OnPlayerJoined()
        {
            OnPlayerAdded?.Invoke();
        }

        private void OnPlayerLeft()
        {
            OnPlayerRemoved?.Invoke();
        }
    }
}

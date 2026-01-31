using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Logbound.Gameplay
{
    public class PlayerCameraLayerHelper : MonoBehaviour
    {
        [SerializeField] private List<LayerMask> _masks;

        private void Awake()
        {
            SplitScreenPlayer player = GetComponent<SplitScreenPlayer>();
            PlayerInput input = GetComponent<PlayerInput>();
            int index = input.playerIndex;

            GetComponentInChildren<Camera>().cullingMask = _masks[index].value;
        }
    }
}

using UnityEngine;

namespace Logbound
{
    public class PlayerMaskHelper : MonoBehaviour
    {
        [SerializeField] private Transform _maskRoot;
        
        public BasicMaskItem CurrentMask { get; private set; }
        
        public void WearMask(BasicMaskItem maskItem)
        {
            maskItem.transform.SetParent(_maskRoot);
            maskItem.transform.localPosition = Vector3.zero;
            maskItem.transform.forward = _maskRoot.transform.forward;

            CurrentMask = maskItem;
        }

        public void DropMask()
        {
            CurrentMask.transform.SetParent(null);
            CurrentMask.StopCarry();
            CurrentMask = null;
        }
    }
}

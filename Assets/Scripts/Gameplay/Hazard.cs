using UnityEngine;

namespace Logbound
{
    public class Hazard : MonoBehaviour
    {
        [field: SerializeField] public int DamagePerTick { get; private set; }
    }
}

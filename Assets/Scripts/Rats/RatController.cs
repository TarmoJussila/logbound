using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace Logbound.Rats
{
    public class RatController : MonoBehaviour
    {
        [Header("Rat GameObjects with SplineFollower")]
        [FormerlySerializedAs("ratObjects")]
        [SerializeField] private List<SplineFollower> _ratObjects = new List<SplineFollower>();
        [Header("Available Splines")]
        [FormerlySerializedAs("splines")]
        [SerializeField] private List<SplineContainer> _splines = new List<SplineContainer>();

        private void Start()
        {
            AssignRandomSplinesToRats();
        }

        public void AssignRandomSplinesToRats()
        {
            if (_splines.Count == 0)
            {
                Debug.LogWarning("No splines available to assign!");
                return;
            }

            foreach (SplineFollower ratObject in _ratObjects)
            {
                if (ratObject == null) continue;

                SplineFollower follower = ratObject.GetComponent<SplineFollower>();
                if (follower == null)
                {
                    Debug.LogWarning($"GameObject {ratObject.name} does not have a SplineFollower component!");
                    continue;
                }
                int randomIndex = Random.Range(0, _splines.Count);
                follower._splineContainer = _splines[randomIndex];
                follower.Initialize();
            }
        }
    }
}

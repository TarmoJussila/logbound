using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace Logbound
{
    public class RatController : MonoBehaviour
    {
        [Header("Rat GameObjects with SplineFollower")]
        [SerializeField] private List<SplineFollower> ratObjects = new List<SplineFollower>();
        [Header("Available Splines")]
        [SerializeField] private List<SplineContainer> splines = new List<SplineContainer>();

        void Start()
        {
            AssignRandomSplinesToRats();
        }

        public void AssignRandomSplinesToRats()
        {
            if (splines.Count == 0)
            {
                Debug.LogWarning("No splines available to assign!");
                return;
            }

            foreach (SplineFollower ratObject in ratObjects)
            {
                if (ratObject == null) continue;

                SplineFollower follower = ratObject.GetComponent<SplineFollower>();
                if (follower == null)
                {
                    Debug.LogWarning($"GameObject {ratObject.name} does not have a SplineFollower component!");
                    continue;
                }
                int randomIndex = Random.Range(0, splines.Count);
                follower.SplineContainer = splines[randomIndex];
                follower.Initialize();
            }
        }
    }
}

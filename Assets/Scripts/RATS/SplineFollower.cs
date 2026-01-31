using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{
    [Header("Settings")]
    public SplineContainer SplineContainer;
    public float Speed = 5f;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool faceForward = true;
    
    private float _distancePercentage = 0f;
    private float _splineLength;

    public void Initialize()
    {
        if (SplineContainer != null)
            _splineLength = SplineContainer.CalculateLength();
    }

    void Update()
    {
        if (SplineContainer == null || _splineLength <= 0) return;

        // Calculate progress
        _distancePercentage += (Speed * Time.deltaTime) / _splineLength;

        // Handle Looping logic
        if (loop)
        {
            // The % 1f keeps the value between 0 and 1
            _distancePercentage %= 1f; 
        }
        else
        {
            _distancePercentage = Mathf.Clamp01(_distancePercentage);
            if (_distancePercentage >= 1f) return; // Stop at the end
        }

        // Apply Position
        transform.position = (Vector3)SplineContainer.EvaluatePosition(_distancePercentage);

        // Apply Rotation
        if (faceForward)
        {
            float3 forward = SplineContainer.EvaluateTangent(_distancePercentage);
            float3 up = SplineContainer.EvaluateUpVector(_distancePercentage);
            
            if (!forward.Equals(float3.zero))
            {
                transform.rotation = Quaternion.LookRotation(forward, up);
            }
        }
    }
}
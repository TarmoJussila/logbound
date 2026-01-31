using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace Logbound.Rats
{
    public class SplineFollower : MonoBehaviour
    {
        [FormerlySerializedAs("SplineContainer")]
        [Header("Settings")]
        public SplineContainer _splineContainer;
        [FormerlySerializedAs("Speed")]
        public float _speed = 5f;
        [FormerlySerializedAs("loop")]
        [SerializeField] private bool _loop = true;
        [FormerlySerializedAs("faceForward")]
        [SerializeField] private bool _faceForward = true;
    
        private float _distancePercentage = 0f;
        private float _splineLength;

        public void Initialize()
        {
            if (_splineContainer != null)
            {
                _splineLength = _splineContainer.CalculateLength();
            }
        }

        private void Update()
        {
            if (_splineContainer == null || _splineLength <= 0) return;
            
            _distancePercentage += (_speed * Time.deltaTime) / _splineLength;
            
            if (_loop)
            {
                _distancePercentage %= 1f; 
            }
            else
            {
                _distancePercentage = Mathf.Clamp01(_distancePercentage);
                if (_distancePercentage >= 1f) return;
            }
            
            transform.position = (Vector3)_splineContainer.EvaluatePosition(_distancePercentage);
            
            if (_faceForward)
            {
                float3 forward = _splineContainer.EvaluateTangent(_distancePercentage);
                float3 up = _splineContainer.EvaluateUpVector(_distancePercentage);
            
                if (!forward.Equals(float3.zero))
                {
                    transform.rotation = Quaternion.LookRotation(forward, up);
                }
            }
        }
    }
}
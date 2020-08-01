using UnityEngine;

namespace AirP.CameraFunc
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _distance = 5f;
        [SerializeField] private float _height = 2f;
        [SerializeField] private float _smoothSpeed = 0.5f;

        private Vector3 _smoothVelocity;

        private void FixedUpdate()
        {
            if (_target)
            {
                HandleCamera();
            }
        }

        private void HandleCamera()
        {
            Vector3 wantedPos = _target.position + (-_target.forward * _distance) + (Vector3.up * _height);
            transform.position = Vector3.SmoothDamp(transform.position, wantedPos, ref _smoothVelocity, _smoothSpeed);
        
            transform.LookAt(_target);
        }
    }
}


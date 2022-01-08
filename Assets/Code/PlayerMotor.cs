using UnityEngine;

namespace PlayerNS
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _cameraRotationLimit = 85.0f;

        private Vector3 _velocity = Vector3.zero;
        private Vector3 _rotation = Vector3.zero;
        private float _cameraRotationX = 0.0f;
        private float _currentCameraRotationX = 0.0f;
        private Rigidbody _rigidbody;
        private Vector3 _thrusterForce = Vector3.zero;

        public void Move(Vector3 velocity)
        {
            _velocity = velocity;
        }

        public void Rotate(Vector3 rotation)
        {
            _rotation = rotation;
        }

        public void RotateCamera(float cameraRotationX)
        {
            _cameraRotationX = cameraRotationX;
        }

        public void ApplyThruster(Vector3 thrusterForce)
        {
            _thrusterForce = thrusterForce;
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            PerformMovement();
            PerformRotation();
        }

        private void PerformMovement()
        {
            if (_velocity != Vector3.zero)
            {
                _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
            }

            if(_thrusterForce != Vector3.zero)
            {
                _rigidbody.AddForce(_thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
            }
        }

        private void PerformRotation()
        {
            _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(_rotation));
            if (_camera != null)
            {
                _currentCameraRotationX -= _cameraRotationX;
                _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -_cameraRotationLimit, _cameraRotationLimit);

                _camera.transform.localEulerAngles = new Vector3(_currentCameraRotationX, 0.0f, 0.0f);
            }
        }
    }

}
using UnityEngine;

namespace PlayerNS
{
    [RequireComponent(typeof(ConfigurableJoint))]
    [RequireComponent(typeof(PlayerMotor))]
    class PlayerController : MonoBehaviour
    {
        #region publicFields



        #endregion


        #region privateFields

        [SerializeField] private float _speed = 5.0f;
        [SerializeField] private float _mouseSensitivity = 3.0f;
        [SerializeField] private float _thrusterForce = 1000.0f;

        [Header("Spring settings")]
        [SerializeField] private JointDriveMode _jointMode = JointDriveMode.Position;
        [SerializeField] private float _jointSpring = 1400.0f;
        [SerializeField] private float _jointMaxForice = 3000.0f;


        private PlayerMotor _motor;
        private ConfigurableJoint _joint;

        #endregion


        #region pulbicMethods



        #endregion


        #region privateMethods

        private void Start()
        {
            _motor = GetComponent<PlayerMotor>();
            _joint = GetComponent<ConfigurableJoint>();
            SetJointSettings(_jointSpring);
        }

        private void Update()
        {
            float xMove = Input.GetAxisRaw("Horizontal");
            float zMove = Input.GetAxisRaw("Vertical");
            Vector3 moveHorizontal = transform.right * xMove;
            Vector3 moveVertical = transform.forward * zMove;
            Vector3 velocity = (moveHorizontal + moveVertical).normalized * _speed;
            _motor.Move(velocity);

            float yRotation = Input.GetAxisRaw("Mouse X");
            Vector3 rotation = new Vector3(0.0f, yRotation, 0.0f) * _mouseSensitivity;
            _motor.Rotate(rotation);

            float xRotation = Input.GetAxisRaw("Mouse Y");
            float cameraRotationX = xRotation * _mouseSensitivity;
            _motor.RotateCamera(cameraRotationX);

            Vector3 thrusterForce = Vector3.zero;
            if (Input.GetButton("Jump"))
            {
                thrusterForce = Vector3.up * _thrusterForce;
                SetJointSettings(0.0f);
            }
            else
            {
                SetJointSettings(_jointSpring);
            }
            // Apply the thruster force (thrust - толкать)
            _motor.ApplyThruster(thrusterForce);

        }

        private void SetJointSettings(float jointSpring)
        {
            _joint.yDrive = new JointDrive { 
                mode = _jointMode,
                positionSpring = _jointSpring,
                maximumForce = _jointMaxForice
            };
        }

        #endregion
    }
}

using Game;
using UnityEngine;

namespace PlayerNS
{
    [RequireComponent(typeof(ConfigurableJoint))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(Animator))]
    class PlayerController : MonoBehaviour
    {
        #region publicFields



        #endregion


        #region privateFields

        [SerializeField] private float _speed = 5.0f;
        [SerializeField] private float _mouseSensitivity = 3.0f;
        [SerializeField] private float _thrusterForce = 1000.0f;

        [SerializeField] private float _thrusterFuelBurnSpeed = 1.0f;
        [SerializeField] private float _thrusterFueldRegenerationSpeed = 0.5f;
        [SerializeField] private LayerMask _environmentMask;

        [SerializeField] private Vector3 _playerNameLabelOffset = new Vector3(0.0f, 0.0f, 0.0f);
        [SerializeField] private float _playerNameLabelDistance = 15.0f;

        [Header("Spring settings")]
        [SerializeField] private float _jointSpring = 1400.0f;
        [SerializeField] private float _jointMaxForice = 3000.0f;

        [Header("Костыли")]
        [SerializeField] private float _verticalOffsetPosition = 1.0f;

        private PlayerMotor _motor;
        private ConfigurableJoint _joint;
        private Animator _animator;
        private float _thrusterFuelAmount = 1.0f;
        private Rect _playerNameLabelRect = new Rect(0, 0, 300, 100);
        private float _playerNameLabelSqrDistance;

        #endregion


        #region pulbicMethods

        public float GetThrusterFuelAmount()
        {
            return _thrusterFuelAmount;
        }

        #endregion


        #region privateMethods

        private void Start()
        {
            _motor = GetComponent<PlayerMotor>();
            _joint = GetComponent<ConfigurableJoint>();
            _animator = GetComponent<Animator>();
            SetJointSettings(_jointSpring);
            _playerNameLabelSqrDistance = _playerNameLabelDistance * _playerNameLabelDistance;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            // Update a spring joint target position
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0f, _environmentMask))
            {
                _joint.targetPosition = new Vector3(0.0f, -hit.point.y + _verticalOffsetPosition, 0.0f);
            }
            else
            {
                _joint.targetPosition = new Vector3(0.0f, 0.0f, 0.0f);
            }

            float xMove = Input.GetAxis("Horizontal");
            float zMove = Input.GetAxis("Vertical");
            Vector3 moveHorizontal = transform.right * xMove;
            Vector3 moveVertical = transform.forward * zMove;
            Vector3 velocity = (moveHorizontal + moveVertical).normalized * _speed;

            _animator.SetFloat("ForwardVelocity", zMove);
            _motor.Move(velocity);

            float yRotation = Input.GetAxis("Mouse X");
            Vector3 rotation = new Vector3(0.0f, yRotation, 0.0f) * _mouseSensitivity;
            _motor.Rotate(rotation);

            float xRotation = Input.GetAxis("Mouse Y");
            float cameraRotationX = xRotation * _mouseSensitivity;
            _motor.RotateCamera(cameraRotationX);

            Vector3 thrusterForce = Vector3.zero;
            if (Input.GetButton("Jump") && _thrusterFuelAmount > 0)
            {
                _thrusterFuelAmount -= _thrusterFuelBurnSpeed * Time.deltaTime;

                if(_thrusterFuelAmount >= 0.05f)
                {
                    thrusterForce = Vector3.up * _thrusterForce;
                    SetJointSettings(0.0f);
                }
            }
            else
            {
                _thrusterFuelAmount += _thrusterFueldRegenerationSpeed * Time.deltaTime;
                SetJointSettings(_jointSpring);
            }

            _thrusterFuelAmount = Mathf.Clamp(_thrusterFuelAmount, 0.0f, 1.0f);

            // Apply the thruster force (thrust - толкать)
            _motor.ApplyThruster(thrusterForce);

        }

        private void OnGUI()
        {
            DisplayPlayerNames();
        }

        private void DisplayPlayerNames()
        {
            foreach (var pair in GameManager.players)
            {
                Player player = pair.Value;
                if (player.isLocalPlayer)
                    continue;
                Vector3 distance = player.transform.position - transform.position;
                Vector3 direction = player.transform.position + _playerNameLabelOffset;
                if (distance.sqrMagnitude < _playerNameLabelSqrDistance)
                {
                    Vector3 screenPoint = _motor._camera.WorldToScreenPoint(direction);
                    if (screenPoint.z < 0)
                        continue;
                    _playerNameLabelRect.x = screenPoint.x;
                    _playerNameLabelRect.y = Screen.height - screenPoint.y;
                    //Rect rect = new Rect(new Vector2(screenPoint.x, Screen.height - screenPoint.y), new Vector2(10, player.playerName.Length * 10.5f));
                    GUI.Label(_playerNameLabelRect, player.playerName);
                }
            }
        }

        private void SetJointSettings(float jointSpring)
        {
            _joint.yDrive = new JointDrive { 
                positionSpring = _jointSpring,
                maximumForce = _jointMaxForice
            };
        }

        #endregion
    }
}

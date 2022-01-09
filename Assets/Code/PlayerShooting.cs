using Game;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayerNS
{
    class PlayerShooting : NetworkBehaviour
    {
        #region publicFields

        public PlayerWeapon weapon;

        #endregion


        #region privateFields

        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private PlayerWeapon _weapon;
        [SerializeField] private GameObject _weaponGraphics;
        [SerializeField] private string _weaponLayerName = "Weapon";

        private const string PLAYER_TAG = "Player";

        #endregion


        #region pulbicMethods



        #endregion


        #region privateMethods

        private void Start()
        {
            if (_camera == null)
            {
                Debug.LogError("PlayerShoot: No camera referenced!");
                this.enabled = false;
            }
            if(isLocalPlayer)
                _weaponGraphics.layer = LayerMask.NameToLayer(_weaponLayerName);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }

        [Client]
        private void Shoot()
        {
            RaycastHit hit;
            if (Physics.Raycast(_camera.transform.position,
                _camera.transform.forward,
                out hit,
                weapon.range,
                _layerMask))
            {
                Debug.Log("We hit " + hit.collider.name);
                if(hit.collider.tag == PLAYER_TAG)
                {
                    CmdPlayerShot(hit.collider.name, weapon.damage);
                }
            }
        }

        [Command]
        private void CmdPlayerShot(string playerID, int damage)
        {
            Debug.Log(playerID + " has been shot.");
            Player player = GameManager.GetPlayer(playerID);
            player.RpcTakeDamage(damage);
        }

        #endregion
    }
}

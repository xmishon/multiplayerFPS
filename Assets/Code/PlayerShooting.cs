using Game;
using Weapon;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayerNS
{
    [RequireComponent(typeof(WeaponManager))]
    class PlayerShooting : NetworkBehaviour
    {
        #region publicFields

        #endregion


        #region privateFields

        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _layerMask;

        private const string PLAYER_TAG = "Player";

        private WeaponManager _weaponManager;
        private PlayerWeapon _currentWeapon;

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

            _weaponManager = GetComponent<WeaponManager>();
        }

        private void Update()
        {
            _currentWeapon = _weaponManager.GetCurrentWeapon();
            if (_currentWeapon.fireRate <= 0.0f)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    Shoot();
                }

            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    InvokeRepeating("Shoot", 0.0f, 1.0f / _currentWeapon.fireRate);
                }
                else if (Input.GetButtonUp("Fire1"))
                {
                    CancelInvoke("Shoot");
                }
            }
        }

        [Client]
        private void Shoot()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            // We are shooting and calling the OnShoot method on the server
            CmdOnShoot();

            RaycastHit hit;
            if (Physics.Raycast(_camera.transform.position,
                _camera.transform.forward,
                out hit,
                _currentWeapon.range,
                _layerMask))
            {
                Debug.Log("We hit " + hit.collider.name);
                if(hit.collider.tag == PLAYER_TAG)
                {
                    CmdPlayerShot(hit.collider.name, _currentWeapon.damage);
                }

                // We hit something, call the OnHit method on the server
                CmdOnHit(hit.point, hit.normal);
            }
        }

        [Command]
        private void CmdPlayerShot(string playerID, int damage)
        {
            Debug.Log(playerID + " has been shot.");
            Player player = GameManager.GetPlayer(playerID);
            player.RpcTakeDamage(damage);
        }

        // Is called on the server when a player shoots
        [Command]
        private void CmdOnShoot()
        {
            RpcDoShootEffect();
        }

        //Is called on all clients when we need to do a shoot effect
        [ClientRpc]
        private void RpcDoShootEffect()
        {
            _weaponManager.GetCurrentGraphics().muzzleFlash.Play();
        }

        //Is called on the server when we hit something
        [Command]
        private void CmdOnHit(Vector3 position, Vector3 normal)
        {
            RpcDoHitEffect(position, normal);
        }

        // Is called on all clients
        // Here we can spawn in cool effects
        [ClientRpc]
        private void RpcDoHitEffect(Vector3 position, Vector3 normal)
        {
                                                                                        // We take a normal to a surface
            GameObject hitEffect =                                                      // as a rotation of a spawned effect
                Instantiate(_weaponManager.GetCurrentGraphics().hitEffectPrefab, position, Quaternion.LookRotation(normal));
            Destroy(hitEffect, 2.0f); // Destroy after 2 seconds
        }

        #endregion
    }
}

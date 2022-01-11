using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Game;

namespace PlayerNS
{
    [RequireComponent(typeof(PlayerSetup))]
    class Player : NetworkBehaviour
    {
        #region publicFields

        public bool isDead
        {
            get { return _isDead; }
            protected set { _isDead = value; }
        }

        [SyncVar] public string playerName;

        #endregion


        #region privateFields

        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private Behaviour[] _disabledOnDeath;
        [SerializeField] private GameObject[] _disableGameObjectsOnDeath;
        [SerializeField] private bool[] _wasEnabled;
        [SerializeField] private GameObject _deathEffect;
        [SerializeField] private GameObject _spawnEffect;

        private bool isFirstSetup = true;

        [SyncVar] private int _currentHealth;
        [SyncVar] private bool _isDead = false;

        #endregion


        #region pulbicMethods

        public void SetupPlayer()
        {
            if (isLocalPlayer)
            {
                // Switching on a scene camera
                GameManager.instance.SetSceneCameraActive(false);
                GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
            }
            
            CmdBroadcastNewPlayerSetup(FindObjectOfType<PlayerNameOverScene>().PlayerName);
        }



        public void SetDefaults()
        {
            _isDead = false;
            _currentHealth = _maxHealth;
            for (int i = 0; i < _disabledOnDeath.Length; i++)
            {
                _disabledOnDeath[i].enabled = _wasEnabled[i];
            }
            for (int i = 0; i < _disableGameObjectsOnDeath.Length; i++)
            {
                _disableGameObjectsOnDeath[i].SetActive(true);
            }
            // Turn off a scene camera
            if (isLocalPlayer)
            {
                GameManager.instance.SetSceneCameraActive(false);
                GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
            }

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }

            // Create spawn effect
            GameObject graphicInstance = Instantiate(_spawnEffect, transform.position, Quaternion.identity);
            Destroy(graphicInstance, 3.0f);
        }

        [ClientRpc]
        public void RpcTakeDamage(int amount)
        {
            if (isDead)
            {
                return;
            }
            _currentHealth -= amount;
            Debug.Log(transform.name + " now has " + _currentHealth + " health.");
            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        #endregion


        #region privateMethods

        private void Die()
        {
            isDead = true;

            for (int i = 0; i < _disabledOnDeath.Length; i++)
            {
                _disabledOnDeath[i].enabled = false;
            }

            for (int i = 0; i < _disableGameObjectsOnDeath.Length; i++)
            {
                _disableGameObjectsOnDeath[i].SetActive(false);
            }

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            GameObject graphicInstance = Instantiate(_deathEffect, transform.position, Quaternion.identity);
            Destroy(graphicInstance, 3.0f);

            if (isLocalPlayer)
            {
                GameManager.instance.SetSceneCameraActive(true);
                GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
            }

            StartCoroutine(Respawn());
        }

        private IEnumerator Respawn()
        {
            yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

            Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;

            // Delay for respawn to let all the clients to recieve new position of a player
            yield return new WaitForSeconds(0.2f);

            SetupPlayer();
        }

        private void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                RpcTakeDamage(9999);
            }
        }

        [Command]
        private void CmdBroadcastNewPlayerSetup(string playerName)
        {
            RpcSetupPlayerOnAllClients(playerName);
        }

        [ClientRpc]
        private void RpcSetupPlayerOnAllClients(string playerName)
        {
            if (isFirstSetup)
            {
                _wasEnabled = new bool[_disabledOnDeath.Length];
                for (int i = 0; i < _wasEnabled.Length; i++)
                {
                    _wasEnabled[i] = _disabledOnDeath[i].enabled;
                }
                this.playerName = playerName;
                isFirstSetup = false;
            }
            SetDefaults();
        }

        #endregion
    }
}

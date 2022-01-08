using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Game;

namespace PlayerNS
{
    class Player : NetworkBehaviour
    {
        #region publicFields

        public bool isDead
        {
            get { return _isDead; }
            protected set { _isDead = value; }
        }

        #endregion


        #region privateFields

        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private Behaviour[] _disabledOnDeath;
        [SerializeField] private bool[] _wasEnabled;

        [SyncVar] private int _currentHealth;
        [SyncVar] private bool _isDead = false;

        #endregion


        #region pulbicMethods

        public void Setup()
        {
            _wasEnabled = new bool[_disabledOnDeath.Length];
            for (int i = 0; i < _wasEnabled.Length; i++)
            {
                _wasEnabled[i] = _disabledOnDeath[i].enabled;
            }
            SetDefaults();
        }

        public void SetDefaults()
        {
            _isDead = false;
            _currentHealth = _maxHealth;
            for (int i = 0; i < _disabledOnDeath.Length; i++)
            {
                _disabledOnDeath[i].enabled = _wasEnabled[i];
            }

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }
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
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            StartCoroutine(Respawn());
        }

        private IEnumerator Respawn()
        {
            yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

            SetDefaults();
            Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
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

        #endregion
    }
}

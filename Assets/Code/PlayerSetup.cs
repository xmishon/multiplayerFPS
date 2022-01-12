using UnityEngine;
using Game;
using UI;
using UnityEngine.Networking;

namespace PlayerNS
{
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(PlayerController))]
    class PlayerSetup : NetworkBehaviour
    {
        #region publicFields



        #endregion


        #region privateFields

        [SerializeField] private Behaviour[] _componentsToDisable;
        [SerializeField] private string _remoteLayerName = "RemotePlayer";
        [SerializeField] private string _dontDrawLayerName = "DontDraw";
        [SerializeField] GameObject _playerGraphics;
        [SerializeField] private GameObject _playerUIPrefab;

        [HideInInspector]
        public GameObject playerUIInstance;

        #endregion


        #region pulbicMethods

        public override void OnStartClient()
        {
            base.OnStartClient();

            string netID = GetComponent<NetworkIdentity>().netId.ToString();
            Player player = GetComponent<Player>();

            GameManager.RegisterPlayer(netID, player);
        }

        #endregion


        #region privateMethods

        private void Start()
        {
            if (!isLocalPlayer)
            {
                DisableComponents();
                AssignRemoteLayer();
            }
            else
            {
                Utils.Util.SetLayerRecursively(_playerGraphics, LayerMask.NameToLayer(_dontDrawLayerName));

                // UI
                playerUIInstance = Instantiate(_playerUIPrefab);
                playerUIInstance.name = _playerUIPrefab.name;
                PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
                if (ui == null)
                {
                    Debug.LogError("No PlayerUI component on PlayerUI prefab.");
                }
                ui.SetController(GetComponent<PlayerController>());
                ui.SetPlayer(GetComponent<Player>());

                GetComponent<Player>().SetupPlayer();
            }
        }

        private void OnDisable()
        {
            Destroy(playerUIInstance);
            if (isLocalPlayer)
            {
                GameManager.instance.SetSceneCameraActive(true);
            }
            GameManager.UnregisterPlayer(transform.name);
            Cursor.lockState = CursorLockMode.None;
        }

        private void DisableComponents()
        {
            for (int i = 0; i < _componentsToDisable.Length; i++)
            {
                _componentsToDisable[i].enabled = false;
            }
        }

        private void AssignRemoteLayer()
        {
            gameObject.layer = LayerMask.NameToLayer(_remoteLayerName);
        }

        #endregion
    }
}

using UnityEngine;
using Game;
using UnityEngine.Networking;

namespace PlayerNS
{
    [RequireComponent(typeof(Player))]
    class PlayerSetup : NetworkBehaviour
    {
        #region publicFields



        #endregion


        #region privateFields

        [SerializeField] private Behaviour[] _componentsToDisable;
        [SerializeField] private Camera _sceneCamera;
        [SerializeField] private string _remoteLayerName = "RemotePlayer";
        [SerializeField] private string _dontDrawLayerName = "DontDraw";
        [SerializeField] GameObject _playerGraphics;
        [SerializeField] private GameObject _playerUIPrefab;

        private GameObject _playerUIInstance;

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
                _sceneCamera = Camera.main;
                if(_sceneCamera != null)
                {
                    _sceneCamera.gameObject.SetActive(false);
                }
                SetLayerRecursively(_playerGraphics, LayerMask.NameToLayer(_dontDrawLayerName));

                _playerUIInstance = Instantiate(_playerUIPrefab);
                _playerUIInstance.name = _playerUIPrefab.name;
            }

            GetComponent<Player>().Setup();
        }

        private void SetLayerRecursively(GameObject obj, int newLayer)
        {
            obj.layer = newLayer;
            foreach(Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

        private void OnDisable()
        {
            Destroy(_playerUIInstance);

            if (_sceneCamera != null)
            {
                _sceneCamera.gameObject.SetActive(true);
            }
            GameManager.UnregisterPlayer(transform.name);
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

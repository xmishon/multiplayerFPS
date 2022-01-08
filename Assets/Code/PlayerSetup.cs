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
            }

            GetComponent<Player>().Setup();
        }

        private void OnDisable()
        {
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

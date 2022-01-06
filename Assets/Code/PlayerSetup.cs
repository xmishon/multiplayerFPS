using UnityEngine;
using UnityEngine.Networking;

namespace Player
{
    class PlayerSetup : NetworkBehaviour
    {
        #region publicFields



        #endregion


        #region privateFields

        [SerializeField] private Behaviour[] _componentsToDisable;
        [SerializeField] private Camera _sceneCamera;

        #endregion


        #region pulbicMethods



        #endregion


        #region privateMethods

        private void Start()
        {
            if (!isLocalPlayer)
            {
                for(int i = 0; i < _componentsToDisable.Length; i++)
                {
                    _componentsToDisable[i].enabled = false;
                }
            }
            else
            {
                _sceneCamera = Camera.main;
                if(_sceneCamera != null)
                {

                }
                _sceneCamera.gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if (_sceneCamera != null)
            {
                _sceneCamera.gameObject.SetActive(true);
            }
        }

        #endregion
    }
}

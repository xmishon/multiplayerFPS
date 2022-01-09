using UnityEngine;
using PlayerNS;

namespace UI
{
    class PlayerUI : MonoBehaviour
    {
        #region publicFields



        #endregion


        #region privateFields

        [SerializeField] private RectTransform _thrusterFuelFill;

        private PlayerController _controller;

        #endregion


        #region pulbicMethods

        public void SetController(PlayerController controller)
        {
            _controller = controller;
        }

        #endregion


        #region privateMethods

        private void Update()
        {
            SetFuelAmount(_controller.GetThrusterFuelAmount());
        }

        private void SetFuelAmount(float amount)
        {
            _thrusterFuelFill.localScale = new Vector3(1.0f, amount, 1.0f);
        }

        #endregion
    }
}

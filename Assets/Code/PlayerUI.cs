using UnityEngine;
using PlayerNS;
using UnityEngine.UI;

namespace UI
{
    class PlayerUI : MonoBehaviour
    {
        #region publicFields



        #endregion


        #region privateFields

        [SerializeField] private RectTransform _thrusterFuelFill;
        [SerializeField] private Text _pointsText;

        private PlayerController _controller;
        private Player _player;

        #endregion


        #region pulbicMethods

        public void SetController(PlayerController controller)
        {
            _controller = controller;
        }

        public void SetPlayer(Player player)
        {
            _player = player;
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
            UpdatePoints(_player.points);
        }

        private void UpdatePoints(float points)
        {
            _pointsText.text = points.ToString();
        }

        #endregion
    }
}

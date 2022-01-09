using PlayerNS;
using Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Weapon
{
    class WeaponManager : NetworkBehaviour
    {
        #region publicFields



        #endregion


        #region privateFields

        [SerializeField] private PlayerWeapon _primaryWeapon;
        [SerializeField] private string _weaponLayerName = "Weapon";
        [SerializeField] private Transform _weaponHolder;

        private PlayerWeapon _currentWeapon;
        private WeaponGraphics _currentGraphics;

        #endregion


        #region pulbicMethods

        public PlayerWeapon GetCurrentWeapon()
        {
            return _currentWeapon;
        }

        public WeaponGraphics GetCurrentGraphics()
        {
            return _currentGraphics;
        }

        #endregion


        #region privateMethods

        private void Start()
        {
            EquipWeapon(_primaryWeapon);
        }

        private void EquipWeapon(PlayerWeapon weapon)
        {
            _currentWeapon = weapon;
            GameObject weaponInstance = Instantiate(weapon.graphics, _weaponHolder.position, _weaponHolder.rotation);
            weaponInstance.transform.SetParent(_weaponHolder);

            _currentGraphics = weaponInstance.GetComponent<WeaponGraphics>();
            if(_currentGraphics == null)
            {
                Debug.LogError("No WeaponGraphics component on the weapon object: " + weaponInstance.name);
            }

            if (isLocalPlayer)
            {
                Util.SetLayerRecursively(weaponInstance, LayerMask.NameToLayer(_weaponLayerName));
            }
        }

        #endregion
    }
}

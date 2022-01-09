using UnityEngine;

namespace PlayerNS
{
    [System.Serializable]
    public class PlayerWeapon
    {
        public string name = "Glock";
        public int damage = 10;
        public float range = 100.0f;

        public float fireRate = 0.0f;

        public GameObject graphics;

    }
}
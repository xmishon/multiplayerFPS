using UnityEngine;
using PlayerNS;
using System.Collections.Generic;
using Settings;

namespace Game
{
    class GameManager : MonoBehaviour
    {

        #region publicFields
        public static GameManager instance;
        public MatchSettings matchSettings;
        public static Dictionary<string, Player> players = new Dictionary<string, Player>();

        #endregion


        #region privateFields

        private const string PLAYER_ID_PREFIX = "Player ";

        [SerializeField] private GameObject _sceneCamera;


        #endregion


        #region pulbicMethods

        public static void RegisterPlayer(string netID, Player player)
        {
            string playerID = PLAYER_ID_PREFIX + netID;
            players.Add(playerID, player);
            player.transform.name = playerID;
        }

        public static void UnregisterPlayer(string playerID)
        {
            players.Remove(playerID);
        }

        public static Player GetPlayer(string playerID)
        {
            return players[playerID];
        }

        public void SetSceneCameraActive(bool isActive)
        {
            if (_sceneCamera == null)
            {
                return;
            }
            _sceneCamera.SetActive(isActive);
        }

        #endregion


        #region privateMethods

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("More than one GameManager in scene.");
            }
            else
            {
                instance = this;
            }
        }

        //private void OnGUI()
        //{
        //    GUILayout.BeginArea(new Rect(200, 200, 200, 500));
        //    GUILayout.BeginVertical();

        //    foreach (string playerID in players.Keys)
        //    {
        //        GUILayout.Label(playerID + " - " + players[playerID].transform.name);
        //    }

        //    GUILayout.EndVertical();
        //    GUILayout.EndArea();
        //}

        #endregion
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace BLabRouletteProject
{
    public class SupraorbsGUIController : MonoBehaviour
    {

        public GameObject PanelNotEnoughCoins;

        [DllImport("__Internal")]
        private static extern void RouletteOrbLoaded();

        [DllImport("__Internal")]
        private static extern void CoinsUpdated(float coins);

        #region temp vars
        private OrbHolder ORB { get { return OrbHolder.Instance; } }
        #endregion temp vars

        void Awake()
        {
            Debug.Log("SupraorbsGUIController: Awake");
        }

        #region regular
        private void Start()
        {
            Debug.Log("SupraorbsGUIController: Start");
            OrbHolder.LoginEvent += SupraorbsLoginHandler;
            OrbHolder.CoinsUpdatedEvent += CoinUpdateHandler;
            RouletteOrbLoaded();
        }
        #endregion regular

        #region event handlers
        public void SupraorbsLoginHandler(string chipsBalance)
        {
            Debug.Log("SupraorbsLoginHandler: " + ORB.initialPlayerBalance);
            ORB.initialPlayerBalance = chipsBalance;
            GameObject.Find("PlayerName").GetComponent<Text>().text = ORB.playerName;
            PlayerPrefs.SetFloat("PLAYER_CASH", float.Parse(ORB.initialPlayerBalance));
            GameObject.Find("TextPlayerTotalCoins").GetComponent<Text>().text = GameCoinsController.getFormattedValue(PlayerPrefs.GetFloat("PLAYER_CASH"));
            GameObject.Find("TextPlayerMinCoinsToPlay").GetComponent<Text>().text = GameCoinsController.getFormattedValue(PlayerPrefs.GetFloat("MIN_PLAY_CASH"));
            if (GameCoinsController.getCurrentCoins() < PlayerPrefs.GetFloat("MIN_PLAY_CASH")) 
            {
                PanelNotEnoughCoins.SetActive(true);
            }
        }

        public void CoinUpdateHandler(float coins)
        {
            CoinsUpdated(coins);
        }

        #endregion event handlers
    }
}



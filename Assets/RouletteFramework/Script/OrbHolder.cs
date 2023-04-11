using UnityEngine;
using System;

namespace BLabRouletteProject
{
    public class OrbHolder : MonoBehaviour
    {
        public static OrbHolder Instance;

        [SerializeField]
        private bool isLogined = false;
        public string playerName;
        public string initialPlayerBalance;

        public static Action<string> LoginEvent;
        public static Action<float> CoinsUpdatedEvent;
        #region regular
        private void Awake()
        {
            Debug.Log("OrbHolder Awake Function");
            if (Instance)
            {
                Debug.Log("OrbHolder Awake Function: True and Destroy");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("OrbHolder Awake Function: False and Setting Instance");
                Instance = this;
            }
            Initialize();
        }

        private void Start()
        {
            // add listeners for login event
            initialPlayerBalance = "0";
        }
        #endregion regular

        #region init
        private void Initialize()
        {
            playerName = "Supraorbs Casino";
        }
        #endregion init

        public bool IsLogined
        {
            get { return isLogined; }
        }

        public void OrbLogin(string balance)
        {
            Debug.Log("OrbLogin starts seting details");

            isLogined = true;
            initialPlayerBalance = balance;
            LoginEvent?.Invoke(initialPlayerBalance);
        }
    }
}

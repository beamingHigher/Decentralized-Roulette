using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using BLab.Utility;

#if USE_PHOTON
using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif

namespace BLabRouletteProject {

public class GameControllerMultiplayer : MonoBehaviour {

 public GameObject rouletteBallMultiplayer;

 public GameObject[] objectsToDeactivateForMasterPlayer;
 public GameObject[] objectsToDeactivateForOtherPlayer;
 public Text[] textToDisable;

 public GameObject playerPrefab;

 public GameObject playerMasterCasino;
 public GameObject playerVsCasino;
 public Transform MultiplayerPlayersRoot;

 public GameDataScriptableAsset gameDataScriptableAsset; 

 public GameObject PanelResult;
 public GameObject PanelOtherPlayerLeftGame;

 #if USE_PHOTON
   [HideInInspector]
   public PhotonView pv;

	// Use this for initialization
	void Start () {

	     foreach(Text t in textToDisable) {
	       t.enabled = false;
	     }


	      pv = GetComponent<PhotonView>();

			Debug.Log("[MPInSceneController][Start] Network : " + PhotonNetwork.connectedAndReady 
				+ "\n room.Name : " + PhotonNetwork.room.Name
				+ "\n player.NickName : " + PhotonNetwork.player.NickName
				+ "\n room.MaxPlayers : " + PhotonNetwork.room.MaxPlayers
				+ "\n room.PlayerCount: " + PhotonNetwork.room.PlayerCount
				+ "\n Game name: " + (string)PhotonNetwork.room.CustomProperties["MapName"]
				+ "\n Game avatarChoiceIdx: " + BLabUtility.avatarChoiceIdx
				+ "\n isMasterClient: " + PhotonNetwork.isMasterClient
		        );



			if(PhotonNetwork.isMasterClient) {

				playerMasterCasino = PhotonNetwork.Instantiate( playerPrefab.name, transform.position, Quaternion.identity, 0 ) as GameObject;
				playerMasterCasino.transform.SetParent(MultiplayerPlayersRoot);

				Hashtable ht = new Hashtable();
				Sprite avSprite = Resources.Load<Sprite>("Avatar/playerAvatar_" + BLabUtility.avatarChoiceIdx.ToString());
				ht["avatarImage"] = BLabUtility.getStringByteFromTexture(avSprite.texture);
				PhotonNetwork.player.SetCustomProperties(ht);


			} else {
				playerVsCasino = PhotonNetwork.Instantiate( playerPrefab.name, transform.position, Quaternion.identity, 0 ) as GameObject;
				playerVsCasino.transform.SetParent(MultiplayerPlayersRoot);

				Hashtable ht = new Hashtable();
				Sprite avSprite = Resources.Load<Sprite>("Avatar/playerAvatar_" + BLabUtility.avatarChoiceIdx.ToString());
				ht["avatarImage"] = BLabUtility.getStringByteFromTexture(avSprite.texture);
				PhotonNetwork.player.SetCustomProperties(ht);

				GameObject p = GameObject.Find("PanelWaitForPlayer");
	            if(p) p.SetActive(false);

			}

			PhotonNetwork.player.SetCustomProperties(new Hashtable(){{"playerCoins", GameCoinsController.getCurrentCoins() }});
			PhotonNetwork.player.SetCustomProperties(new Hashtable(){{"countryCode", PlayerPrefs.GetString("countryCode") }});
						
	}
	
	// Update is called once per frame
	void Update () {
		
	}

#region PhotonEvents


  public void OnDisconnectedFromPhoton() { 
	  Debug.Log("InSceneControllerMultiplayer *** OnDisconnectedFromPhoton ***");
      SceneManager.LoadScene(0);

  }

   void OnPhotonPlayerConnected(PhotonPlayer newPlayer) { 

	 GameObject p = GameObject.Find("PanelWaitForPlayer");
	 if(p) p.SetActive(false);

   }


   void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
     PanelOtherPlayerLeftGame.SetActive(true);
	 Invoke("disconnectPlayer",6);
   }

#endregion

  void disconnectPlayer() {
     PhotonNetwork.Disconnect();
  }

#endif
}
}
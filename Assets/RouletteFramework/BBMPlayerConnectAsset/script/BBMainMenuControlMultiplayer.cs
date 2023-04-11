using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BBMainMenuControlMultiplayer : MonoBehaviour {

	public GameObject BaseMenuWindow;
	public GameObject multiplayerWindows;
	public GameObject multiplayerWindowsDirect;
	public GameObject multiplayerConnectController;
	public CheckForMultiplayerRooms checkForMultiplayerRoomsScript; 

	public GameObject TextLoadingOnFastConnect;

	// Use this for initialization
	void Start () {

		#if UNITY_EDITOR
		gameObject.AddComponent<Blab.Utility.BBGetScreenShoot>();
		#endif
#if UNITY_WEBGL
		GameObject.Find("BUTTON_PLAY_MULTIPLAYER_BEST_SERVER").GetComponent<Button>().interactable = false;
#endif
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

//	public void gotButtonClick() {
//		Application.LoadLevel("demoMPConnect");
//	}

#if USE_PHOTON
	public void buttonsClickController(GameObject _go) {

		GameObject tap = Resources.Load("tapPrefab") as GameObject;
		if(tag != null) {
		  GameObject _tap = Instantiate(tap);
		  Destroy(_tap,1);
		}
	
		switch(_go.name) {
			case "BUTTON_PLAY_MULTIPLAYER_BEST" :
				BaseMenuWindow.SetActive(false);
				multiplayerWindows.SetActive(true);
				checkForMultiplayerRoomsScript.useDirectAccess = false;
				multiplayerConnectController.SetActive(true);
			break;
			case "BUTTON_PLAY_MULTIPLAYER_DIRECT":
				BaseMenuWindow.SetActive(false);
				multiplayerWindowsDirect.SetActive(true);
				checkForMultiplayerRoomsScript.useDirectAccess = true;
				multiplayerConnectController.SetActive(true);
			break;
		    case "BUTTON_PLAY_MULTIPLAYER_BEST_SERVER":
			    TextLoadingOnFastConnect.SetActive(true);
			    BaseMenuWindow.SetActive(false);
			    checkForMultiplayerRoomsScript.directConnectToBestServer = true;
			    checkForMultiplayerRoomsScript.useDirectAccess = false;
				multiplayerConnectController.SetActive(true);

		    break;
		    case "ButtonBackMainMenu":
		     SceneManager.LoadScene(0);
		    break;
	    }
	
	}
#endif	
	
}

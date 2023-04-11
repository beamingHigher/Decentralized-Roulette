using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BLab.Utility;

#if USE_PHOTON
using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif

namespace BLabRouletteProject {

public class MPCharacterControl : MonoBehaviour {

#if USE_PHOTON

 PhotonView pv;

 GameControllerMultiplayer mpGameController;

	IEnumerator Start () {

	 mpGameController = GameObject.FindGameObjectWithTag("MPGameController").GetComponent<GameControllerMultiplayer>();

	 yield return new WaitForSeconds(3);

	  pv = GetComponent<PhotonView>();

	
			if(BLab.Utility.BLabUtility.deepLog) Debug.Log("[MPCharacterControl][Start] isMine : " + pv.isMine);

		gameObject.name = pv.owner.NickName;

	     if(pv.isMine) {

			Image playerAvatarImage = transform.Find("Image").GetComponent<Image>();
			string imageData = (string)pv.owner.CustomProperties["avatarImage"];
			playerAvatarImage.sprite = BLabUtility.getSpriteFromBytes(imageData);

				//Debug.Log("[MPCharacterControl][Start] imageData : " + imageData);

			transform.Find("TextPlayerName").GetComponent<Text>().text = pv.owner.NickName;
			transform.Find("TextPlayerMoney").GetComponent<Text>().text = GameCoinsController.getFormattedValue( (float)pv.owner.CustomProperties["playerCoins"] );
			string countryCode = (string)pv.owner.CustomProperties["countryCode"];
			transform.Find("ImageCountry").GetComponent<RawImage>().texture = BLabUtility.getImageFromCountryCode(countryCode);
			transform.Find("ImageYou").GetComponent<Image>().enabled = true;

	     } else {

			Image playerAvatarImage = transform.Find("Image").GetComponent<Image>();
			string imageData = (string)pv.owner.CustomProperties["avatarImage"];
			playerAvatarImage.sprite = BLabUtility.getSpriteFromBytes(imageData);

				//Debug.Log("[MPCharacterControl][Start] imageData : " + imageData);

			transform.Find("TextPlayerName").GetComponent<Text>().text = pv.owner.NickName;
			transform.Find("TextPlayerMoney").GetComponent<Text>().text = GameCoinsController.getFormattedValue( (float)pv.owner.CustomProperties["playerCoins"] );
			string countryCode = (string)pv.owner.CustomProperties["countryCode"];
			transform.Find("ImageCountry").GetComponent<RawImage>().texture = BLabUtility.getImageFromCountryCode(countryCode);

	      }


		if(pv.owner.IsMasterClient) {
			 if(BLab.Utility.BLabUtility.deepLog) Debug.Log("[MPCharacterControl][Start] IsMasterClient IsMain");
			 if(pv.isMine) {
				foreach(GameObject g in mpGameController.objectsToDeactivateForMasterPlayer) {
				  Button b = g.GetComponent<Button>();
				  if(b != null) {
				    b.interactable = false;
				    g.GetComponent<BBUIButtonMessageForUUI>().enabled = false;
				  } else {
				   g.SetActive(false);
				  }
				}

				 /*GameObject[] allbetPoints = GameObject.FindGameObjectsWithTag("betObject");
				   foreach(GameObject g in allbetPoints) {
						g.layer = LayerMask.NameToLayer("Ignore Raycast");
				   }*/

			 }

			transform.SetParent(mpGameController.MultiplayerPlayersRoot);
			transform.localPosition =  mpGameController.MultiplayerPlayersRoot.Find("MultiplayerPlayerMasterPosition").localPosition;
			transform.localScale = new Vector3(20,20,0);
			transform.localEulerAngles = new Vector3(0,0,0);
		    //transform.localPosition = new Vector3(0,0,0);
			transform.Find("TextPlayerType").GetComponent<Text>().text = "Casino Owner";


		  	

		} else {
				if(BLab.Utility.BLabUtility.deepLog) Debug.Log("[MPCharacterControl][Start] *** NOT *** IsMasterClient NOT Mine");
			  if(pv.isMine) {
				foreach(GameObject g in mpGameController.objectsToDeactivateForOtherPlayer) {
				  Button b = g.GetComponent<Button>();
				  if(b != null) {
				    b.interactable = false;
				    g.GetComponent<BBUIButtonMessageForUUI>().enabled = false;
				  } else {
				   g.SetActive(false);
				  }
				}

			  }

			transform.SetParent(mpGameController.MultiplayerPlayersRoot);
			transform.localPosition =  mpGameController.MultiplayerPlayersRoot.Find("MultiplayerPlayerPosition").localPosition;
			transform.localScale = new Vector3(20,20,0);
			transform.localEulerAngles = new Vector3(0,0,0);
			transform.Find("TextPlayerType").GetComponent<Text>().text = "Player Vs Casino";
		   // transform.localPosition = new Vector3(0,0,0);

		}

																		
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

	}

#endif
}
}
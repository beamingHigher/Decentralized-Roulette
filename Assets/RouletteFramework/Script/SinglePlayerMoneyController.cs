using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

#if USE_PHOTON
using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif

namespace BLabRouletteProject {

public class SinglePlayerMoneyController : MonoBehaviour {

   public static float initialGameMoney = 1000;
	
	public void GetSetInitialPlayerMoney() {
	        GetComponent<BBRouletteController>().GotPlayerMoney(PlayerPrefs.GetFloat("MIN_PLAY_CASH"));
			GetComponent<BBRouletteController>().GotCasinoMoney(PlayerPrefs.GetFloat("MIN_PLAY_CASH"));

	}

#if USE_PHOTON

  IEnumerator ie_RPCShareResult(float value, float realVal) {

			string winner = "";
     string casinoCoins = "";
     string playerCoins = "";
     string handResultCasino = "";
	 string handResultPlayer = "";


			if(PhotonNetwork.player.IsMasterClient) {
				Debug.Log("RPCShareResult******* IsMasterClient  *******[SinglePlayerMoneyController] value : " + value + " realval : " + realVal);
				Debug.Log("RPCShareResult******* IsMasterClient  *******[SinglePlayerMoneyController] player cash : " +  GetComponent<BBRouletteController>().currentCash);
				Debug.Log("RPCShareResult******* IsMasterClient  *******[SinglePlayerMoneyController] casino cash : " +  GetComponent<BBRouletteController>().currentCasinoCash);

				  GetComponent<BBRouletteController>().spinButtonOnTable.SetActive(true);
				  GetComponent<BBRouletteController>().spinButtonOnTable.GetComponent<Button>().interactable = true;
				  GetComponent<BBRouletteController>().spinButtonOnTable.GetComponent<BBUIButtonMessageForUUI>().enabled = true;

				  GetComponent<BBRouletteController>().spinButtonOnCanvas.SetActive(true);
				  GetComponent<BBRouletteController>().spinButtonOnCanvas.GetComponent<Button>().interactable = true;
				  GetComponent<BBRouletteController>().spinButtonOnCanvas.GetComponent<BBUIButtonMessageForUUI>().enabled = true;

				  StartCoroutine(GetComponent<BBRouletteController>().hideBettingPoints(null));	

					if(realVal < 0) {
					  //Casino won
                      GameCoinsController.addCoins(Mathf.Abs(realVal));
                      winner = "CASINO";
					  handResultCasino = GameCoinsController.getFormattedValue(Mathf.Abs(realVal));
					  handResultPlayer = GameCoinsController.getFormattedValue(realVal);
					}
					else if(realVal > 0) {
					 // player won
					 GameCoinsController.removeCoins(realVal);
					  winner = "PLAYER";
					  handResultCasino = GameCoinsController.getFormattedValue(-realVal);
					  handResultPlayer = GameCoinsController.getFormattedValue(realVal);
					}
					else {
					  // draw
					  winner = "DRAW";
					  handResultCasino = "0.0";
					  handResultPlayer = "0.0";
					}
  
				   PhotonNetwork.player.SetCustomProperties(new Hashtable(){{"playerCoins", GameCoinsController.getCurrentCoins() }});

				   yield return new WaitForSeconds(4);

			} else {
				if(BLab.Utility.BLabUtility.deepLog) Debug.Log("RPCShareResult*****  *NOT* IsMasterClient  *********[SinglePlayerMoneyController] value : " + value + " realval : " + realVal);
				if(BLab.Utility.BLabUtility.deepLog) Debug.Log("RPCShareResult******  *NOT* IsMasterClient  ********[SinglePlayerMoneyController] player cash : " +  GetComponent<BBRouletteController>().currentCash);
				if(BLab.Utility.BLabUtility.deepLog) Debug.Log("RPCShareResult******  *NOT* IsMasterClient  ********[SinglePlayerMoneyController] casino cash : " +  GetComponent<BBRouletteController>().currentCasinoCash);

				GetComponent<BBRouletteController>().betInProgress = false;

				    if(realVal < 0) {
					  //Casino won
                      GameCoinsController.addCoins(realVal);
                      winner = "CASINO";
					  handResultPlayer = GameCoinsController.getFormattedValue(realVal);
					  handResultCasino = GameCoinsController.getFormattedValue(Mathf.Abs(realVal));
					}
					else if(realVal > 0) {
					 // player won
					 GameCoinsController.removeCoins(realVal);
					  winner = "PLAYER";
					  handResultPlayer = GameCoinsController.getFormattedValue(realVal);
					  handResultCasino = GameCoinsController.getFormattedValue(realVal);
					}
					else {
					  // draw
					  winner = "DRAW";
					  handResultPlayer = "0.0";
					  handResultCasino = "0.0";
					}

				   PhotonNetwork.player.SetCustomProperties(new Hashtable(){{"playerCoins", GameCoinsController.getCurrentCoins() }});

				  yield return new WaitForSeconds(4);

			}

			GameObject panelResult = GameObject.FindGameObjectWithTag("MPGameController").GetComponent<GameControllerMultiplayer>().PanelResult;
			panelResult.SetActive(true);

			foreach(PhotonPlayer pp in PhotonNetwork.playerList) {
			  if(pp.IsMasterClient) {
					casinoCoins =  GameCoinsController.getFormattedValue( (float)pp.CustomProperties["playerCoins"] );
			  } else {
					playerCoins =  GameCoinsController.getFormattedValue( (float)pp.CustomProperties["playerCoins"] );
			  }
				GameObject.Find(pp.NickName).transform.Find("TextPlayerMoney").GetComponent<Text>().text =
				GameCoinsController.getFormattedValue( (float)pp.CustomProperties["playerCoins"] );
			}

			panelResult.transform.Find("TextCasinoCoinsValue").GetComponent<Text>().text = casinoCoins;
			panelResult.transform.Find("TextPlayerCoinsValue").GetComponent<Text>().text = playerCoins;
			panelResult.transform.Find("TextHandResultWinnerValue").GetComponent<Text>().text = winner;
			panelResult.transform.Find("TextCasinoCoinsValueThisHand").GetComponent<Text>().text = handResultCasino;
			panelResult.transform.Find("TextPlayerCoinsValueThisHand").GetComponent<Text>().text = handResultPlayer;

			yield return new WaitForSeconds(6);

			panelResult.SetActive(false);

  }

[PunRPC] 
 void RPCShareResult(float value, float realVal) {
   StartCoroutine(ie_RPCShareResult(value,realVal));
 }
#endif

	public void gotMoneyResult(float value, float realVal) {

	 
         if(GetComponent<BBRouletteController>().isMultiplayer) {
#if USE_PHOTON
				if(PhotonNetwork.player.IsMasterClient) {
			      GetComponent<BBRouletteController>().pv.RPC("RPCShareResult",PhotonTargets.All,value,realVal);
			    }
#endif
         } else {
			   if(realVal > 0) {
				  GetComponent<BBRouletteController>().currentCasinoCash -= realVal;
				  GameCoinsController.addCoins(realVal);
			   } else {
				  GameCoinsController.removeCoins(Mathf.Abs(realVal));
				  GetComponent<BBRouletteController>().currentCasinoCash += Mathf.Abs(realVal);
			   }


				GetComponent<BBRouletteController>().TextCasinoCash.text = String.Format("{0:0,0}", GetComponent<BBRouletteController>().currentCasinoCash) + " $";

				if(BLab.Utility.BLabUtility.deepLog) Debug.Log("**************[SinglePlayerMoneyController] value : " + value + " realval : " + realVal);
				if(BLab.Utility.BLabUtility.deepLog) Debug.Log("**************[SinglePlayerMoneyController] player cash : " +  GetComponent<BBRouletteController>().currentCash);
				if(BLab.Utility.BLabUtility.deepLog) Debug.Log("**************[SinglePlayerMoneyController] casino cash : " +  GetComponent<BBRouletteController>().currentCasinoCash);

					if(GetComponent<BBRouletteController>().currentCash < 1) {
					 // player lose
						GetComponent<BBRouletteController>().panleLose.SetActive(true);
					}

					if(GetComponent<BBRouletteController>().currentCasinoCash < 1) {
					 // casino lose
						GetComponent<BBRouletteController>().panleWon.SetActive(true);
					}

					if(GetComponent<BBRouletteController>().ButtonRouletteSpin != null) {
						GetComponent<BBRouletteController>().ButtonRouletteSpin.SetActive(true);
					}
		 }
	}
			
}
}
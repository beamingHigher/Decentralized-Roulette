using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

namespace BLabRouletteProject {

public class BBRouletteController : MonoBehaviour {

  public bool isAmerican = false;
  public bool isMultiplayer = false;

  public GameObject PanelStats;
  public GameObject PanelCasinoNotEnoughCoins;
		 
  [HideInInspector]
  public bool isSinglePlayerMode = true;

	
   public Transform CanvasUI;
   public Transform rouletteCamera;

    public enum betResultType {single,split}

    public Text[] numberOutRed;
	public Text[] numberOutBlack;


	public GameObject wheelRoulette;
	
	 List<GameObject> betsList = new List<GameObject>();
	 List<GameObject> betsChipList = new List<GameObject>();

	 List<string> lastbetComplete = new List<string>();

    [HideInInspector]
	public float currentCash = 0;
	[HideInInspector]
	public float currentCashBetted = 0;
	[HideInInspector]
	public float currentCasinoCash = 0;
	
	
	
	Text TextMyCash;
	Text TextRouletteMessage;
	[HideInInspector]
	public Text TextCasinoCash;

	[HideInInspector]
	public bool betInProgress = false;

	[Header("Use for extracting simulation")]
	public bool simulateNumber = false;
	public string numberToSimulate = "0";
	
	Text TextCurrentMoneyBeted;
	
	private bool canBet = false;
	
	[HideInInspector]
	public GameObject ButtonRouletteSpin;
	
	GameObject Button_EXIT;
	
	GameObject  MobileJoystickHoiz;
	GameObject	MobileJoystickVert;
	
	public AudioClip clipPutChip;
	public AudioClip clipTap;

	// future use
    //public int playerPositionOnTable = 1;	

	public GameObject panleWon;
	public GameObject panleLose;

	public GameObject simulateNumberAlert;

	List<int> extrattedNumbers = new List<int>();

	private GameObject VoicesController;

	private bool autoSpin = false;
	private float autoSpinSeconds = 20;
	public GameObject spinButtonOnTable;
	public GameObject spinButtonOnCanvas;
	public Text TextSpinInSeconds;

	public Vector3 chipTransformSize = new Vector3(0.9f,0.3f,0.9f);

#if USE_PHOTON
    [HideInInspector]
    public PhotonView pv;
#endif

	void GotUpdatePlayerMoneyCash() {
		
		currentCashBetted = 0;
		tempWonResultForCasino = 0;

		TextCurrentMoneyBeted.text = String.Format("{0:0,0}", currentCashBetted) + " $";
		GameObject.Find("TextMyCashLastBet").GetComponent<Text>().text =  String.Format("{0:0,0}", 0.0f) + " $"; 

			if(ButtonRouletteSpin != null) {
				ButtonRouletteSpin.GetComponent<Button>().interactable = true;
		        ButtonRouletteSpin.GetComponent<BBUIButtonMessageForUUI>().enabled = true;
			}

		Button_EXIT.SetActive(true);		
			    
		VoicesController.SendMessage("playVoice","MessieursFaitesVosJeux",SendMessageOptions.DontRequireReceiver);

	}    
	
	void GotCanBet(bool val) {
		
		Debug.Log("**************GotCanBet***************GotCanBet : " + val);
		
		canBet = val;
			
			if(canBet) {   
				wheelRoulette.SendMessage("startSpin",SendMessageOptions.DontRequireReceiver);
				betInProgress = true;
				TextRouletteMessage.text = "Rien ne va plus, les jeux sont faits...";
		        TextRouletteMessage.color = Color.green;
			} else {
				TextRouletteMessage.text = "Sorry! Can't Bet Check Casino Or Your Money";
				TextRouletteMessage.color = Color.red;
				
			} 
		
		
		
	}	
	
	public void GotCasinoMoney(float val) {
			if(BLab.Utility.BLabUtility.deepLog) Debug.Log("**************GotCasinoMoney***************GotCasinoMoney : " + val);
		
		if(val == -100.0f) {
			TextRouletteMessage.text = "ERROR Getting Data Connections Problems...";
			TextRouletteMessage.color = Color.red;
			Invoke("disconnectAfterFatalError",6);
		} else {
			currentCasinoCash = val;
			TextCasinoCash.text = String.Format("{0:0,0}", currentCasinoCash) + " $";
		}
	}	
	
	public void GotPlayerMoney(float val) {
			if(BLab.Utility.BLabUtility.deepLog) Debug.Log("**************GotPlayerMoney*************** : " + val);
		
		if(val == -100.0f) {
			TextRouletteMessage.text = "ERROR Getting Data Connections Problems...";
			TextRouletteMessage.color = Color.red;
			Invoke("disconnectAfterFatalError",6);
		} else {
			currentCash = val;
			TextMyCash.text = String.Format("{0:0,0}", currentCash) + " $";
		}
		
	}	
	
	
	
	void Awake() {

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	
	    if(isSinglePlayerMode) {
	      if(!(GetComponent<SinglePlayerMoneyController>()))  gameObject.AddComponent<SinglePlayerMoneyController>();
	    }
	

		if(CanvasUI.parent != null) 
	          CanvasUI.parent.SetParent(null,false); //= null;
	   
	   setBetObjectView(false);
	   
	  // rouletteCamera.parent = null;
	}

	float savedAutoSpinSeconds = 0;

	void seconsCountDown() {
			autoSpinSeconds--;
			TextSpinInSeconds.text = autoSpinSeconds.ToString();
	}

	void executeAutoSpin() {
			    wheelRoulette.GetComponent<BBRotate>().startSpin();
                saveLastBet();
			    if(!isAmerican) setMultiBetButtons(false);
				VoicesController.SendMessage("playVoice","rienneVaPlusJeuxFait",SendMessageOptions.DontRequireReceiver);
			    betInProgress = true;
			    CancelInvoke("seconsCountDown");
			    autoSpinSeconds = savedAutoSpinSeconds;
			    TextSpinInSeconds.text = "0";
	}

	// Use this for initialization
	void Start () {
	    if(PlayerPrefs.HasKey("UseAutoSpin")) {
		    if(PlayerPrefs.GetInt("UseAutoSpin") == 1) {
				GameObject.Find("ImageAutoSpin").GetComponent<Image>().color = Color.green;
				autoSpin = true;
				autoSpinSeconds = PlayerPrefs.GetFloat("AutoSpinSeconds");
				Invoke("executeAutoSpin",autoSpinSeconds);
				InvokeRepeating("seconsCountDown",1,1);
				savedAutoSpinSeconds = autoSpinSeconds;
				Destroy( spinButtonOnTable );
		        Destroy( spinButtonOnCanvas );
				TextSpinInSeconds.enabled = true;
			} else {
				GameObject.Find("ImageAutoSpin").GetComponent<Image>().color = Color.red;
			}
        } else {
				GameObject.Find("ImageAutoSpin").GetComponent<Image>().color = Color.red;
        }

		VoicesController = GameObject.Find("VoicesController");
	    
		Button_EXIT = GameObject.Find("Button_EXIT");
		ButtonRouletteSpin = GameObject.Find("ButtonRouletteSpin");
	
		TextCurrentMoneyBeted = GameObject.Find("TextCurrentMoneyBeted").GetComponent<Text>();

/*
		GameObject sim = GameObject.Find("ToggleWantSimulate");
		
		if(sim) { 
		   simulateNumber = sim.GetComponent<Toggle>().isOn;
		} else {
		  simulateNumber = false;
		}
		
		if(sim) GameObject.Find("InputFieldTestNumber").GetComponent<InputField>().text = numberToSimulate;
*/

		TextRouletteMessage = GameObject.Find("TextRouletteMessage").GetComponent<Text>();
		TextRouletteMessage.text = "Starts Betting...";
		TextMyCash = GameObject.Find("TextMyCash").GetComponent<Text>();
		TextMyCash.text =  String.Format("{0:0,0}", currentCash) + " $"; 

		TextCasinoCash = GameObject.Find("TextCasinoCash").GetComponent<Text>();
		TextCasinoCash.text =  String.Format("{0:0,0}", currentCasinoCash) + " $"; 
		
		if(simulateNumber) {
		  simulateNumberAlert.SetActive(true);
				simulateNumberAlert.GetComponent<Text>().text = simulateNumberAlert.GetComponent<Text>().text + " Numb : " + numberToSimulate;
		}
	    if(isSinglePlayerMode) {
	        GetComponent<SinglePlayerMoneyController>().GetSetInitialPlayerMoney();
	    }		
	}
	
	// Update is called once per frame
	void Update () {
	
		if( Input.GetMouseButtonDown(0) && !betInProgress && canUseClick)
		{

			if(GetComponent<BBMoveObjectsController>().gotHandEnds) {
				GetComponent<BBMoveObjectsController>().resetAllBettingPos();
				GotUpdatePlayerMoneyCash();
				if(!isAmerican) setMultiBetButtons(true);
			}

			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit hit;
			
			if( Physics.Raycast( ray, out hit, 100 ) )
			{
				if(hit.collider.gameObject.tag == "betObject") {
				     
						instantiateNewChip(hit);

			     	//Debug.Log( hit.transform.gameObject.name );
			     	  /* if(currentCash >= selectedMoneyToBet) {
							GameObject chip = Instantiate(Resources.Load("BetPositionChip"),hit.collider.transform.position,Quaternion.identity)  as GameObject;
							chip.name = hit.transform.gameObject.name; // + "#" + playerPositionOnTable.ToString();
							chip.GetComponent<BBChipData>().betValue = selectedMoneyToBet;
							chip.transform.Find("chipgreen").transform.localScale = chipTransformSize;//new Vector3(0.9f,0.3f,0.9f);

							if(selectedMoneyToBet == 500) { chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().redMat;}
							else if(selectedMoneyToBet == 1000) { chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().blueMat;}
			                else if(selectedMoneyToBet == 100) { chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().greenMat;}
							else {
								chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().doubleMat;
								chip.GetComponent<BBChipData>().resultCanvas.SetActive(true);
								chip.GetComponent<BBChipData>().resultCanvas.transform.Find("Canvas/ResultValue").GetComponent<Text>().text = selectedMoneyToBet.ToString();
							}

							gotNumberEnterOnTableTrigger(hit.transform.gameObject,chip);
							currentCash -= selectedMoneyToBet; TextMyCash.text = String.Format("{0:0,0}", currentCash) + " $";
						    currentCashBetted += selectedMoneyToBet; TextCurrentMoneyBeted.text = String.Format("{0:0,0}", currentCashBetted) + " $";
						
						    playClipOneShot(clipPutChip);
							
						} else {
						  TextRouletteMessage.text = "Ops... No Money to Bet!";
						}*/

			    }
				if(hit.collider.gameObject.tag == "betChip") {
				    removeChip(hit);
				   /*
					currentCash += hit.collider.gameObject.GetComponent<BBChipData>().betValue; TextMyCash.text = String.Format("{0:0,0}", currentCash) + " $";
					currentCashBetted -= hit.collider.gameObject.GetComponent<BBChipData>().betValue; TextCurrentMoneyBeted.text = String.Format("{0:0,0}", currentCashBetted) + " $";
					gotNumberExitOnTableTrigger(hit.transform.gameObject,hit.collider.gameObject);
					Destroy(hit.collider.gameObject);
					playClipOneShot(clipPutChip);
					*/
				 }
				
			}
		}
		
	}


	bool checkIfCanBetCoseCasinoHasMoney(string betType) {
		bool tmpRet = false;
	   return tmpRet;
	}
				
	float tempWonResult = 0;
	float tempWonResultForCasino = 0;

	void processWinResult(string winNumber, string goNameNumber, int typeLength) {


		if(BLab.Utility.BLabUtility.deepLog) Debug.Log("[BBRouletteController][processWinResult] : " + winNumber + " : " + goNameNumber + " : " + typeLength + " realLenght : " + goNameNumber.Length);
	
		GameObject[] res = GameObject.FindGameObjectsWithTag("betChip");
	
	   switch(typeLength) {
	     case 1:
	         foreach(GameObject go in res) {
				if(go.name == goNameNumber) {
                     	tempWonResult += (go.GetComponent<BBChipData>().betValue) * 36;    
					    tempWonResultForCasino += (go.GetComponent<BBChipData>().betValue) * 35;           
				}
	         }
	     break;
		 case 2:
			foreach(GameObject go in res) {
				if(go.name == goNameNumber) {
					tempWonResult += (go.GetComponent<BBChipData>().betValue) * 18;   
					tempWonResultForCasino += (go.GetComponent<BBChipData>().betValue) * 17;           
				}
			}
			break;
		  case 3:
			foreach(GameObject go in res) {
				if(go.name == goNameNumber) {
					tempWonResult += (go.GetComponent<BBChipData>().betValue) * 12;               
					tempWonResultForCasino += (go.GetComponent<BBChipData>().betValue) * 11;           
				}
			}
			break;
			
		 case 4:
			foreach(GameObject go in res) {
				if(go.name == goNameNumber) {
					tempWonResult += (go.GetComponent<BBChipData>().betValue) * 8;  
					tempWonResultForCasino += (go.GetComponent<BBChipData>().betValue) * 7;           
				}
			}
			break;
		  case 6:
			foreach(GameObject go in res) {
				if(go.name == goNameNumber) {
					tempWonResult += (go.GetComponent<BBChipData>().betValue) * 6;      
					tempWonResultForCasino += (go.GetComponent<BBChipData>().betValue) * 5;           
				}
			}
			break;
			
		  case 12:
			foreach(GameObject go in res) {
					if(go.name == goNameNumber) {
						tempWonResult += (go.GetComponent<BBChipData>().betValue) * 3;   
						tempWonResultForCasino += (go.GetComponent<BBChipData>().betValue) * 2;  
					}
			}
			break;
			
		  case 18:
			foreach(GameObject go in res) {
				if(go.name == goNameNumber) {
					tempWonResult += (go.GetComponent<BBChipData>().betValue) * 2;      
					tempWonResultForCasino += (go.GetComponent<BBChipData>().betValue) * 1;  
				}
			}
			break;
			
	   }
	
	    currentCash += tempWonResult;
	    
		TextMyCash.text =  String.Format("{0:0,0}", currentCash) + " $"; 
	
	}
	
	void checkResult(string winNumber) {

	string goNumber = "";
	bool gotAWon = false;
	int chipMultiplier = 0;
	
		foreach(GameObject s in betsList) {
		       
//		        string newString = s.name.Substring(s.name.Length-1);
//			    Debug.Log("+++++++++++++++++++++++++++++++ newString : " + newString + " s : " + s);

				string[] numbers = s.name.Split(new char[] { '_' });
//			    Debug.Log("--> : " + s + " : " + numbers.Length); 
			    
			    switch(numbers.Length) {
			      case 1: 
				          goNumber = numbers[0];
				          if(numbers[0] == winNumber) {gotAWon = true; processWinResult(winNumber,goNumber,1); foreach(GameObject g in betsChipList) {if(g.name == s.name) {g.GetComponent<BBChipData>().wonChip = true;}}}  
				          goNumber = "";
			      break;
			      case 2:
				         goNumber = numbers[0] + "_" + numbers[1]; 
				          if(numbers[0] == winNumber) {gotAWon = true;  processWinResult(winNumber,goNumber,2); foreach(GameObject g in betsChipList) {if(g.name == s.name) {g.GetComponent<BBChipData>().wonChip = true;}}}
				          if(numbers[1] == winNumber) {gotAWon = true;  processWinResult(winNumber,goNumber,2); foreach(GameObject g in betsChipList) {if(g.name == s.name) {g.GetComponent<BBChipData>().wonChip = true;}}}
				         goNumber = "";
			      break;
				  case 3:
				        goNumber = numbers[0] + "_" + numbers[1] + "_" + numbers[2]; 
				         if(numbers[0] == winNumber) {gotAWon = true;  processWinResult(winNumber,goNumber,3); foreach(GameObject g in betsChipList) {if(g.name == s.name) {g.GetComponent<BBChipData>().wonChip = true;}}}
				         if(numbers[1] == winNumber) {gotAWon = true;  processWinResult(winNumber,goNumber,3); foreach(GameObject g in betsChipList) {if(g.name == s.name) {g.GetComponent<BBChipData>().wonChip = true;}}}
				         if(numbers[2] == winNumber) {gotAWon = true;  processWinResult(winNumber,goNumber,3); foreach(GameObject g in betsChipList) {if(g.name == s.name) {g.GetComponent<BBChipData>().wonChip = true;}}}
				        goNumber = "";
						break;
				  case 4:
				        goNumber = numbers[0] + "_" + numbers[1] + "_" + numbers[2] + "_" + numbers[3]; 
						if(numbers[0] == winNumber) {
						    gotAWon = true;  processWinResult(winNumber,goNumber,4);
					        foreach(GameObject g in betsChipList) {if(g.name == s.name) {g.GetComponent<BBChipData>().wonChip = true;}}
						}
						if(numbers[1] == winNumber) {gotAWon = true;  processWinResult(winNumber,goNumber,4); foreach(GameObject g in betsChipList) {if(g.name == s.name) {g.GetComponent<BBChipData>().wonChip = true;}}}
						if(numbers[2] == winNumber) {gotAWon = true;  processWinResult(winNumber,goNumber,4); foreach(GameObject g in betsChipList) {if(g.name == s.name) {g.GetComponent<BBChipData>().wonChip = true;}}}
						if(numbers[3] == winNumber) {gotAWon = true;  processWinResult(winNumber,goNumber,4); foreach(GameObject g in betsChipList) {if(g.name == s.name) {g.GetComponent<BBChipData>().wonChip = true;}}}
				        goNumber = "";
						break;
				  case 6:
						for(int x = 0; x < 6; x++) { if(x == 5) goNumber = goNumber + numbers[x]; else goNumber = goNumber + numbers[x] + "_"; }
						//Debug.Log("-->6 : " + goNumber);
				        for(int x = 0; x < 6; x++) {
				              if(numbers[x] == winNumber) {
				                   gotAWon = true;  
				                   processWinResult(winNumber,goNumber,6);
						            foreach(GameObject g in betsChipList) {
				                      if(g.name == s.name) {
				                         g.GetComponent<BBChipData>().wonChip = true;
				                      }
				                    }
				              }
				        }
				        goNumber = "";
						break;
			      case 12:
						for(int x = 0; x < 12; x++) { if(x == 11) goNumber = goNumber + numbers[x]; else goNumber = goNumber + numbers[x] + "_"; }
						//Debug.Log("-->12 : " + goNumber);
				        for(int x = 0; x < 12; x++) {
				             if(numbers[x] == winNumber) {
				                    gotAWon = true; 
				                    processWinResult(winNumber,goNumber,12);
						            foreach(GameObject g in betsChipList) {
				                      if(g.name == s.name) {
				                         g.GetComponent<BBChipData>().wonChip = true;
				                      }
				                    }
				             }
				        }
				        goNumber = "";
						break;
				
			      case 18:
						for(int x = 0; x < 18; x++) { if(x == 17) goNumber = goNumber + numbers[x]; else goNumber = goNumber + numbers[x] + "_"; }
				           for(int x = 0; x < 18; x++) {
				              if(numbers[x] == winNumber) {
							       if(BLab.Utility.BLabUtility.deepLog) Debug.Log("-->18 : " + goNumber + " GameObject : " + s.name);
				                    gotAWon = true;  
				                    processWinResult(winNumber,goNumber,18); 
				                    foreach(GameObject g in betsChipList) {
				                      if(g.name == s.name) {
				                         g.GetComponent<BBChipData>().wonChip = true;
				                      }
				                    }
				               }
				            }
				        goNumber = "";
						break;
						
				  default:
				  
			    	gotGameResult (0,0);		
				  break;		 
			    } 
		}
		
		if(!gotAWon) { 
		    StartCoroutine( gotGameResult(0,0) );
			GameObject.Find("TextMyCashLastBet").GetComponent<Text>().text =  String.Format("{0:0,0}", tempWonResult) + " $"; 
		} else {
			StartCoroutine( gotGameResult(tempWonResult,chipMultiplier) );
			GameObject.Find("TextMyCashLastBet").GetComponent<Text>().text =  String.Format("{0:0,0}", tempWonResult) + " $"; 
		}

	

	}
	
	
	
	public void onChangeToggleWantSimulate(Toggle tg) {
		simulateNumber = GameObject.Find("ToggleWantSimulate").GetComponent<Toggle>().isOn;
	}
	
	public void onChangeSimulateNumber(InputField value) {
		numberToSimulate = value.text;
		GameObject.Find("InputFieldTestNumber").GetComponent<InputField>().text = numberToSimulate;
	}


#if USE_PHOTON
   [PunRPC]
   void RPCShareWinnerNumber(string winNumber,Vector3 ballPos) {
	  GameObject.FindGameObjectWithTag("MPGameController").GetComponent<GameControllerMultiplayer>().rouletteBallMultiplayer.transform.position = ballPos;

	       VoicesController.SendMessage("playVoice",winNumber,SendMessageOptions.DontRequireReceiver);

			Debug.Log("*** RPCShareWinnerNumber *** [BBRouletteController] gotNumber winner : " + winNumber);

		setNumberOut(winNumber);
		checkResult(winNumber);
		betInProgress = false;

		GetComponent<BBMoveObjectsController>().winnerNumber = winNumber;
		GetComponent<BBMoveObjectsController>().startMoveChips();

		betsList.Clear();
		betsChipList.Clear();
		tempWonResult = 0;
   }
#endif

	void gotFinalNumber(string winNumber) {


#if USE_PHOTON

  if(BLab.Utility.BLabUtility.deepLog) Debug.Log(isMultiplayer + ") << isMultiplayer *** gotFinalNumber *** [BBRouletteController] gotNumber winNumber : " + winNumber + " isMaster : " + PhotonNetwork.isMasterClient);

  if(isMultiplayer) {
     if(PhotonNetwork.isMasterClient) {
		 pv.RPC("RPCShareWinnerNumber",PhotonTargets.Others,winNumber,GameObject.FindGameObjectWithTag("MPGameController").GetComponent<GameControllerMultiplayer>().rouletteBallMultiplayer.transform.position);
	  } else {
         return;
      }

  } else {

  }
#endif
		
		if(simulateNumber) {
		    winNumber = numberToSimulate;
//			GameObject.Find("InputFieldTestNumber").GetComponent<InputField>().text = numberToSimulate;
		}
		

		VoicesController.SendMessage("playVoice",winNumber,SendMessageOptions.DontRequireReceiver);

           if(autoSpin) {
			    Invoke("executeAutoSpin",autoSpinSeconds);
				InvokeRepeating("seconsCountDown",1,1);
		   }

			if(BLab.Utility.BLabUtility.deepLog) Debug.Log("[BBRouletteController] gotNumber winner : " + winNumber);

		setNumberOut(winNumber);
		checkResult(winNumber);
		betInProgress = false;
		//GameObject[] betChipList = GameObject.FindGameObjectsWithTag("betChip");
		//foreach(GameObject go in betChipList) Destroy(go);

		GetComponent<BBMoveObjectsController>().winnerNumber = winNumber;
		GetComponent<BBMoveObjectsController>().startMoveChips();

		betsList.Clear();
		betsChipList.Clear();
		tempWonResult = 0;
		
	}

	void gotMultiSelectionButton(GameObject _go) {

     switch(_go.name) {
			case "ButtonSerie_0_spiel":
#if USE_PHOTON
		      if(isMultiplayer) {
				pv.RPC("RPCcreate_0_spiel",PhotonTargets.All);
			  } else {
				StartCoroutine( create_0_spiel() );
			  }
#else
			    StartCoroutine( create_0_spiel() );
#endif
			break;
			case "ButtonSerie_0_2_3":
#if USE_PHOTON
                if(isMultiplayer) {
				   pv.RPC("RPCcreate_0_2_3",PhotonTargets.All);
				} else {
					StartCoroutine( create_0_2_3() );
				}
#else
				StartCoroutine( create_0_2_3() );
#endif
			break;
			case "ButtonOrphanel":
#if USE_PHOTON
				if(isMultiplayer) {
				    pv.RPC("RPCcreate_Orphelins",PhotonTargets.All);
				} else {
					StartCoroutine( create_Orphelins() );
				}
#else
				StartCoroutine( create_Orphelins() );
#endif
			break;
			case "ButtonSerie5_8":
#if USE_PHOTON
				if(isMultiplayer) {
				    pv.RPC("RPCcreate_serie_5_8",PhotonTargets.All);
				} else {
					StartCoroutine( create_serie_5_8() );
				}
#else
				StartCoroutine( create_serie_5_8() );
#endif
			break;
     }

			if(!isAmerican)  setMultiBetButtons(false);

	}

	void setMultiBetButtons(bool _enabled) {
			GameObject.Find("ButtonOrphanel").GetComponent<Button>().interactable = _enabled;
			GameObject.Find("ButtonOrphanel").GetComponent<BBUIButtonMessageForUUI>().enabled = _enabled;
			GameObject.Find("ButtonSerie5_8").GetComponent<Button>().interactable = _enabled;
			GameObject.Find("ButtonSerie5_8").GetComponent<BBUIButtonMessageForUUI>().enabled = _enabled;
			GameObject.Find("ButtonSerie_0_2_3").GetComponent<Button>().interactable = _enabled;
			GameObject.Find("ButtonSerie_0_2_3").GetComponent<BBUIButtonMessageForUUI>().enabled = _enabled;
			GameObject.Find("ButtonSerie_0_spiel").GetComponent<Button>().interactable = _enabled;
			GameObject.Find("ButtonSerie_0_spiel").GetComponent<BBUIButtonMessageForUUI>().enabled = _enabled;
			GameObject.Find("ButtonRepeatLastBet").GetComponent<Button>().interactable = _enabled;
			GameObject.Find("ButtonRepeatLastBet").GetComponent<BBUIButtonMessageForUUI>().enabled = _enabled;

	}

#if USE_PHOTON
 [PunRPC]
 void RPCcreate_serie_5_8() {
  StartCoroutine(create_serie_5_8());
 }
#endif

	IEnumerator create_serie_5_8() {
			//  5/8; 10/11; 13/16; 23/24; 27/30; 33/36.

			Transform pointToInstantiate = GameObject.Find("_multiBetSelectionPoint").transform;

			GameObject[] allBetPos = GameObject.FindGameObjectsWithTag("betObject");

			GameObject tmpChip = null;

			BBMoveObjectsController moveController = GetComponent<BBMoveObjectsController>();

			moveController.multiObjectMoveList.Clear();

			for(int x = 0;x < 6; x++) {

		 if(currentCash < 1) { 
			 TextRouletteMessage.text = "Ops... No Money to Bet!";TextRouletteMessage.color = Color.red;
		 } else {

				yield return new WaitForSeconds(0.15f);
			    pointToInstantiate.position = new Vector3(pointToInstantiate.position.x,pointToInstantiate.position.y,pointToInstantiate.position.z + UnityEngine.Random.Range(0.01f,0.05f));
			  switch(x) {
						case 0:
					        foreach(GameObject g in allBetPos) {if(g.name == "5_8") {
					                tmpChip = instatiateChip("5_8",pointToInstantiate,g); 
									BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
									mc.toMove = tmpChip.transform;
									mc.destination = g.transform;
									moveController.multiObjectMoveList.Add(mc);
					          break;}}
					    break;
						case 1:
							foreach(GameObject g in allBetPos) {if(g.name == "10_11") { 
							tmpChip = instatiateChip("10_11",pointToInstantiate,g); 
									BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
									mc.toMove = tmpChip.transform;
									mc.destination = g.transform;
									moveController.multiObjectMoveList.Add(mc);
							break;}}
					    break;
						case 2:
					        foreach(GameObject g in allBetPos) {if(g.name == "13_16") { 
							tmpChip = instatiateChip("13_16",pointToInstantiate,g); 
									BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
									mc.toMove = tmpChip.transform;
									mc.destination = g.transform;
									moveController.multiObjectMoveList.Add(mc);
							 break;}}
					    break;
						case 3:
							foreach(GameObject g in allBetPos) {if(g.name == "23_24") { 
							 tmpChip = instatiateChip("23_24",pointToInstantiate,g); 
									BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
									mc.toMove = tmpChip.transform;
									mc.destination = g.transform;
									moveController.multiObjectMoveList.Add(mc);
							 break;}}
					    break;
						case 4:
							foreach(GameObject g in allBetPos) {if(g.name == "27_30") { 
							tmpChip = instatiateChip("27_30",pointToInstantiate,g); 
									BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
									mc.toMove = tmpChip.transform;
									mc.destination = g.transform;
									moveController.multiObjectMoveList.Add(mc);
							break;}}
					    break;
				        case 5:
							foreach(GameObject g in allBetPos) {if(g.name == "33_36") { 
							tmpChip = instatiateChip("33_36",pointToInstantiate,g); 
									BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
									mc.toMove = tmpChip.transform;
									mc.destination = g.transform;
									moveController.multiObjectMoveList.Add(mc);
							break;}}
					    break;

			   }
			}

	   }

		 yield return new WaitForSeconds(2);

		 moveController.moveMultiObjectMoveList = true;

	}

#if USE_PHOTON
 [PunRPC]
 void RPCcreate_Orphelins() {
   StartCoroutine(create_Orphelins());
 }
#endif

	IEnumerator create_Orphelins() {
			//6/9; 14/17; 17/20  31/34 1

			Transform pointToInstantiate = GameObject.Find("_multiBetSelectionPoint").transform;

			GameObject[] allBetPos = GameObject.FindGameObjectsWithTag("betObject");

			GameObject tmpChip = null;

			BBMoveObjectsController moveController = GetComponent<BBMoveObjectsController>();

			moveController.multiObjectMoveList.Clear();

			for(int x = 0;x < 5; x++) {

		     if(currentCash < 1) { 
			 TextRouletteMessage.text = "Ops... No Money to Bet!";TextRouletteMessage.color = Color.red;
		     } else {

				yield return new WaitForSeconds(0.15f);
			    pointToInstantiate.position = new Vector3(pointToInstantiate.position.x,pointToInstantiate.position.y,pointToInstantiate.position.z + UnityEngine.Random.Range(0.01f,0.05f));
			  switch(x) {
						case 0:
					        foreach(GameObject g in allBetPos) {if(g.name == "6_9") {
					                tmpChip = instatiateChip("6_9",pointToInstantiate,g); 
									BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
									mc.toMove = tmpChip.transform;
									mc.destination = g.transform;
									moveController.multiObjectMoveList.Add(mc);
					          break;}}
					    break;
						case 1:
							foreach(GameObject g in allBetPos) {if(g.name == "14_17") { 
							tmpChip = instatiateChip("14_17",pointToInstantiate,g); 
									BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
									mc.toMove = tmpChip.transform;
									mc.destination = g.transform;
									moveController.multiObjectMoveList.Add(mc);
							break;}}
					    break;
						case 2:
					        foreach(GameObject g in allBetPos) {if(g.name == "17_20") { 
							tmpChip = instatiateChip("17_20",pointToInstantiate,g); 
									BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
									mc.toMove = tmpChip.transform;
									mc.destination = g.transform;
									moveController.multiObjectMoveList.Add(mc);
							 break;}}
					    break;
						case 3:
							foreach(GameObject g in allBetPos) {if(g.name == "31_34") { 
							 tmpChip = instatiateChip("31_34",pointToInstantiate,g); 
									BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
									mc.toMove = tmpChip.transform;
									mc.destination = g.transform;
									moveController.multiObjectMoveList.Add(mc);
							 break;}}
					    break;
						case 4:
							foreach(GameObject g in allBetPos) {if(g.name == "1") { 
							tmpChip = instatiateChip("1",pointToInstantiate,g); 
									BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
									mc.toMove = tmpChip.transform;
									mc.destination = g.transform;
									moveController.multiObjectMoveList.Add(mc);
							break;}}
					    break;
			   }
			}

		  }

			yield return new WaitForSeconds(2);

			moveController.moveMultiObjectMoveList = true;

	}

#if USE_PHOTON
 [PunRPC]
 void RPCcreate_0_2_3() {
	StartCoroutine(create_0_2_3());
 }
#endif


	IEnumerator create_0_2_3() {
			// _22,_18,_29,_7,_28,_12,_35,_3,_26,_0,_32,_15,_19,_4,_21,_2,_25

			Transform pointToInstantiate = GameObject.Find("_multiBetSelectionPoint").transform;

			GameObject[] allBetPos = GameObject.FindGameObjectsWithTag("betObject");

			GameObject tmpChip = null;

			BBMoveObjectsController moveController = GetComponent<BBMoveObjectsController>();

			moveController.multiObjectMoveList.Clear();

			for(int x = 0;x < 17; x++) {

			 if(currentCash < 1) { 
			  TextRouletteMessage.text = "Ops... No Money to Bet!";TextRouletteMessage.color = Color.red;
		     } else {

			  yield return new WaitForSeconds(0.15f);
			  pointToInstantiate.position = new Vector3(pointToInstantiate.position.x,pointToInstantiate.position.y,pointToInstantiate.position.z + UnityEngine.Random.Range(0.01f,0.05f));
			   switch(x) {
				case 16:
			        foreach(GameObject g in allBetPos) {if(g.name == "25") {
			                tmpChip = instatiateChip("25",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
			          break;}}
			    break;
				case 15:
			        foreach(GameObject g in allBetPos) {if(g.name == "2") {
			                tmpChip = instatiateChip("2",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
			          break;}}
			    break;
				case 14:
			        foreach(GameObject g in allBetPos) {if(g.name == "21") {
			                tmpChip = instatiateChip("21",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
			          break;}}
			    break;
				case 13:
			        foreach(GameObject g in allBetPos) {if(g.name == "4") {
			                tmpChip = instatiateChip("4",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
			          break;}}
			    break;
				case 12:
			        foreach(GameObject g in allBetPos) {if(g.name == "19") {
			                tmpChip = instatiateChip("19",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
			          break;}}
			    break;
				case 11:
			        foreach(GameObject g in allBetPos) {if(g.name == "28") {
			                tmpChip = instatiateChip("28",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
			          break;}}
			    break;
				case 10:
			        foreach(GameObject g in allBetPos) {if(g.name == "7") {
			                tmpChip = instatiateChip("7",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
			          break;}}
			    break;
				case 9:
			        foreach(GameObject g in allBetPos) {if(g.name == "29") {
			                tmpChip = instatiateChip("29",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
			          break;}}
			    break;
				case 8:
			        foreach(GameObject g in allBetPos) {if(g.name == "18") {
			                tmpChip = instatiateChip("18",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
			          break;}}
			    break;
				case 7:
			        foreach(GameObject g in allBetPos) {if(g.name == "22") {
			                tmpChip = instatiateChip("22",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
			          break;}}
			    break;
			    case 0:
			        foreach(GameObject g in allBetPos) {if(g.name == "12") {
			                tmpChip = instatiateChip("12",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
			          break;}}
			    break;
				case 1:
					foreach(GameObject g in allBetPos) {if(g.name == "35") { 
					tmpChip = instatiateChip("35",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
					break;}}
			    break;
				case 2:
					foreach(GameObject g in allBetPos) {if(g.name == "3") { 
					 tmpChip = instatiateChip("3",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
					 break;}}
			    break;
				case 3:
					foreach(GameObject g in allBetPos) {if(g.name == "26") { 
					 tmpChip = instatiateChip("26",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
					 break;}}
			    break;
				case 4:
					foreach(GameObject g in allBetPos) {if(g.name == "0") { 
					tmpChip = instatiateChip("0",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
					break;}}
			    break;
				case 5:
					foreach(GameObject g in allBetPos) {if(g.name == "32") { 
					tmpChip = instatiateChip("32",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
					break;}}
			    break;
				case 6:
					foreach(GameObject g in allBetPos) {if(g.name == "15") { 
					 tmpChip = instatiateChip("15",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
					 break;}}
			    break;

			   }
			  
			}
		 }
			yield return new WaitForSeconds(2);

			moveController.moveMultiObjectMoveList = true;

	}

#if USE_PHOTON
 [PunRPC]
 void RPCcreate_0_spiel() {
	StartCoroutine(create_0_spiel());
 }
#endif


	IEnumerator create_0_spiel() {
			// 12, 35, 3, 26, 0, 32, 15
			Transform pointToInstantiate = GameObject.Find("_multiBetSelectionPoint").transform;

			GameObject[] allBetPos = GameObject.FindGameObjectsWithTag("betObject");

			GameObject tmpChip = null;

			BBMoveObjectsController moveController = GetComponent<BBMoveObjectsController>();

			moveController.multiObjectMoveList.Clear();

			for(int x = 0;x < 7; x++) {

			 if(currentCash < 1) { 
			    TextRouletteMessage.text = "Ops... No Money to Bet!";TextRouletteMessage.color = Color.red;
		     } else {

			  yield return new WaitForSeconds(0.15f);
			  pointToInstantiate.position = new Vector3(pointToInstantiate.position.x,pointToInstantiate.position.y,pointToInstantiate.position.z + UnityEngine.Random.Range(0.01f,0.05f));
			   switch(x) {
			    case 0:
			        foreach(GameObject g in allBetPos) {if(g.name == "12") {
			                tmpChip = instatiateChip("12",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
			          break;}}
			    break;
				case 1:
					foreach(GameObject g in allBetPos) {if(g.name == "35") { 
					tmpChip = instatiateChip("35",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
					break;}}
			    break;
				case 2:
					foreach(GameObject g in allBetPos) {if(g.name == "3") { 
					 tmpChip = instatiateChip("3",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
					 break;}}
			    break;
				case 3:
					foreach(GameObject g in allBetPos) {if(g.name == "26") { 
					 tmpChip = instatiateChip("26",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
					 break;}}
			    break;
				case 4:
					foreach(GameObject g in allBetPos) {if(g.name == "0") { 
					tmpChip = instatiateChip("0",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
					break;}}
			    break;
				case 5:
					foreach(GameObject g in allBetPos) {if(g.name == "32") { 
					tmpChip = instatiateChip("32",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
					break;}}
			    break;
				case 6:
					foreach(GameObject g in allBetPos) {if(g.name == "15") { 
					 tmpChip = instatiateChip("15",pointToInstantiate,g); 
							BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
							mc.toMove = tmpChip.transform;
							mc.destination = g.transform;
							moveController.multiObjectMoveList.Add(mc);
					 break;}}
			    break;

			   }
			  
			}
		  }
			yield return new WaitForSeconds(2);

			moveController.moveMultiObjectMoveList = true;

	}

	GameObject instatiateChip(string numberName, Transform pos, GameObject betPosOwner) {

			                GameObject chip = Instantiate(Resources.Load("BetPositionChip"),pos.position,Quaternion.identity)  as GameObject;
			                chip.transform.Find("chipgreen").transform.localScale = chipTransformSize;//new Vector3(0.9f,0.3f,0.9f);
			                chip.name = numberName;
							chip.GetComponent<BBChipData>().betValue = selectedMoneyToBet;

			                if(selectedMoneyToBet == 500) { chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().redMat;}
							else if(selectedMoneyToBet == 1000) { chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().blueMat;}
			                else if(selectedMoneyToBet == 100) { chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().greenMat;}
			                else {
								chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().doubleMat;
								chip.GetComponent<BBChipData>().resultCanvas.SetActive(true);
				                chip.GetComponent<BBChipData>().resultCanvas.transform.Find("Canvas/ResultValue").GetComponent<Text>().text = selectedMoneyToBet.ToString();
							}
			                gotNumberEnterOnTableTrigger(betPosOwner,chip);
							currentCash -= selectedMoneyToBet; TextMyCash.text = String.Format("{0:0,0}", currentCash) + " $";
						    currentCashBetted += selectedMoneyToBet; TextCurrentMoneyBeted.text = String.Format("{0:0,0}", currentCashBetted) + " $";
						
						    playClipOneShot(clipPutChip);

						    return chip;

	}

#if USE_PHOTON
  [PunRPC]
  void RPCStartSpin() {
	   wheelRoulette.GetComponent<BBRotate>().startSpin();
	   betInProgress = true;
  }
#endif
										
	void gotButtonClick(GameObject _go) {
	  
	   switch(_go.name) {
		   case "ButtonRouletteSpin":
		   
			if(betsList.Count > 0) {
#if USE_PHOTON
                if(isMultiplayer) {
					pv.RPC("RPCStartSpin",PhotonTargets.All);
                } else {
					wheelRoulette.GetComponent<BBRotate>().startSpin();
				}
#else
			    wheelRoulette.GetComponent<BBRotate>().startSpin();
#endif
			    //float res = getPossibilyMaxMoneyWin();
				ButtonRouletteSpin.GetComponent<Button>().interactable = false;
				ButtonRouletteSpin.GetComponent<BBUIButtonMessageForUUI>().enabled = false;

	            //Button_EXIT.SetActive(false);		
	            playClipOneShot(clipTap);	
                saveLastBet();

                 if(!isAmerican) setMultiBetButtons(false);

					VoicesController.SendMessage("playVoice","rienneVaPlusJeuxFait",SendMessageOptions.DontRequireReceiver);
					betInProgress = true;

			} else {
			       TextRouletteMessage.text = "No bets found please bet before spin...";
				   TextRouletteMessage.color = Color.red;
			     }
			break;
			
			case "Button_EXIT":

#if USE_UNITY_ADV
		BLab.GetCoinsSystem.UnityAdvertisingController.ShowAdPlacement(BLab.GetCoinsSystem.UnityAdvertisingController.zoneIdVideo);
#endif

#if USE_PHOTON
           if(isMultiplayer) {
             PhotonNetwork.Disconnect();
           } else {
			 SceneManager.LoadSceneAsync(0);
           }
#else
			SceneManager.LoadSceneAsync(0);
#endif
			break;
			case "ButtonResetDB":
				break;
			case "ButtonViewBettingsPoints":
			  showBettingPoints(_go);
			  playClipOneShot(clipTap);
			break;
			case "ButtonOKOnPanelWonLose":
#if USE_UNITY_ADV
		BLab.GetCoinsSystem.UnityAdvertisingController.ShowAdPlacement(BLab.GetCoinsSystem.UnityAdvertisingController.zoneIdVideo);
#endif

			  SceneManager.LoadSceneAsync(0);
			break;	
			case "ButtonRepeatLastBet":
			  StartCoroutine( repeatLastbet() );
			break;
			case "ButtonOKCasinoCantAcceptBet":
				canUseClick = true;
				PanelCasinoNotEnoughCoins.SetActive(false);
			break;	
	   }
	
	}



	IEnumerator repeatLastbet() {

			Transform pointToInstantiate = GameObject.Find("_multiBetSelectionPoint").transform;

			GameObject tmpChip = null;

			BBMoveObjectsController moveController = GetComponent<BBMoveObjectsController>();

			moveController.multiObjectMoveList.Clear();

			GameObject[] allBetPos = GameObject.FindGameObjectsWithTag("betObject");


	              foreach(string s in lastbetComplete) {
	                yield return new WaitForSeconds(0.15f);
				    if(currentCash < 1) { 
			          TextRouletteMessage.text = "Ops... No Money to Bet!";TextRouletteMessage.color = Color.red;
		            } else {
					        foreach(GameObject g in allBetPos) {if(g.name == s) { 
								tmpChip = instatiateChip(g.name,pointToInstantiate,g); 
								BBMoveObjectsController.MultiObjectMove mc = new BBMoveObjectsController.MultiObjectMove();
								mc.toMove = tmpChip.transform;
								mc.destination = g.transform;
								moveController.multiObjectMoveList.Add(mc);
								break;
					         }
					       }
		            }
	              }


			yield return new WaitForSeconds(2);

			moveController.moveMultiObjectMoveList = true;

	}

	void saveLastBet() {
	    GameObject[] completeBet = GameObject.FindGameObjectsWithTag("betChip");
	    lastbetComplete.Clear();
	    foreach(GameObject g in completeBet) lastbetComplete.Add(g.name);

			if(BLab.Utility.BLabUtility.deepLog) foreach(string s in lastbetComplete) Debug.Log("SaveLastBet --> : " + s);

	}
	
	void viewBets() {
		Debug.Log("---------------------");
	  foreach(GameObject s in betsList) {
	    Debug.Log("--> : " + s.name);
	  }
		Debug.Log("---------------------");
		
	}
	
	void checkForResult() {
	
	  
		foreach(GameObject s in betsList) {
			Debug.Log("--> : " + s.name);
			
		}
		
	
	}
	
	void gotNumberEnterOnTableTrigger(GameObject num,GameObject chip) {

			if(BLab.Utility.BLabUtility.deepLog) Debug.Log("gotNumberEnterOnTableTrigger : " + currentCash);

			if(currentCash < 1) {
				TextRouletteMessage.text = "Ops... No Money to Bet!";
				TextRouletteMessage.color = Color.red;
				Destroy(chip);
				return;
			}

		betsList.Add(num);
		betsChipList.Add(chip);
	}

	void gotNumberExitOnTableTrigger(GameObject num,GameObject chip) {
		betsList.Remove(num);
		betsChipList.Remove(chip);
	}
	
	void setNumberOut(string number) {
	
	  string numColor = getNumberColor(number);
	  
			if(BLab.Utility.BLabUtility.deepLog) Debug.Log("numColor : " + numColor);

		string[] oldDataR = new string[5];
		for(int x = 0; x < numberOutRed.Length; x++) oldDataR[x] = numberOutRed[x].text;
		string[] oldDataN = new string[5];
		for(int x = 0; x < numberOutBlack.Length; x++) oldDataN[x] = numberOutBlack[x].text;
		
		if(numColor == "R" || numColor == "Z") {
		   if(number == "100") {
		    number = "00";
		   } 
		   numberOutRed[0].text = number;
		   numberOutBlack[0].text = "";
        }       		
  else if(numColor == "N") { 
			numberOutRed[0].text = "";
			numberOutBlack[0].text = number;
		}       		
		
		numberOutRed[1].text = oldDataR[0];
		numberOutRed[2].text = oldDataR[1];
		numberOutRed[3].text = oldDataR[2];
		numberOutRed[4].text = oldDataR[3];
		
		numberOutBlack[1].text = oldDataN[0];
		numberOutBlack[2].text = oldDataN[1];
		numberOutBlack[3].text = oldDataN[2];
		numberOutBlack[4].text = oldDataN[3];

		setStats(int.Parse(number));
	
	}

	public Image[] onBouleNumbers;
	public Image[] dozen_1;
	public Image[] dozen_2;
	public Image[] dozen_3;
	public Image[] column_1;
	public Image[] column_2;
	public Image[] column_3;
	public Text[] NumbersOnStats;


	void setStats(int number) {

			

			if(onBouleNumbers.Length == 0) onBouleNumbers = GameObject.Find("StatsPointsIndicators").GetComponentsInChildren<Image>();
			if(dozen_1.Length == 0) dozen_1 = GameObject.Find("dozen_1").GetComponentsInChildren<Image>();
			if(dozen_2.Length == 0) dozen_2 = GameObject.Find("dozen_2").GetComponentsInChildren<Image>();
			if(dozen_3.Length == 0) dozen_3 = GameObject.Find("dozen_3").GetComponentsInChildren<Image>();
			if(column_1.Length == 0) column_1 = GameObject.Find("column_1").GetComponentsInChildren<Image>();
			if(column_2.Length == 0) column_2 = GameObject.Find("column_2").GetComponentsInChildren<Image>();
			if(column_3.Length == 0) column_3 = GameObject.Find("column_3").GetComponentsInChildren<Image>();
			if(NumbersOnStats.Length == 0) NumbersOnStats = GameObject.Find("NumbersOnStats").GetComponentsInChildren<Text>();


			for(int x = 0;x < onBouleNumbers.Length;x++) {
				if(onBouleNumbers[x].gameObject.name == "ImageStatsNumber_" + number.ToString()) {
					onBouleNumbers[x].color = Color.green;
				 break;
				}
			}

			int doz = getDozen(number);
			int doztmpVal = 0;
			switch(doz) {
			 case 1:for(int x = 0;x < dozen_1.Length;x++) {if(dozen_1[x].color == Color.green) {} else {dozen_1[x].color = Color.green;
				doztmpVal = int.Parse( GameObject.Find("TextOnlyNumber_doz_1").GetComponent<Text>().text );
				doztmpVal++;
				GameObject.Find("TextOnlyNumber_doz_1").GetComponent<Text>().text = doztmpVal.ToString();
			 break;}}break;
			 case 2:for(int x = 0;x < dozen_2.Length;x++) {if(dozen_2[x].color == Color.green) {} else {dozen_2[x].color = Color.green;
				doztmpVal = int.Parse( GameObject.Find("TextOnlyNumber_doz_2").GetComponent<Text>().text );
				doztmpVal++;
				GameObject.Find("TextOnlyNumber_doz_2").GetComponent<Text>().text = doztmpVal.ToString();
			 break;}}break;
			 case 3:for(int x = 0;x < dozen_3.Length;x++) {if(dozen_3[x].color == Color.green) {} else {dozen_3[x].color = Color.green;
				doztmpVal = int.Parse( GameObject.Find("TextOnlyNumber_doz_3").GetComponent<Text>().text );
				doztmpVal++;
				GameObject.Find("TextOnlyNumber_doz_3").GetComponent<Text>().text = doztmpVal.ToString();
			 break;}}break;
			}

			int col = getColumn(number);
			int coltmpVal = 0;
			switch(col) {
			case 1:for(int x = 0;x < column_1.Length;x++) {if(column_1[x].color == Color.green) {} else {column_1[x].color = Color.green;
				coltmpVal = int.Parse( GameObject.Find("TextOnlyNumber_col_1").GetComponent<Text>().text );
				coltmpVal++;
				GameObject.Find("TextOnlyNumber_col_1").GetComponent<Text>().text = coltmpVal.ToString();
			break;}}break;
			case 2:for(int x = 0;x < column_2.Length;x++) {if(column_2[x].color == Color.green) {} else {column_2[x].color = Color.green;
				coltmpVal = int.Parse( GameObject.Find("TextOnlyNumber_col_2").GetComponent<Text>().text );
				coltmpVal++;
				GameObject.Find("TextOnlyNumber_col_2").GetComponent<Text>().text = coltmpVal.ToString();
			break;}}break;
			case 3:for(int x = 0;x < column_3.Length;x++) {if(column_3[x].color == Color.green) {} else {column_3[x].color = Color.green;
				coltmpVal = int.Parse( GameObject.Find("TextOnlyNumber_col_3").GetComponent<Text>().text );
				coltmpVal++;
				GameObject.Find("TextOnlyNumber_col_3").GetComponent<Text>().text = coltmpVal.ToString();
			break;}}break;
			}


			if(BLab.Utility.BLabUtility.deepLog) Debug.Log("extrattedNumbers.Count : " + extrattedNumbers.Count);

			if(extrattedNumbers.Count < 15) {
				extrattedNumbers.Add(number);
				for(int x = 0;x < NumbersOnStats.Length;x++) {
					if(NumbersOnStats[x].text == "n.a.") {
					   NumbersOnStats[x].text = number.ToString();
					   string c = getNumberColor(number.ToString());
					   switch(c) {
					    case "R": NumbersOnStats[x].alignment = TextAnchor.MiddleRight; NumbersOnStats[x].color = Color.red; break;
						case "N": NumbersOnStats[x].alignment = TextAnchor.MiddleLeft; NumbersOnStats[x].color = Color.black; break;
						case "Z": NumbersOnStats[x].alignment = TextAnchor.MiddleCenter; NumbersOnStats[x].color = Color.green; break;
					   }
					   break;
					}

				}
			} else {
			    extrattedNumbers.RemoveAt(0);
				extrattedNumbers.Add(number);
				foreach(int i in extrattedNumbers) 	Debug.Log("extrattedNumbers---> 12 ---> : " + i);
				for(int x = 0;x < NumbersOnStats.Length;x++) {
					NumbersOnStats[x].text = extrattedNumbers[x].ToString();
					string c = getNumberColor(extrattedNumbers[x].ToString());
					   switch(c) {
					    case "R": NumbersOnStats[x].alignment = TextAnchor.MiddleRight; NumbersOnStats[x].color = Color.red; break;
						case "N": NumbersOnStats[x].alignment = TextAnchor.MiddleLeft; NumbersOnStats[x].color = Color.black; break;
						case "Z": NumbersOnStats[x].alignment = TextAnchor.MiddleCenter; NumbersOnStats[x].color = Color.green; break;
					  }
				}
			}


			if(BLab.Utility.BLabUtility.deepLog) foreach(int i in extrattedNumbers) Debug.Log("extrattedNumbers------> : " + i);



	}

	int getColumn(int number) {
		int tmpRes = 0;
		 if( (number == 1) || (number == 4) || (number == 7) || (number == 10) || (number == 13) || (number == 16) || (number == 19) || (number == 22) || (number == 25) || (number == 28) || (number == 31) || (number == 34)) tmpRes = 1;
		 if( (number == 2) || (number == 5) || (number == 8) || (number == 11) || (number == 14) || (number == 17) || (number == 20) || (number == 23) || (number == 26) || (number == 29) || (number == 32) || (number == 35)) tmpRes = 2;
		 if( (number == 3) || (number == 6) || (number == 9) || (number == 12) || (number == 15) || (number == 18) || (number == 21) || (number == 24) || (number == 27) || (number == 30) || (number == 33) || (number == 36)) tmpRes = 3;
      return tmpRes;
	}

	int getDozen(int number) {
	   int tmpRes = 0;
	     if((number > 0) && (number < 13)) { tmpRes = 1; }
		 if((number > 12) && (number < 25)) { tmpRes = 2; }
		 if((number > 24) && (number < 37)) { tmpRes = 3; }
		 return tmpRes;
	}
	
	Button buttBet_100;
	Button buttBet_500;
	Button buttBet_1000;
	Button buttBet_CUSTOM;
	public GameObject PanelGetCustomValue;
	float selectedMoneyToBet = 100;
	
	void gotBetSelection(GameObject _go) {
	
	   Debug.Log(">>>>> gotBetSelection >>> : " + _go.name);

	   if(!buttBet_100) { buttBet_100 = GameObject.Find("ButtonBet_100").GetComponent<Button>(); 
	                      buttBet_500 = GameObject.Find("ButtonBet_500").GetComponent<Button>();
	                      buttBet_1000 = GameObject.Find("ButtonBet_1000").GetComponent<Button>();
				          buttBet_CUSTOM = GameObject.Find("buttBet_CUSTOM").GetComponent<Button>();
	                    }
	      
	      buttBet_100.GetComponent<Image>().color = Color.white; 
	      buttBet_500.GetComponent<Image>().color = Color.white; 
	      buttBet_1000.GetComponent<Image>().color = Color.white;	
	  
			if(_go.name == "ButtonBet_100") { 
			    selectedMoneyToBet = 100; buttBet_100.GetComponent<Image>().color = Color.yellow;
			 }
			else if(_go.name == "ButtonBet_500") { selectedMoneyToBet = 500; buttBet_500.GetComponent<Image>().color = Color.yellow;}
			else if(_go.name == "ButtonBet_1000") { selectedMoneyToBet = 1000; buttBet_1000.GetComponent<Image>().color = Color.yellow;}
			else if(_go.name == "buttBet_CUSTOM") {
			    buttBet_CUSTOM.GetComponent<Image>().color = Color.yellow;
				PanelGetCustomValue.SetActive(true);
			  }

#if USE_PHOTON
   if(isMultiplayer) {
	 pv.RPC("RPCgotBetSelection",PhotonTargets.Others, selectedMoneyToBet);
   }
#endif


	}

#if USE_PHOTON
  [PunRPC]
  void RPCgotBetSelection(float currBetValue) {
		selectedMoneyToBet = currBetValue;
  }

#endif

	public void gotButtonGetCustomValue(GameObject valueGO) {

	   string val = valueGO.GetComponentInChildren<InputField>().text;
	   selectedMoneyToBet = float.Parse(val);
	   valueGO.SetActive(false);

#if USE_PHOTON
   if(isMultiplayer) {
	 pv.RPC("RPCgotBetSelection",PhotonTargets.Others, selectedMoneyToBet);
   }
#endif


	}



	float getPossibilyMaxMoneyWin() {
	
	 float lastMaxPossibility = 0;
	 float[] maxList = new float[37];
	
	  GameObject[] betList = GameObject.FindGameObjectsWithTag("betChip");
	  
	  for(int x = 0; x < 37; x++) {
	     int lookFor = x;  
	       foreach(GameObject g in betList) {
			   string[] numbers = g.name.Split(new char[] { '_' });
				  int moltipli = getMultiplier(numbers.Length);
			      foreach(string s in numbers) {
			        if(s == x.ToString()) {
			             
			             if(x == lookFor) {
			                 float betval = g.GetComponent<BBChipData>().betValue;
			                 float maxOnThisBet = betval*moltipli;
			                 //Debug.Log("---> : " + s + " : " + moltipli + " : " + x + " : " + lookFor + " : " + betval + " : " + maxOnThisBet);
			                 maxList[x] += maxOnThisBet;
			             }
			             
			        }
			      }
	       }
	      
	  }
	  
		for(int x = 0; x < 37; x++) {
			if(maxList[x] > lastMaxPossibility) lastMaxPossibility = maxList[x];
//		  Debug.Log("****>> : [" + x + "] " + maxList[x]);
		}

		Debug.Log("*******************>> lastMaxPossibility: " + lastMaxPossibility);
		
									
	  return lastMaxPossibility;				
	}		
	
	int getMultiplier(int arrayLen) {
        int tmpRet = 0;
		  switch(arrayLen) {
			case 1: tmpRet =  35; break;
			case 2: tmpRet =  17; break;
			case 3: tmpRet =  11; break;
			case 4: tmpRet =  7; break;
			case 6: tmpRet =  5; break;
			case 12: tmpRet =  2; break;
			case 18: tmpRet =  1; break;
		  }
	  return tmpRet;
	}
	
	IEnumerator gotGameResult (float resValue, int chipMultiplier) {

		
		yield return new WaitForEndOfFrame();
		
		Debug.Log("*** GAME RESULT MULTIPLIER ------> : " + resValue + " real : " + (resValue - currentCashBetted) + " resCasino : " + tempWonResultForCasino);
		
		if(resValue == 0) {
			TextRouletteMessage.text = "Ops! Try Again... Tap Anywhere To Next Spin";
			TextRouletteMessage.color = Color.red;
			 if(isSinglePlayerMode) {
				GetComponent<SinglePlayerMoneyController>().gotMoneyResult(resValue,(resValue - currentCashBetted));
			 } else {
			 }
			
		} else {
			if(isSinglePlayerMode) {
			   GetComponent<SinglePlayerMoneyController>().gotMoneyResult(resValue,(resValue - currentCashBetted));
			} else {
			}
			
				TextRouletteMessage.text = "Won : " + String.Format("{0:0,0}", (resValue - currentCashBetted).ToString()) + " $" + " : Tap Anywhere To Next Spin";
			    TextRouletteMessage.color = Color.yellow;
		}

			
	}


	void setBetObjectView(bool wantShow) {
	  GameObject[] betObjList = GameObject.FindGameObjectsWithTag("betObject");
	  foreach(GameObject go in betObjList) {
	      go.GetComponent<MeshRenderer>().enabled = wantShow;
	  }
	}
	
	public IEnumerator hideBettingPoints(GameObject butt) {
	
	 yield return new WaitForSeconds(6);
	 
	 setBetObjectView(false);

     if(butt != null) butt.SetActive(true);
				
	
	}
	
	void showBettingPoints(GameObject go) {
       setBetObjectView(true);
       go.SetActive(false);	
       StartCoroutine(hideBettingPoints(go));	
	}

	void playClipOneShot(AudioClip clipToPlay) {
		gameObject.GetComponent<AudioSource>().PlayOneShot(clipToPlay);
    }

    string getNumberColor(string number) {
	
	 string tempRet = "";
	  
	     switch(number) {
			case "0": case "100": tempRet = "Z"; break;
		case "1": case "3": case "5": case "7": case "9": case "12": case "14": case "16": case "18": case "19": case "21": case "23": case "25": case "27": case "30": case "32": case "34": case "36":
	         tempRet = "R";
	         break; 
		case "2": case "4": case "6": case "8": case "10": case "11": case "13": case "15": case "17": case "20": case "22": case "24": case "26": case "28": case "29": case "31": case "33": case "35":
			  tempRet = "N";
			  break;
	     }
	
	  return tempRet;
	
	}


  #if USE_PHOTON
  List<GameObject> allBetObjects = new List<GameObject>(); 
  [PunRPC]
  public void instantiateNewChip(string hitName) {

     if(allBetObjects.Count < 2) {
       allBetObjects = GameObject.FindGameObjectsWithTag("betObject").ToList();
     }

     GameObject g = allBetObjects.Find(d => d.name == hitName);
     if(g == null) return;

     Transform hitTransform = g.transform;

			            if(currentCash >= selectedMoneyToBet) {
				            GameObject chip = Instantiate(Resources.Load("BetPositionChip"),hitTransform.position,Quaternion.identity)  as GameObject;
				            chip.name = hitTransform.gameObject.name; // + "#" + playerPositionOnTable.ToString();
							chip.GetComponent<BBChipData>().betValue = selectedMoneyToBet;
							chip.transform.Find("chipgreen").transform.localScale = chipTransformSize;//new Vector3(0.9f,0.3f,0.9f);

							if(selectedMoneyToBet == 500) { chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().redMat;}
							else if(selectedMoneyToBet == 1000) { chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().blueMat;}
			                else if(selectedMoneyToBet == 100) { chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().greenMat;}
							else {
								chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().doubleMat;
								chip.GetComponent<BBChipData>().resultCanvas.SetActive(true);
								chip.GetComponent<BBChipData>().resultCanvas.transform.Find("Canvas/ResultValue").GetComponent<Text>().text = selectedMoneyToBet.ToString();
							}

				            gotNumberEnterOnTableTrigger(hitTransform.gameObject,chip);
							currentCash -= selectedMoneyToBet; TextMyCash.text = String.Format("{0:0,0}", currentCash) + " $";
						    currentCashBetted += selectedMoneyToBet; TextCurrentMoneyBeted.text = String.Format("{0:0,0}", currentCashBetted) + " $";
						
						    playClipOneShot(clipPutChip);
							
						} else {
						  TextRouletteMessage.text = "Ops... No Money to Bet!";
						}

  #else
		public void instantiateNewChip(RaycastHit hit) {

		                if(currentCash >= selectedMoneyToBet) {
							GameObject chip = Instantiate(Resources.Load("BetPositionChip"),hit.collider.transform.position,Quaternion.identity)  as GameObject;
							chip.name = hit.transform.gameObject.name; // + "#" + playerPositionOnTable.ToString();
							chip.GetComponent<BBChipData>().betValue = selectedMoneyToBet;
							chip.transform.Find("chipgreen").transform.localScale = chipTransformSize;//new Vector3(0.9f,0.3f,0.9f);

							if(selectedMoneyToBet == 500) { chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().redMat;}
							else if(selectedMoneyToBet == 1000) { chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().blueMat;}
			                else if(selectedMoneyToBet == 100) { chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().greenMat;}
							else {
								chip.GetComponent<BBChipData>().meshRend.material = chip.GetComponent<BBChipData>().doubleMat;
								chip.GetComponent<BBChipData>().resultCanvas.SetActive(true);
								chip.GetComponent<BBChipData>().resultCanvas.transform.Find("Canvas/ResultValue").GetComponent<Text>().text = selectedMoneyToBet.ToString();
							}

							gotNumberEnterOnTableTrigger(hit.transform.gameObject,chip);
							currentCash -= selectedMoneyToBet; TextMyCash.text = String.Format("{0:0,0}", currentCash) + " $";
						    currentCashBetted += selectedMoneyToBet; TextCurrentMoneyBeted.text = String.Format("{0:0,0}", currentCashBetted) + " $";
						
						    playClipOneShot(clipPutChip);
							
						} else {
						  TextRouletteMessage.text = "Ops... No Money to Bet!";
						}

  #endif


  }

  public void removeChip(RaycastHit hit) {

			        currentCash += hit.collider.gameObject.GetComponent<BBChipData>().betValue; TextMyCash.text = String.Format("{0:0,0}", currentCash) + " $";
					currentCashBetted -= hit.collider.gameObject.GetComponent<BBChipData>().betValue; TextCurrentMoneyBeted.text = String.Format("{0:0,0}", currentCashBetted) + " $";
					gotNumberExitOnTableTrigger(hit.transform.gameObject,hit.collider.gameObject);
					Destroy(hit.collider.gameObject);
					playClipOneShot(clipPutChip);

  }

  bool canUseClick = true;
  public void enterMaskToDisableClick() {
	 canUseClick = false;
  }

  public void exitMaskToDisableClick() {
	 canUseClick = true;
  }
    		    		
}
}


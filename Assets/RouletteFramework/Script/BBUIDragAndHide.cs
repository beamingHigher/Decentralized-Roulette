using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BLabRouletteProject {

[RequireComponent (typeof(AudioSource))]

public class BBUIDragAndHide : MonoBehaviour {

    public enum MoveMode {RightToLeft,LeftToRight,TopToDown,BottonToUp};
    public MoveMode moveMode;

    public Transform menuBlockerOpen;
	public Transform menuBlockerClose;
	public Transform buttonActivate;
	public float moveSpeed = 10;
	public bool close = false;
	public bool open = false;

	public bool isOpen = false;
	public bool isMoveing = false;

	public GameObject[] othersPanelToHide;

	Vector3 currentVectorDirectionOnOpen = Vector3.zero;
	Vector3 currentVectorDirectionOnClose = Vector3.zero;

	public AudioClip tapClip;
	public AudioClip dragClip;

	float WValue;
	float HValue;

	public Camera cameraToUse;

	public bool openAtStart = false;

		public enum MoveListClipsOpen {chatView,heliDashBoardOpen,PanelSettingsCarRaceOpen,PanelGameContainerCarOpen}
		public enum MoveListClipsClose {chatViewClose,HeliDashBoardClose,PanelSettingsCarRaceClose,PanelGameContainerCarClose}

	public MoveListClipsOpen moveListClipsOpen;
	public MoveListClipsClose moveListClipsClose;

	private string currentAniOpen = "";
	private string currentAniClose = "";

	public bool useAnimations = false;



	void Awake() {

	    gameObject.AddComponent<AudioSource>();
		if(!tapClip) tapClip = Resources.Load("tap") as AudioClip;
		if(!dragClip) dragClip = Resources.Load("drag") as AudioClip;

		if(!cameraToUse) cameraToUse = Camera.main;

#if MOBILE_INPUT
			moveSpeed = 10;//BBGeneralUtilityStaticData.menuHideMoveSpeed * 2;
#else
		    moveSpeed = 5;//BBGeneralUtilityStaticData.menuHideMoveSpeed;
#endif
	}

	// Use this for initialization
	void Start () {

	  if(useAnimations) {

	           switch(moveListClipsOpen) {
	            case MoveListClipsOpen.chatView: currentAniOpen = moveListClipsOpen.ToString();break;
				case MoveListClipsOpen.heliDashBoardOpen: currentAniOpen = moveListClipsOpen.ToString();break;
				case MoveListClipsOpen.PanelSettingsCarRaceOpen: currentAniOpen = moveListClipsOpen.ToString();break;
				case MoveListClipsOpen.PanelGameContainerCarOpen: currentAniOpen = moveListClipsOpen.ToString();break;
	           }
				switch(moveListClipsClose) {
	            case MoveListClipsClose.chatViewClose: currentAniClose = moveListClipsClose.ToString();break;
				case MoveListClipsClose.HeliDashBoardClose: currentAniClose = moveListClipsClose.ToString();break;
				case MoveListClipsClose.PanelSettingsCarRaceClose: currentAniClose = moveListClipsClose.ToString();break;
				case MoveListClipsClose.PanelGameContainerCarClose: currentAniClose = moveListClipsClose.ToString();break;
	           }

				if(openAtStart) {
			        gotButtonController();
			    }

	  } else {
				if(!cameraToUse) cameraToUse = Camera.main;

			    WValue = GetComponent<RectTransform>().sizeDelta.x;
				HValue = GetComponent<RectTransform>().sizeDelta.y;

			    switch(moveMode) {
				case MoveMode.BottonToUp: 
				        currentVectorDirectionOnOpen = Vector3.up; 
				        currentVectorDirectionOnClose = Vector3.down; 
				        break;
				case MoveMode.TopToDown: currentVectorDirectionOnOpen = Vector3.down; currentVectorDirectionOnClose = Vector3.up; break;
				case MoveMode.RightToLeft: currentVectorDirectionOnOpen = Vector3.left; currentVectorDirectionOnClose = Vector3.right; break;
				case MoveMode.LeftToRight: currentVectorDirectionOnOpen = Vector3.right; currentVectorDirectionOnClose = Vector3.left; break;
			    }

			    setPositionsController();

			    if(openAtStart) {
			        gotButtonController();
			    }
		     }
	}

	void closeOthersPanels() {
	  foreach(GameObject g in othersPanelToHide) {
	     if(g) {
		    BBUIDragAndHide dh = g.GetComponentInChildren<BBUIDragAndHide>();
		       if(dh)
		        if(dh.isOpen) {
		          dh.gotButtonController();  
		          dh.gameObject.transform.SetAsLastSibling();

		        }
		 }
	  }
	}

	public void gotButtonController() {

       if(useAnimations) {

				gameObject.BroadcastMessage("resetAlertCol",SendMessageOptions.DontRequireReceiver);

			  if(!isOpen) {
			        isOpen = true;
			        GetComponent<Animator>().Play(currentAniOpen);
					closeOthersPanels();
			  } else {
			        isOpen = false;
				    GetComponent<Animator>().Play(currentAniClose);
			  }

       } else {
			  if(isMoveing) return;

			  setPositionsController();

			        gameObject.BroadcastMessage("resetAlertCol",SendMessageOptions.DontRequireReceiver);

					AudioSource _as = GetComponent<AudioSource>();

			         if(_as) _as.PlayOneShot(tapClip);

			  if(!isOpen) {
			    open = true;
			    closeOthersPanels();
			  } else {
			    close = true;
			  }

			  StartCoroutine(playDrag());
      }
	}

	IEnumerator playDrag() {
	   yield return new WaitForSeconds(1);
	   AudioSource _as = GetComponent<AudioSource>();
	   if(_as) _as.PlayOneShot(dragClip);
	}

	int posMoveCount = 0;

	void moveCounter () {
		posMoveCount++;
	}

	void FixedUpdate () {

	if(useAnimations) return;

		//	Debug.Log(cameraToUse.WorldToViewportPoint(transform.position));

	    if(open) {
		           switch(moveMode) {
		            case MoveMode.LeftToRight:
						if(transform.localPosition.x + (WValue/2) < menuBlockerOpen.transform.localPosition.x) {
							transform.Translate( currentVectorDirectionOnOpen * (Time.deltaTime * moveSpeed),cameraToUse.transform);
						    isMoveing = true;
						} else {
							open = false;
							isOpen = true;
							isMoveing = false;
						}
		            break;
					case MoveMode.RightToLeft:
						if(transform.localPosition.x + (WValue/2) > menuBlockerOpen.transform.localPosition.x) {
							transform.Translate( currentVectorDirectionOnOpen * (Time.deltaTime * moveSpeed),cameraToUse.transform);
						    isMoveing = true;
						} else {
							open = false;
							isOpen = true;
							isMoveing = false;
						}
		            break;
					case MoveMode.TopToDown:
						if(transform.localPosition.y + (HValue/2) > menuBlockerOpen.transform.localPosition.y) {
							transform.Translate( currentVectorDirectionOnOpen * (Time.deltaTime * moveSpeed),cameraToUse.transform);
						    isMoveing = true;
						} else {
							open = false;
							isOpen = true;
							isMoveing = false;
						}
		            break;
					case MoveMode.BottonToUp:
						if(transform.localPosition.y + (HValue/2) < menuBlockerOpen.transform.localPosition.y) {
							transform.Translate( currentVectorDirectionOnOpen * (Time.deltaTime * moveSpeed),cameraToUse.transform);
						    isMoveing = true;
						} else {
							open = false;
							isOpen = true;
							isMoveing = false;
						}
		            break;
		           }
   	    }

		if(close) {
				  switch(moveMode) {
				   case MoveMode.LeftToRight:
						if(transform.localPosition.x - (WValue/2) > menuBlockerClose.transform.localPosition.x) {
							transform.Translate(currentVectorDirectionOnClose * (Time.deltaTime * moveSpeed),cameraToUse.transform);
					    isMoveing = true;
					} else {
						close = false;
						isOpen = false;
						isMoveing = false;
					}
				   break;
					case MoveMode.RightToLeft:
						if(transform.localPosition.x - (WValue/2) < menuBlockerClose.transform.localPosition.x) {
							transform.Translate(currentVectorDirectionOnClose * (Time.deltaTime * moveSpeed),cameraToUse.transform);
					    isMoveing = true;
					} else {
						close = false;
						isOpen = false;
						isMoveing = false;
					}
				   break;
					case MoveMode.TopToDown:
						if(transform.localPosition.y - (HValue/2) < menuBlockerClose.transform.localPosition.y) {
							transform.Translate(currentVectorDirectionOnClose * (Time.deltaTime * moveSpeed),cameraToUse.transform);
					    isMoveing = true;
					} else {
						close = false;
						isOpen = false;
						isMoveing = false;
					}
				   break;
					case MoveMode.BottonToUp:
						if(transform.localPosition.y - (HValue/2) > menuBlockerClose.transform.localPosition.y) {
							transform.Translate(currentVectorDirectionOnClose * (Time.deltaTime * moveSpeed),cameraToUse.transform);
					    isMoveing = true;
					} else {
						close = false;
						isOpen = false;
						isMoveing = false;
					}
				   break;
				  }
		    }
	
	}

	void setPositionsController() {

		if(moveMode == MoveMode.BottonToUp) {
						float pos_y = GetComponent<RectTransform>().localPosition.y;
						float H = HValue;
						float moveRange = H + Mathf.Abs(pos_y);
						moveRange = moveRange - buttonActivate.GetComponent<RectTransform>().sizeDelta.y;
						float RealMove = moveRange - Mathf.Abs(pos_y);

					    menuBlockerOpen.GetComponent<RectTransform>().localPosition = new Vector3(
					    menuBlockerOpen.GetComponent<RectTransform>().localPosition.x,
						menuBlockerOpen.GetComponent<RectTransform>().localPosition.y + (RealMove + buttonActivate.GetComponent<RectTransform>().sizeDelta.y),
						menuBlockerOpen.GetComponent<RectTransform>().localPosition.z);

					    menuBlockerClose.GetComponent<RectTransform>().localPosition = new Vector3(
						menuBlockerClose.GetComponent<RectTransform>().localPosition.x,
						menuBlockerClose.GetComponent<RectTransform>().localPosition.y - (RealMove + (buttonActivate.GetComponent<RectTransform>().sizeDelta.y)), // + buttonActivate.GetComponent<RectTransform>().sizeDelta.y),
						menuBlockerClose.GetComponent<RectTransform>().localPosition.z);
				}
		  else if(moveMode == MoveMode.TopToDown) {

					    menuBlockerOpen.GetComponent<RectTransform>().localPosition = new Vector3(
					    menuBlockerOpen.GetComponent<RectTransform>().localPosition.x,
						menuBlockerOpen.GetComponent<RectTransform>().localPosition.y,
						menuBlockerOpen.GetComponent<RectTransform>().localPosition.z);

					    menuBlockerClose.GetComponent<RectTransform>().localPosition = new Vector3(
						menuBlockerClose.GetComponent<RectTransform>().localPosition.x,
						menuBlockerClose.GetComponent<RectTransform>().localPosition.y,
						menuBlockerClose.GetComponent<RectTransform>().localPosition.z);
				} 
		  else if(moveMode == MoveMode.LeftToRight) {
 /*
						float pos_x = GetComponent<RectTransform>().localPosition.x;
						float W = WValue;
						float moveRange = W + Mathf.Abs(pos_x);
						moveRange = moveRange - buttonActivate.GetComponent<RectTransform>().sizeDelta.x;
						float RealMove = moveRange - Mathf.Abs(pos_x);

					    menuBlockerOpen.GetComponent<RectTransform>().localPosition = new Vector3(
				        menuBlockerOpen.GetComponent<RectTransform>().localPosition.x + (RealMove + buttonActivate.GetComponent<RectTransform>().sizeDelta.x),
						menuBlockerOpen.GetComponent<RectTransform>().localPosition.y,
						menuBlockerOpen.GetComponent<RectTransform>().localPosition.z);

					    menuBlockerClose.GetComponent<RectTransform>().localPosition = new Vector3(
				        menuBlockerClose.GetComponent<RectTransform>().localPosition.x - (RealMove + buttonActivate.GetComponent<RectTransform>().sizeDelta.x),
						menuBlockerClose.GetComponent<RectTransform>().localPosition.y,
						menuBlockerClose.GetComponent<RectTransform>().localPosition.z);
*/
				} 
		 else if(moveMode == MoveMode.RightToLeft) {

					    menuBlockerOpen.GetComponent<RectTransform>().localPosition = new Vector3(
				        menuBlockerOpen.GetComponent<RectTransform>().localPosition.x,
						menuBlockerOpen.GetComponent<RectTransform>().localPosition.y,
						menuBlockerOpen.GetComponent<RectTransform>().localPosition.z);

					    menuBlockerClose.GetComponent<RectTransform>().localPosition = new Vector3(
				        menuBlockerClose.GetComponent<RectTransform>().localPosition.x,
						menuBlockerClose.GetComponent<RectTransform>().localPosition.y,
						menuBlockerClose.GetComponent<RectTransform>().localPosition.z);
				} 

	}
}
}
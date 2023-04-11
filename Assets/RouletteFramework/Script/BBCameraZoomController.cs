using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BLabRouletteProject {

public class BBCameraZoomController : MonoBehaviour {

  public bool isRouletteProfessionalModel = false;

   private bool canMove = false;
   private bool moveDX = false;
   private bool isZoomed = false;
   private Camera cam;
   public float orthoSize = 12;
   public float orthoSizeZoomed = 6;

   public float orthoSizeIPAD = 12;
   public float orthoSizeZoomedIPAD = 6;


   private float startingPosX = 0;
   public float maxPosOnDX = 17;
   public float maxPosOnSX = -8;

	private GameObject Button_MoveCameraDX;
	private GameObject Button_MoveCameraSX;

	public bool simulateIPAD = false;

	// Use this for initialization
	void Start () {

       if(!isRouletteProfessionalModel) {
        orthoSize = 16;
       }

			cam = GetComponent<Camera>();

			if(SystemInfo.deviceModel.Contains("iPad") || simulateIPAD) {
				cam.orthographicSize = orthoSizeIPAD;
			} else {  
			    cam.orthographicSize = orthoSize;
		    }

			  startingPosX = transform.position.x;
			  Button_MoveCameraDX = GameObject.Find("Button_MoveCameraDX");
			  Button_MoveCameraSX = GameObject.Find("Button_MoveCameraSX");
		      setButtons(false);

	}

	void setButtons(bool _active) {
		Button_MoveCameraDX.GetComponent<Button>().interactable = _active;
		Button_MoveCameraSX.GetComponent<Button>().interactable = _active;

		BBUIButtonMessageForUUI[] bmSx = Button_MoveCameraSX.GetComponents<BBUIButtonMessageForUUI>();
		BBUIButtonMessageForUUI[] bmDx = Button_MoveCameraDX.GetComponents<BBUIButtonMessageForUUI>();
		foreach(BBUIButtonMessageForUUI b in bmSx) b.enabled = _active;
		foreach(BBUIButtonMessageForUUI b in bmDx) b.enabled = _active;
	}

	// Update is called once per frame
	void LateUpdate () {

	   if(canMove) {

           if(moveDX) {
             if(transform.position.x < maxPosOnDX) {
				 float xPos = transform.position.x+0.1f;
                 transform.position = new Vector3(xPos,transform.position.y,transform.position.z);
             }
           }
		   if(!moveDX) {
			   if(transform.position.x > maxPosOnSX) {
				  float xPos = transform.position.x-0.1f;
                  transform.position = new Vector3(xPos,transform.position.y,transform.position.z);
               }
           }

	   }


	}

	void gotZoomButton() {
	  isZoomed = !isZoomed;

	    if(isZoomed) {
				if(SystemInfo.deviceModel.Contains("iPad") || simulateIPAD) {
					cam.orthographicSize = orthoSizeZoomedIPAD;
			    } else {
					cam.orthographicSize = orthoSizeZoomed;
			    }  
	      setButtons(true);
	    } else {
				if(SystemInfo.deviceModel.Contains("iPad") || simulateIPAD) {
					cam.orthographicSize = orthoSizeIPAD;
			    } else {
					cam.orthographicSize = orthoSize;
			    }
			      
		     transform.position = new Vector3(startingPosX,transform.position.y,transform.position.z);
		     setButtons(false);
	    }

	}

	void gotButtonMoveDXDown() {
	 canMove = true;
      moveDX = true;
	}

	void gotButtonMoveDXUp() {
	  canMove = false;
	}

	void gotButtonMoveSXDown() {
	  canMove = true;
	  moveDX = false;
	}

	void gotButtonMoveSXUp() {
		canMove = false;
	}

}
}
using UnityEngine;
using System.Collections;

namespace BLabRouletteProject {

public class BBBallSetting : MonoBehaviour {

  public Transform _pivot;
  public float speed = 100;
  public bool forceRotate = true;
  public bool isMultiplayer = false;

  private bool canMove = true;

#if USE_PHOTON
  private Vector3 correctPlayerPos = Vector3.zero; // We lerp towards this
  private Quaternion correctPlayerRot = Quaternion.identity; // We lerp towards this

  PhotonView pv;
#endif

   void OnEnable() {
		speed = 200;
		forceRotate = true;
		_pivot = GameObject.Find("ballPivot").transform;	
	    speed = UnityEngine.Random.Range(180.0f,220.0f);
	    canMove = true;
   }


	// Use this for initialization
	void Start () {
#if USE_PHOTON
   if(isMultiplayer) {
       pv = GetComponent<PhotonView>();
   }
#endif

	 //_pivot = GameObject.Find("ballPivot").transform;	
	 
	 //speed = UnityEngine.Random.Range(180.0f,220.0f);
				
	}
	
	// Update is called once per frame
	void FixedUpdate () {

	  if(!canMove) return;

#if USE_PHOTON

         if(isMultiplayer) {

				if (!pv.isMine)
		        {
		            transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
		            transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
		        } else {

						if(forceRotate) transform.RotateAround(_pivot.position, Vector3.up, speed * Time.deltaTime);
						
						if(speed > 0)  speed -= 0.15f;
						else forceRotate = false;
					   
						if(GetComponent<Rigidbody>().velocity.magnitude < 0.1f && !forceRotate) {
						
							GameObject.Find("wheelRoulette").SendMessage("gotBallStopped",SendMessageOptions.DontRequireReceiver);
				             if(isMultiplayer) {
				               canMove = false;
				             } else {
				               this.enabled = false;	
				             }
						}
				}

		 } else {
				if(forceRotate) transform.RotateAround(_pivot.position, Vector3.up, speed * Time.deltaTime);
						
						if(speed > 0)  speed -= 0.15f;
						else forceRotate = false;
					   
						if(GetComponent<Rigidbody>().velocity.magnitude < 0.1f && !forceRotate) {
						
							GameObject.Find("wheelRoulette").SendMessage("gotBallStopped",SendMessageOptions.DontRequireReceiver);
				             if(isMultiplayer) {
				               canMove = false;
				             } else {
				               this.enabled = false;	
				             }
						}
		 }

#else
			    if(forceRotate) transform.RotateAround(_pivot.position, Vector3.up, speed * Time.deltaTime);
				
				if(speed > 0)  speed -= 0.15f;
				else forceRotate = false;
			   
				if(GetComponent<Rigidbody>().velocity.magnitude < 0.1f && !forceRotate) {
				
					GameObject.Find("wheelRoulette").SendMessage("gotBallStopped",SendMessageOptions.DontRequireReceiver);
		             if(isMultiplayer) {
		               canMove = false;
		             } else {
		               this.enabled = false;	
		             }
				}

#endif

	}


#if USE_PHOTON
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

		if(!canMove) return;

		if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Network player, receive data
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
        }
	}
#endif

}
}
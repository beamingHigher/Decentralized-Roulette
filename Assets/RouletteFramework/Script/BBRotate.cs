using UnityEngine;
using System.Collections;

namespace BLabRouletteProject {

public class BBRotate : MonoBehaviour {

	public Vector3 rotationVector;
	public float speed = 10;
	
	public Transform _wheels;
	public bool canRotate = true;
	public bool reduceSpeed = false;
	
	public AudioClip rollingBall;
	public AudioClip rollingEndBall;
	
	bool endClipended = false;
	string winningNumber = "";
	
	public GameObject rouletteController;
	
	public bool automateSpin = false;
	
	public Transform ballSpawnPoint;

	public GameControllerMultiplayer gameControllerMultiplayer;

	public bool isMultiplayer = false;
	
	void autoSpin() {
	 startSpin();
	}
   
	// Use this for initialization
	void Awake() {
	  canRotate = false;
	  if(automateSpin) InvokeRepeating("autoSpin",1,60);
	}
	
	void realStart () {
	
	 speed = UnityEngine.Random.Range(140.0f,220.0f);
	
		GameObject ball = GameObject.Find("rouletteBall(Clone)");
		if(ball) Destroy(ball);

			GameObject _ball = null;
#if USE_PHOTON
        if(isMultiplayer) {
			_ball = gameControllerMultiplayer.rouletteBallMultiplayer; //Instantiate(Resources.Load("rouletteBallMultiplayer"),ballSpawnPoint.position,Quaternion.identity) as GameObject;//, transform.FindChild("ballSpawnPoint").position, transform.FindChild("ballSpawnPoint").rotation);   
			_ball.SetActive(false);
			_ball.transform.position = ballSpawnPoint.position;
			_ball.SetActive(true);
			canRotate = true;
		} else {
			_ball = Instantiate(Resources.Load("rouletteBall"),ballSpawnPoint.position,Quaternion.identity) as GameObject;//, transform.FindChild("ballSpawnPoint").position, transform.FindChild("ballSpawnPoint").rotation);   
		}
#else
	   _ball = Instantiate(Resources.Load("rouletteBall"),ballSpawnPoint.position,Quaternion.identity) as GameObject;//, transform.FindChild("ballSpawnPoint").position, transform.FindChild("ballSpawnPoint").rotation);   
#endif
	   _ball.GetComponent<Rigidbody>().mass = UnityEngine.Random.Range(3.0f,6.0f); 

	 GetComponent<AudioSource>().clip = rollingBall;
	 GetComponent<AudioSource>().loop = true;
	 GetComponent<AudioSource>().Play();
	 
		if(!rouletteController) rouletteController = GameObject.Find("_rouletteController");
	 
	    canRotate = true;
	}
	
	void decreaseSpeed() {
	  speed -= 1;
	}
	
	void sendWinnerNumber() {
		
	}
	
	void gotBallStopped() {
		rouletteController.SendMessage("gotFinalNumber",winningNumber, SendMessageOptions.DontRequireReceiver);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	   if(canRotate) {
			_wheels.Rotate(rotationVector * (speed * Time.deltaTime));
	
			if(reduceSpeed) {
				
				 if(speed > -2) {
					//GetComponent<AudioSource>().Stop();  
					GetComponent<AudioSource>().loop = false;
					GetComponent<AudioSource>().clip = rollingEndBall;
					 if(!endClipended) {
					   if(!GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Play();
						//Invoke("sendWinnerNumber",4);
						endClipended = true;
					    resetStarting();
					 }
					
				 } else {
					speed += 0.5f;
				 }
				  
			} else { 		
				speed--;
				
				if(speed < -300) {
				   reduceSpeed = true;
				}
			}
			
	   }
	}
	
	void gotNumberTrigger(string num) {
	    winningNumber = num;
		//Debug.Log("gotNumberTrigger : " + num);
	
	}
	
	public void startSpin() {
	  realStart();
	}
	
	void resetStarting() {
	  canRotate = false;
	  speed = 200;
	  endClipended = false;
	  reduceSpeed = false;
	}
	
	
}
}
using UnityEngine;
using System.Collections;

namespace BLabRouletteProject {

public class BBRouletteNumberTrigger : MonoBehaviour {

	public GameObject rootGameObject;
	public bool isOnTableNumber = false;
	
	public GameObject RouletteControllerGO;
	
	
	
	void Start() {
		RouletteControllerGO = GameObject.Find("_rouletteController");
	}
	

	void OnTriggerStay(Collider other) {
		
	    if(isOnTableNumber) {
			
	    } else {	
		   rootGameObject.SendMessage("gotNumberTrigger",gameObject.name,SendMessageOptions.DontRequireReceiver);
	    }	
	}
	
	void OnTriggerEnter(Collider other) {
		
		if(isOnTableNumber) RouletteControllerGO.SendMessage("gotNumberEnterOnTableTrigger",gameObject.name,SendMessageOptions.DontRequireReceiver);
		
	}
	
	void OnTriggerExit(Collider other) {
		
		if(isOnTableNumber) RouletteControllerGO.SendMessage("gotNumberExitOnTableTrigger",gameObject.name,SendMessageOptions.DontRequireReceiver);
		
		
	}
	
	
}
}
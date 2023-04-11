using UnityEngine;
using System.Collections;

namespace BLabRouletteProject {

public class BBVoicesController : MonoBehaviour {

	bool isAmerican = false;

 IEnumerator Start() {
   yield return new WaitForSeconds(3);

   isAmerican = GameObject.FindGameObjectWithTag("GameController").GetComponent<BBRouletteController>().isAmerican;

 }

 void playVoice(string voice) {

     if(isAmerican) {
		GetComponent<AudioSource>().PlayOneShot( (Resources.Load("USAVoices/" + voice) as AudioClip));
     } else {
		GetComponent<AudioSource>().PlayOneShot( (Resources.Load("Voice/" + voice) as AudioClip));
	 }
 }

}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace BLabRouletteProject {

 public class GameCoinsController : MonoBehaviour {
 private OrbHolder ORB { get { return OrbHolder.Instance; } }

 public static float addCoins(float val) {
		float currentCoins = PlayerPrefs.GetFloat("PLAYER_CASH");
		float newVal = currentCoins + val;
		PlayerPrefs.SetFloat("PLAYER_CASH",newVal);
		OrbHolder.CoinsUpdatedEvent?.Invoke(newVal);
		return newVal;
 }

 public static float removeCoins(float val) {
		float currentCoins = PlayerPrefs.GetFloat("PLAYER_CASH");
		float newVal = currentCoins - val;
		PlayerPrefs.SetFloat("PLAYER_CASH",newVal);
		OrbHolder.CoinsUpdatedEvent?.Invoke(newVal);
		return newVal;
 }

 public static float getCurrentCoins() {
	return PlayerPrefs.GetFloat("PLAYER_CASH");
 }

 public static string getFormattedValue(float val) {
	return String.Format("{0:0,0}", val) + " $"; 
 }

 public static bool canPlay(float needsValue) {
   bool tmpRet = false;
     if(getCurrentCoins() >= needsValue) {
      tmpRet = true;
     }
     return tmpRet;
 }

}
}
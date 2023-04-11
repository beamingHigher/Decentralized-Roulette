using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BLab.Utility {

public class BLabUtility : MonoBehaviour {

 public static int avatarChoiceIdx = 0;

 #if USE_DEEP_LOG
   public static bool deepLog = true;
 #else
   public static bool deepLog = false;
 #endif

 public static string getStringByteFromTexture(Texture2D tex) {
			byte[] byteArray= tex.EncodeToPNG();
			string camImageBytes = Convert.ToBase64String(byteArray);
			return camImageBytes;
 }

 public static Sprite getSpriteFromBytes(string bArray) {
       Sprite tmpSprite = null;
      
			Texture2D text = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			text.LoadImage(Convert.FromBase64String(bArray));
			tmpSprite = Sprite.Create (text, new Rect(0,0,text.width,text.height), new Vector2(.5f,.5f));

       return tmpSprite;
 }

		public static Texture2D getImageFromCountryCode( string code) {
      Texture2D texCountry;
					string _cc = code;
					if( (_cc.Length == 0) || (_cc == "XX") ) {
					   texCountry = Resources.Load("NULL") as Texture2D;
					} else {
					   texCountry = Resources.Load(_cc) as Texture2D;
					}
	  return texCountry;
 }


}
}
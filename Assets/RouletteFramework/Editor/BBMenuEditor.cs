using UnityEngine;
using UnityEditor;
using System.Collections;

public class BBMenuEditor : EditorWindow {

	[MenuItem("Window/BBMenu/Delete PlayerPref")]
	public static void BBDeletePlayerpref() {
		PlayerPrefs.DeleteAll();
	}
	

}

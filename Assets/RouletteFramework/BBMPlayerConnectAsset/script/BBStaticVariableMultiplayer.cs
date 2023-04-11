using UnityEngine;
using System.Collections;

public class BBStaticVariableMultiplayer  {
#region multiplayerTmpData  	
#if USE_PHOTON
	public static CloudRegionCode selectedRegionCode = CloudRegionCode.eu;
    public static string currentMPPlayerName = "";
	public static string currentMPRoomName = "";
	public static int currentMPmaxPlayerNumber = 2;
	public static string photonConnectionVersion = "1.0";
#endif
#endregion	
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BBChatController : MonoBehaviour {

	public GameObject chatRoot;
	public InputField InputFieldChatText;
	public Text[] messagingText;
	public AudioClip messageBeep;
	public Color color_1;
	public Color color_2;
	public Image chatAlert;
	
#if USE_PHOTON	
	public PhotonView photonView;
	// Use this for initialization
	void Start () {
	 messagingText[0].color = color_1;
	 
	 if(!photonView) photonView = GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void resetAlertCol() {
		chatAlert.color = Color.red;
	}
	
	[PunRPC]
	void gotChatdata(string[] val) { 
		messagingHubMP(Color.yellow,"[" + val[1] + "]" + val[0]);
		GetComponent<AudioSource>().PlayOneShot(messageBeep);
		chatAlert.color = Color.green;
	}
	
	
	public void gotChatButton(GameObject _go) {
		
		if(_go.name == "ButtonOpenChat") {
			
			if(chatRoot.activeSelf) {
				//chatRoot.SetActive(false);
			} else {
				//chatRoot.SetActive(true);
			}
		}
		
		if(_go.name == "ButtonChatSend") {
			string[] val = new string[2];
			val[0] = InputFieldChatText.text;
			val[1] = PhotonNetwork.player.NickName;
			
			photonView.RPC("gotChatdata", PhotonTargets.All,val);
			//chatRoot.SetActive(false);
		}
		
		if(_go.name == "ButtonBBbanner") {
		//	Application.OpenURL(chat_ShowMessagebannerlink);
		}
		
		
	}
	
	void sendExternalmessage(string message) {
		string[] val = new string[2];
		val[0] = message;
		val[1] = PhotonNetwork.player.NickName;
		
		photonView.RPC("gotChatdata", PhotonTargets.All,val);
	}
	
	public void messagingHubMP(Color col, string message) {
		
		string[] oldText = new string[messagingText.Length];
		Color[] oldTextColor = new Color[messagingText.Length];
		
		
		for(int x = 0;x < oldText.Length;x++) {
			oldText[x] = messagingText[x].text;
			oldTextColor[x] = messagingText[x].color;
		}
		
		for(int i = 0; i < messagingText.Length-1; i++) {
			messagingText[i+1].text = oldText[i];
			messagingText[i+1].color = oldTextColor[i];
			
		}
		
		messagingText[0].text = message;
		
		if(messagingText[0].color == color_1) {
			messagingText[0].color = color_2;
		} else {
			messagingText[0].color = color_1;
		}

		 Debug.Log("**************messagingHubMP***************** : " + message);

		
	}


	void gotDanceButton() {
		GameObject.FindGameObjectWithTag("PlayerMP").SendMessage("gotDance",SendMessageOptions.DontRequireReceiver);
		string[] val = new string[2];
		val[0] = "Let's Dance!";
		val[1] = PhotonNetwork.player.NickName;
		
		photonView.RPC("gotChatdata", PhotonTargets.All,val);
		
	}	

	void gotHelloButton() {
		GameObject.FindGameObjectWithTag("PlayerMP").SendMessage("gotHelloButton",SendMessageOptions.DontRequireReceiver);
		string[] val = new string[2];
		val[0] = "Hi!";
		val[1] = PhotonNetwork.player.NickName;
		
		photonView.RPC("gotChatdata", PhotonTargets.All,val);
		
	}	

#endif
	
}

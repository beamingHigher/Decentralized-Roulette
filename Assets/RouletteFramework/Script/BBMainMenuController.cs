using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using BLab.Utility;

namespace BLabRouletteProject
{

	public class BBMainMenuController : MonoBehaviour
	{

		// public Toggle[] toggleCashList;
		public ToggleGroup toggleGroup;

		public GameObject PanelSettings;
		public GameObject PanelMain;
		public GameObject PanelRules;

		public Transform AvatarListRoot;
		public Canvas CanvasMultiplayer;

		public GameObject PanelNotEnoughCoins;

		private int avatarChoice = 0;

		void Awake()
		{

			Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_EDITOR
			gameObject.AddComponent<Blab.Utility.BBGetScreenShoot>();
#endif

#if MOBILE_INPUT
       Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
		}

		// Use this for initialization
		void Start()
		{

			if (!PlayerPrefs.HasKey("PLAYER_CASH"))
			{
				PlayerPrefs.SetFloat("PLAYER_CASH", float.Parse("0"));
			}

			if (!PlayerPrefs.HasKey("MIN_PLAY_CASH"))
			{
				PlayerPrefs.SetFloat("MIN_PLAY_CASH", float.Parse("1000"));
			}

			//Remove them if worked in orbholder
			GameObject.Find("TextPlayerTotalCoins").GetComponent<Text>().text = GameCoinsController.getFormattedValue(PlayerPrefs.GetFloat("PLAYER_CASH"));
			GameObject.Find("TextPlayerMinCoinsToPlay").GetComponent<Text>().text = GameCoinsController.getFormattedValue(PlayerPrefs.GetFloat("MIN_PLAY_CASH"));



			if (PlayerPrefs.HasKey("UseAutoSpin"))
			{
				if (PlayerPrefs.GetInt("UseAutoSpin") == 1)
				{
					SliderSpinSeconds.SetActive(true);
					SliderSpinSeconds.GetComponent<Slider>().value = PlayerPrefs.GetFloat("AutoSpinSeconds");
					GameObject.Find("ToggleWantAutoSpin").GetComponent<Toggle>().isOn = true;

				}
				else
				{
					SliderSpinSeconds.SetActive(false);
					GameObject.Find("ToggleWantAutoSpin").GetComponent<Toggle>().isOn = false;
				}
			}

		}

		void gotButtonClick(GameObject _go)
		{

			switch (_go.name)
			{
				case "ButtonOKOnPanelSettings":
					PanelMain.SetActive(true);
					PanelSettings.GetComponent<Canvas>().enabled = false;
					break;
				case "Buttonsetting":
					PanelMain.SetActive(false);
					PanelSettings.GetComponent<Canvas>().enabled = true;
					break;
				case "ButtonOKOnRulePanel":
					PanelMain.SetActive(true);
					PanelRules.SetActive(false);
					break;
				case "ButtonRules":
					PanelMain.SetActive(false);
					PanelRules.SetActive(true);
					break;
				case "ButtonOKGoMultiplayer":
					if (GameCoinsController.getCurrentCoins() >= PlayerPrefs.GetFloat("MIN_PLAY_CASH")) 
					{
						BLabUtility.avatarChoiceIdx = avatarChoice;
						SceneManager.LoadScene("MultiplayerMainMenu");
					}
					else
					{
						PanelNotEnoughCoins.SetActive(true);
					}
					break;
			}


		}

		public void playGame()
		{

			if (GameCoinsController.getCurrentCoins() >= PlayerPrefs.GetFloat("MIN_PLAY_CASH"))
			{
				GameObject tap = Resources.Load("tapPrefab") as GameObject;
				if (tag != null)
				{
					GameObject _tap = Instantiate(tap);
					Destroy(_tap, 1);
				}

				if (GameObject.Find("ToggleFrenchRoulette").GetComponent<Toggle>().isOn)
				{
					SceneManager.LoadSceneAsync("SinglePlayerScene");
				}
				else
				{
					SceneManager.LoadSceneAsync("SinglePlayerSceneAmerican");
				}


			}
			else
			{
				PanelNotEnoughCoins.SetActive(true);
			}

		}

		public GameObject SliderSpinSeconds;

		public void onSliderSplitSecondsChange(Slider s)
		{

			s.transform.Find("TextLabelSeconds").GetComponent<Text>().text = s.value.ToString();
			PlayerPrefs.SetFloat("AutoSpinSeconds", s.value);

		}

		public void ToggleWantAutoSpinChange(Toggle t)
		{

			if (t.isOn)
			{
				SliderSpinSeconds.SetActive(true);
				PlayerPrefs.SetInt("UseAutoSpin", 1);
				PlayerPrefs.SetFloat("AutoSpinSeconds", GameObject.Find("SliderSpinSeconds").GetComponent<Slider>().value);
			}
			else
			{
				SliderSpinSeconds.SetActive(false);
				PlayerPrefs.SetInt("UseAutoSpin", 0);
			}
		}

		void gotAvatarChoice(GameObject _go)
		{

			Debug.Log("gotAvatarChoice : " + _go.name);

			Button[] avatarButtons = AvatarListRoot.GetComponentsInChildren<Button>();

			for (int x = 0; x < avatarButtons.Length; x++)
			{
				AvatarListRoot.Find("avatar_" + x.ToString() + "/Image").GetComponent<Image>().color = Color.white;
			}

			string[] splittedName = _go.name.Split('_');

			AvatarListRoot.Find("avatar_" + splittedName[1] + "/Image").GetComponent<Image>().color = Color.green;

			avatarChoice = int.Parse(splittedName[1]);
		}

	}
}
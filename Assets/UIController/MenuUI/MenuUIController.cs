using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour {

	public GameObject userLinePrefab;

	private SyncController syncController;

	private Button createGameButton;
	private Button joinGameButton;
	private Button readyButton;
	private Button startGameButton;
	private Button selectMapButton;
	private InputField roomNameInput;
	private InputField userNameInput;
	private Image playerColorImage;
	private GameObject usersList;
	private Text roomNameText;
	private GameObject mapSelectContainer;
	private GameObject spellData;
	private Text spellName;
	private Text spellMultiplier;
	private GameObject spellMultiplierIcon;
	private Text spellIncrement;
	private GameObject spellIncrementIcon;
	private Text spellCooldown;
	private Text spellDescription;

	private List<SpellIcon> spellIcons;

	private GameObject selectRoomCanvas;
	private GameObject roomCanvas;

	private string mapName;

	void Awake () {
		syncController = GameObject.FindObjectOfType<SyncController>();

		selectRoomCanvas = transform.Find("SelectRoomCanvas").gameObject;
		roomCanvas = transform.Find("RoomCanvas").gameObject;

		roomNameInput = selectRoomCanvas.transform.Find("RoomNameInput").GetComponent<InputField>();
		roomNameInput.onValueChanged.AddListener(this.OnRoomNameChange);
		userNameInput = selectRoomCanvas.transform.Find("UserNameInput").GetComponent<InputField>();
		userNameInput.onValueChanged.AddListener(this.OnUserNameChange);
		createGameButton = selectRoomCanvas.transform.Find("CreateGameButton").GetComponent<Button>();
		joinGameButton = selectRoomCanvas.transform.Find("JoinGameButton").GetComponent<Button>();

		startGameButton = roomCanvas.transform.Find("StartGameButton").GetComponent<Button>();
		selectMapButton = roomCanvas.transform.Find("SelectMapButton").GetComponent<Button>();
		readyButton = roomCanvas.transform.Find("ReadyButton").GetComponent<Button>();
		playerColorImage = roomCanvas.transform.Find("PlayerColorImage").GetComponent<Image>();
		usersList = roomCanvas.transform.Find("UsersList").gameObject;
		roomNameText = roomCanvas.transform.Find("RoomNameText").GetComponent<Text>();
		mapSelectContainer = roomCanvas.transform.Find("SelectMapContainer").gameObject;
		mapSelectContainer.SetActive(false);
		SpellIcon[] iconsArray = roomCanvas.GetComponentsInChildren<SpellIcon>();
		spellIcons = new List<SpellIcon>(iconsArray);

		spellData = roomCanvas.transform.Find("SpellData").gameObject;
		spellName = spellData.transform.Find("SpellName").GetComponent<Text>();
		spellMultiplier = spellData.transform.Find("Multiplier").GetComponent<Text>();
		spellIncrement = spellData.transform.Find("Increment").GetComponent<Text>();
		spellCooldown = spellData.transform.Find("Cooldown").GetComponent<Text>();
		spellDescription = spellData.transform.Find("SpellDescription").GetComponent<Text>();
		spellMultiplierIcon = spellData.transform.Find("MultiplierIcon").gameObject;
		spellIncrementIcon = spellData.transform.Find("IncrementIcon").gameObject;

		spellData.SetActive(false);

		createGameButton.onClick.AddListener(syncController.CreateGame);
		joinGameButton.onClick.AddListener(syncController.JoinGame);
		readyButton.onClick.AddListener(this.onReadyClick);
		selectMapButton.onClick.AddListener(this.OpenSelectMap);
		mapName = "";
		startGameButton.onClick.AddListener(() => syncController.StartGame(mapName));

		selectRoomCanvas.SetActive(true);
		roomCanvas.SetActive(false);

		User us = syncController.GetUser();
		if(us != null) this.MyUserJoinedRoom(syncController.GetRoomName(), us.isOwner);
	}

	void onReadyClick() {
		syncController.ToggleReady();
		Text text = readyButton.GetComponentInChildren<Text>();

		User us = syncController.GetUser();
		if(us.status == "waiting") {
			text.text = "Wait";
		} else {
			text.text = "Ready";			
		}
	}

	public void SetPlayerColor(Color color) {
		playerColorImage.color = color;
	}

	public void OnRoomNameChange(string name) {
		syncController.SetRoomName(name);
	}

	public void OnUserNameChange(string name) {
		syncController.SetUserName(name);
	}

	public void SetUserName(string name) {
		userNameInput.text = name;
	}

	public void MyUserJoinedRoom(string roomName, bool isOwner) {
		selectRoomCanvas.SetActive(false);
		roomCanvas.SetActive(true);

		if(!isOwner) {
			startGameButton.gameObject.SetActive(false);
			selectMapButton.gameObject.SetActive(false);
		}
		roomNameText.text = syncController.GetRoomName();
	}

	public void UserStatusUpdate(List<User> users) {
		if(users == null) return;
		
        foreach (Transform child in usersList.transform) {
			Destroy(child.gameObject);
		}

		foreach(User u in users) {
			GameObject userLine = Instantiate(userLinePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			userLine.transform.SetParent(usersList.transform);

			if(u.status == "ready") userLine.transform.Find("StatusColor").GetComponent<Image>().color = new Color(0.1f, 0.36f, 0.13f, 0.4f);
			else userLine.transform.Find("StatusColor").GetComponent<Image>().color = new Color(0, 0, 0, 0);
			userLine.transform.Find("UserName").GetComponent<Text>().text = u.name;
			userLine.transform.Find("WinCount").GetComponent<Text>().text = u.winCount.ToString();
			userLine.transform.Find("UserColor").GetComponent<Image>().color = u.color;
			if(!u.isOwner) userLine.transform.Find("OwnerImage").gameObject.SetActive(false);
		}
	}

	public void OpenSelectMap() {
		mapSelectContainer.SetActive(true);
	}

	public void SetMapName(string mapName) {
		this.mapName = mapName;
		mapSelectContainer.SetActive(false);
	}

	public string GetMapName() {
		return this.mapName;
	}

	public void UserJoinedRoom(int number) {
		
	}

	public void ExitGame() {
		Application.Quit();
	}

	public void SelectSpell(JSONObject data, int idx) {
		SpellIcon ic = spellIcons.Find(x => x.name == data["spellName"].str);
		ic.Select(idx);

		spellData.SetActive(true);
		spellName.text = data["spellData"]["name"].str;
		if(data["spellData"]["type"].str == "offensive") {
			spellMultiplier.text = (data["spellData"]["knockbackMultiplier"].n * 100) + "%";
			spellIncrement.text = (data["spellData"]["knockbackIncrement"].n * 100 - 100) + "%";
			spellMultiplierIcon.SetActive(true);
			spellIncrementIcon.SetActive(true);
		} else {
			spellMultiplier.text = "";
			spellIncrement.text = "";
			spellMultiplierIcon.SetActive(false);
			spellIncrementIcon.SetActive(false);
		}

		spellCooldown.text = (data["spellData"]["cooldown"].n / 1000) + " sec.";
		spellDescription.text = data["spellData"]["description"].str;
	}

	public void DeselectSpell(string name) {
		SpellIcon ic = spellIcons.Find(x => x.name == name);
		ic.Deselect();
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour {

	public GameObject userLinePrefab;
	public GameObject spellIconPrefab;

	private SyncController syncController;
    private SpellsList spellsController;

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
	private Transform offensiveSpellsList;
	private Transform defensiveSpellsList;
	private GameObject spellData;
	private Text spellName;
	private Text spellMultiplier;
	private GameObject spellMultiplierIcon;
	private Text spellIncrement;
	private GameObject spellIncrementIcon;
	private Text spellCooldown;
	private Text spellDescription;

	private List<SpellIcon> spellIcons;
	private List<User> lastUsers;

	private GameObject selectRoomCanvas;
	private GameObject roomCanvas;

	private string mapName;

	void Awake () {
		syncController = GameObject.FindObjectOfType<SyncController>();
        spellsController = GameObject.FindObjectOfType<SpellsList>();

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
		offensiveSpellsList = roomCanvas.transform.Find("OffensiveSpellsList");
		defensiveSpellsList = roomCanvas.transform.Find("DefensiveSpellsList");
		roomNameText = roomCanvas.transform.Find("RoomNameText").GetComponent<Text>();
		mapSelectContainer = roomCanvas.transform.Find("SelectMapContainer").gameObject;
		mapSelectContainer.SetActive(false);

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

		this.SpellsStatusUpdate();
	}

	public void UserStatusUpdate(List<User> users) {
		if(users != null) lastUsers = users;
		if(lastUsers == null) return;
        
		foreach (Transform child in usersList.transform) {
			Destroy(child.gameObject);
		}

		foreach(User u in lastUsers) {
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

	public void SpellsStatusUpdate() {
		foreach (Transform child in offensiveSpellsList) {
			Destroy(child.gameObject);
		}
		foreach (Transform child in defensiveSpellsList) {
			Destroy(child.gameObject);
		}

		spellIcons = new List<SpellIcon>();
		foreach(SpellItem spell in spellsController.spells) {
			SpellIcon icon = Instantiate(spellIconPrefab).GetComponent<SpellIcon>();
			icon.SetData(spell);
			if(spell.type == "offensive") icon.transform.SetParent(offensiveSpellsList);
			else icon.transform.SetParent(defensiveSpellsList);

			spellIcons.Add(icon);
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

		SpellIcon ic = spellIcons.Find(x => x.spellName == data["spellName"].str);
		ic.Select(idx);

		SpellItem sData = spellsController.spells.Find(x => x.name == data["spellName"].str);

		spellData.SetActive(true);
		spellName.text = sData.showName;
		if(sData.type == "offensive") {
			spellMultiplier.text = (sData.knockbackMultiplier * 100) + "%";
			spellIncrement.text = (sData.knockbackIncrement * 100 - 100) + "%";
			spellMultiplierIcon.SetActive(true);
			spellIncrementIcon.SetActive(true);
		} else {
			spellMultiplier.text = "";
			spellIncrement.text = "";
			spellMultiplierIcon.SetActive(false);
			spellIncrementIcon.SetActive(false);
		}

		spellCooldown.text = (sData.cooldown / 1000) + " sec.";
		spellDescription.text = sData.description;
	}

	public void DeselectSpell(string spellName) {
		SpellIcon ic = spellIcons.Find(x => x.spellName == spellName);
		ic.Deselect();
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public bool isGameScene = true;
	public GameObject userLinePrefab;

	private SyncController syncController;

	private Button createGameButton;
	private Button joinGameButton;
	private Button readyButton;
	private Button startGameButton;
	private InputField roomNameInput;
	private InputField userNameInput;
	private Image playerColorImage;
	private GameObject usersList;
	private Text roomNameText;

	private GameObject selectRoomCanvas;
	private GameObject roomCanvas;

	private Player player;
	private Text playerInfoText;
	private Image healthbarImage;

	private GameObject winnerCanvas;
	private GameObject loserCanvas;

	void Awake () {
		syncController = GameObject.FindObjectOfType<SyncController>();

		if(!isGameScene) {
			selectRoomCanvas = GameObject.Find("SelectRoomCanvas");
			roomCanvas = GameObject.Find("RoomCanvas");

			roomNameInput = GameObject.Find("RoomNameInput").GetComponent<InputField>();
			userNameInput = GameObject.Find("UserNameInput").GetComponent<InputField>();
			createGameButton = GameObject.Find("CreateGameButton").GetComponent<Button>();
			joinGameButton = GameObject.Find("JoinGameButton").GetComponent<Button>();
			readyButton = GameObject.Find("ReadyButton").GetComponent<Button>();
			startGameButton = GameObject.Find("StartGameButton").GetComponent<Button>();
			playerColorImage = GameObject.Find("PlayerColorImage").GetComponent<Image>();
			usersList = GameObject.Find("UsersList");
			roomNameText = GameObject.Find("RoomNameText").GetComponent<Text>();

			createGameButton.onClick.AddListener(syncController.CreateGame);
			joinGameButton.onClick.AddListener(syncController.JoinGame);
			readyButton.onClick.AddListener(this.onReadyClick);
			startGameButton.onClick.AddListener(syncController.StartGame);

			selectRoomCanvas.SetActive(true);
			roomCanvas.SetActive(false);

			if(syncController.isUserInRoom) this.MyUserJoinedRoom(syncController.GetRoomName(), syncController.isUserRoomOwner);

		} else {
			playerInfoText = GameObject.Find("InfoText").GetComponent<Text>();
			healthbarImage = GameObject.Find("HealthbarImage").GetComponent<Image>();
			winnerCanvas = transform.Find("WinnerCanvas").gameObject;
			loserCanvas = transform.Find("LoserCanvas").gameObject;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!isGameScene) {
			
		} else {
			if(player == null) {
				player = syncController.GetPlayer();
			} else {
				if(player.Status == "alive") {
					Vector3 scl = healthbarImage.rectTransform.localScale;
					scl.x = player.Life / 100f;
					healthbarImage.rectTransform.localScale = scl;
					playerInfoText.text = "Knockback " + player.Knockback;
				} else {
					healthbarImage.rectTransform.localScale = Vector3.zero;
					playerInfoText.text = "U IS DED";
				}
			}
		}
		
	}

	void onReadyClick() {
		syncController.ToggleReady();
		Text text = readyButton.GetComponentInChildren<Text>();

		if(syncController.isUserReady) {
			text.text = "Wait";
		} else {
			text.text = "Ready";			
		}
	}

	public void SetPlayerColor(Color color) {
		playerColorImage.color = color;
	}

	public void OnRoomNameChange() {
		syncController.SetRoomName(roomNameInput.text);
	}

	public void OnUserNameChange() {
		syncController.SetUserName(userNameInput.text);
	}

	public void MyUserJoinedRoom(string roomName, bool isOwner) {
		selectRoomCanvas.SetActive(false);
		roomCanvas.SetActive(true);

		if(!isOwner) startGameButton.gameObject.SetActive(false);
		roomNameText.text = syncController.GetRoomName();
	}

	public void UserJoinedRoom(int number) {
		
	}

	public void EndGame(bool win) {
		if(win) winnerCanvas.SetActive(true);
		else loserCanvas.SetActive(true);
	}

	public void ExitGame() {
		Application.Quit();
	}

	public void UserStatusUpdate(List<User> users) {
		if(isGameScene) return;

        foreach (Transform child in usersList.transform) {
			Destroy(child.gameObject);
		}

		foreach(User u in users) {
			GameObject userLine = Instantiate(userLinePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			userLine.transform.SetParent(usersList.transform);

			if(u.status == "ready") userLine.transform.Find("StatusColor").GetComponent<Image>().color = new Color(0.1f, 0.36f, 0.13f, 0.4f);
			else userLine.transform.Find("StatusColor").GetComponent<Image>().color = new Color(0, 0, 0, 0);
			userLine.transform.Find("UserName").GetComponent<Text>().text = u.name;
			userLine.transform.Find("UserColor").GetComponent<Image>().color = u.color;
			if(!u.isOwner) userLine.transform.Find("OwnerImage").gameObject.SetActive(false);
		}
	}

}

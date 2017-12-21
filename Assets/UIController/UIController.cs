using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public bool isGameScene = true;

	private SyncController syncController;

	private Button createGameButton;
	private Button joinGameButton;
	private Button readyButton;
	private Button startGameButton;
	private InputField roomNameInput;
	private Text usersReadyText;
	private Text usersWaitingText;
	private Image playerColorImage;

	private Player player;
	private Text playerInfoText;

	void Awake () {
		syncController = GameObject.FindObjectOfType<SyncController>();

		if(!isGameScene) {
			roomNameInput = GameObject.Find("RoomNameInput").GetComponent<InputField>();
			createGameButton = GameObject.Find("CreateGameButton").GetComponent<Button>();
			joinGameButton = GameObject.Find("JoinGameButton").GetComponent<Button>();
			readyButton = GameObject.Find("ReadyButton").GetComponent<Button>();
			startGameButton = GameObject.Find("StartGameButton").GetComponent<Button>();
			usersReadyText = GameObject.Find("UsersReadyText").GetComponent<Text>();
			usersWaitingText = GameObject.Find("UsersWaitingText").GetComponent<Text>();
			playerColorImage = GameObject.Find("PlayerColorImage").GetComponent<Image>();

			createGameButton.onClick.AddListener(syncController.CreateGame);
			joinGameButton.onClick.AddListener(syncController.JoinGame);
			readyButton.onClick.AddListener(this.onReadyClick);
			startGameButton.onClick.AddListener(syncController.StartGame);

			roomNameInput.gameObject.SetActive(true);
			createGameButton.gameObject.SetActive(true);
			joinGameButton.gameObject.SetActive(true);

			readyButton.gameObject.SetActive(false);
			startGameButton.gameObject.SetActive(false);
			playerColorImage.gameObject.SetActive(false);

			if(syncController.isUserInRoom) this.MyUserJoinedRoom();

		} else {
			playerInfoText = GameObject.Find("InfoText").GetComponent<Text>();
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
					playerInfoText.text = "Life: " + player.Life + "\nKnockback: " + player.Knockback;
				} else {
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

	public void MyUserJoinedRoom() {
		roomNameInput.gameObject.SetActive(false);
		createGameButton.gameObject.SetActive(false);
		joinGameButton.gameObject.SetActive(false);

		readyButton.gameObject.SetActive(true);
		startGameButton.gameObject.SetActive(true);
		playerColorImage.gameObject.SetActive(true);
	}

	public void UserJoinedRoom(int number) {
		
	}

	public void ExitGame() {
		Application.Quit();
	}

	public void UserStatusUpdate(int ready, int waiting) {
		usersReadyText.text = ready.ToString();
		usersWaitingText.text = waiting.ToString();
	}

}

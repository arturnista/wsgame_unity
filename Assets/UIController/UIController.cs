using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public bool isGameScene = true;

	private SyncController syncController;

	private Player player;
	private Text playerInfoText;
	private InputField roomNameInput;

	void Awake () {
		syncController = GameObject.FindObjectOfType<SyncController>();
		if(!isGameScene) roomNameInput = GameObject.Find("RoomNameInput").GetComponent<InputField>();

		if(isGameScene) playerInfoText = GameObject.Find("InfoText").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!isGameScene) return;
		
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

	public void OnRoomNameChange() {
		syncController.SetRoomName(roomNameInput.text);
	}
}

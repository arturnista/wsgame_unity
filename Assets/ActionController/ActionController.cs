using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour {

    enum Action {
        Move,
        Fireball,
    }

	private SyncController syncController;
	private Player player;
	private Action nextAction;

	private GameObject moveSignal;
	private float screenWidthProp;
	private Vector3 lastMousePos;

	void Awake () {
		moveSignal = GameObject.Find("MoveSignal");
		syncController = GameObject.FindObjectOfType<SyncController>();
		nextAction = Action.Move;

		screenWidthProp = Screen.width * 1f / Screen.height;
	}

	// Update is called once per frame
	void Update () {
		float cameraSize = Camera.main.orthographicSize * 2;
		transform.localScale = new Vector2(cameraSize * screenWidthProp, cameraSize);

		if(player == null) {
			player = syncController.GetPlayer();
			return;
		}
		if(Input.GetKeyDown(KeyCode.Q)) {
			nextAction = Action.Fireball;
		}
		if(Input.GetKeyDown(KeyCode.E)) {
			syncController.UseReflectShield();
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
            nextAction = Action.Move;
        }
	}

	void OnMouseDown() {
        if(player == null) return;
        
		Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 direction = position - player.transform.position;

		switch(nextAction) {
			case Action.Move:
				moveSignal.transform.position = position;
				syncController.MovePlayer(position);
				break;
            case Action.Fireball:
                syncController.UseFireball(position, direction.normalized);
                break;
        }
		nextAction = Action.Move;
    }

	// void OnMouseDrag() {
	// 	Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

	// 	if(nextAction != Action.Move) {
	// 		lastMousePos = mousePos;
	// 		return;
	// 	}

	// 	if(Vector3.Distance(mousePos, lastMousePos) < 5.0f) return;
	// 	lastMousePos = mousePos;

	// 	moveSignal.transform.position = mousePos;
	// 	syncController.MovePlayer(mousePos);
	// }

}

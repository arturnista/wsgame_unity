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

	void Awake () {
		moveSignal = GameObject.Find("MoveSignal");
		syncController = GameObject.FindObjectOfType<SyncController>();
		nextAction = Action.Move;
		transform.localScale = new Vector2(Screen.width, Screen.height);
	}
	
	// Update is called once per frame
	void Update () {
		if(player == null) {
			player = syncController.GetPlayer();
		}
		if(Input.GetKeyDown(KeyCode.Q)) {
			nextAction = Action.Fireball;
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
            nextAction = Action.Move;
        }
	}

	void OnMouseDown() {
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
	
}

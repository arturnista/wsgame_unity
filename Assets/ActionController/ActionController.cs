using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour {

    enum Action {
        Move,
        Fireball,
        Blink,
    }

	private SyncController syncController;
	private Player player;
	private Action nextAction;

	private GameObject moveSignal;
	private float screenWidthProp;
	private Vector3 lastMousePos;
	private List<string> spells;

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
			this.SelectSpell(0);
		}
		if(Input.GetKeyDown(KeyCode.W)) {
			this.SelectSpell(1);
		}
		if(Input.GetKeyDown(KeyCode.E)) {
			this.SelectSpell(2);
		}
		if(Input.GetKeyDown(KeyCode.R)) {
			this.SelectSpell(3);
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
            nextAction = Action.Move;
        }
	}

	public void SetSpells(List<string> spells) {
		this.spells = new List<string>(spells);
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
				this.UseSpell("fireball");
                break;
            case Action.Blink:
				this.UseSpell("blink");
                break;
        }
		nextAction = Action.Move;
    }

	void SelectSpell(int idx) {
		if(idx > spells.Count - 1) return;
		string spellName = spells[idx];
		switch(spellName) {
			case "fireball": 
				nextAction = Action.Fireball;
				break;
			case "blink": 
				nextAction = Action.Blink;
				break;
			default:
				this.UseSpell(spellName);
				break;
		}
	}

	void UseSpell(string spellName) {
		Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 direction = position - player.transform.position;
		syncController.UseSpell(spellName, position, direction.normalized);
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

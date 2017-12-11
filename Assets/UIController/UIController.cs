using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

	private SyncController syncController;

	private Player player;

	void Awake () {
		syncController = GameObject.FindObjectOfType<SyncController>();
	}
	
	// Update is called once per frame
	void Update () {
		if(player == null) {
			player = syncController.GetPlayer();
		} else {
			print(player.Life);
		}
	}
}

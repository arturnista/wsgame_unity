using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour {

	private SyncController syncController;
	
	private string mapName;

	private Player player;
	private Text playerInfoText;
	private Image healthbarImage;

	private GameObject winnerCanvas;
	private GameObject loserCanvas;

	void Awake () {
		syncController = GameObject.FindObjectOfType<SyncController>();

		playerInfoText = GameObject.Find("InfoText").GetComponent<Text>();
		healthbarImage = GameObject.Find("HealthbarImage").GetComponent<Image>();
		winnerCanvas = transform.Find("WinnerCanvas").gameObject;
		loserCanvas = transform.Find("LoserCanvas").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
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

	public void EndGame(bool win) {
		if(win) winnerCanvas.SetActive(true);
		else loserCanvas.SetActive(true);
	}

}

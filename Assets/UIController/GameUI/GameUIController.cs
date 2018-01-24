using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour {

	public GameObject spellIconPrefab;

	private SyncController syncController;
	
	private string mapName;

	private Player player;
	private Text playerInfoText;
	private Image healthbarImage;

	private Transform spellsListCanvas;
	private List<GameSpellIcon> spellIcons;

	private GameObject winnerCanvas;
	private GameObject loserCanvas;

	void Awake () {
		syncController = GameObject.FindObjectOfType<SyncController>();

		playerInfoText = GameObject.Find("InfoText").GetComponent<Text>();
		healthbarImage = GameObject.Find("HealthbarImage").GetComponent<Image>();
		spellsListCanvas = GameObject.Find("SpellsListCanvas").transform;
		winnerCanvas = transform.Find("WinnerCanvas").gameObject;
		loserCanvas = transform.Find("LoserCanvas").gameObject;
		
		spellIcons = new List<GameSpellIcon>();
		foreach(SpellItem spellItem in syncController.GetSpellsSelected()) {
			GameSpellIcon spellIcon = Instantiate(spellIconPrefab).GetComponent<GameSpellIcon>();
			spellIcon.SetData(spellItem);
			spellIcon.transform.SetParent(spellsListCanvas);

			spellIcons.Add(spellIcon);
		}
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

	public void UseSpell(string spellName) {
		GameSpellIcon sIcon = spellIcons.Find(x => x.spellName == spellName);
		sIcon.UseSpell();
	}

	public void EndGame(bool win) {
		Text textName;
		if(win) {
			winnerCanvas.SetActive(true);
			textName = winnerCanvas.transform.Find("Panel/PlayerNameText").GetComponent<Text>();
		} else {
			loserCanvas.SetActive(true);
			textName = loserCanvas.transform.Find("Panel/PlayerNameText").GetComponent<Text>();
		}

		User user = syncController.GetUser();
		textName.text = user.name;
		foreach(UserSpell us in user.spells) {
			Debug.Log(us.name + " " + us.uses);
		}
	}

}

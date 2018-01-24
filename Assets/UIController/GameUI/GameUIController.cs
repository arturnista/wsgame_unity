using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour {

	public GameObject spellIconPrefab;
	public GameObject endSpellIconPrefab;

	private SyncController syncController;
	private SpellsController spellsController;
	
	private string mapName;

	private Player player;
	private Text playerInfoText;
	private Image healthbarImage;

	private Transform spellsListCanvas;
	private Transform endSpellsListCanvas;
	private List<GameSpellIcon> spellIcons;

	private GameObject endCanvas;

	void Awake () {
		syncController = GameObject.FindObjectOfType<SyncController>();
		spellsController = GameObject.FindObjectOfType<SpellsController>();

		playerInfoText = GameObject.Find("InfoText").GetComponent<Text>();
		healthbarImage = GameObject.Find("HealthbarImage").GetComponent<Image>();
		spellsListCanvas = GameObject.Find("SpellsListCanvas").transform;
		endCanvas = transform.Find("EndCanvas").gameObject;
		endCanvas.SetActive(false);
		
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
		endCanvas.SetActive(true);
		if(win) {
			textName = endCanvas.transform.Find("Panel/PlayerNameText").GetComponent<Text>();
			endCanvas.transform.Find("Panel/WinText").gameObject.SetActive(true);
		} else {
			textName = endCanvas.transform.Find("Panel/PlayerNameText").GetComponent<Text>();
			endCanvas.transform.Find("Panel/LoseText").gameObject.SetActive(true);
		}

		User user = syncController.GetUser();
		textName.text = user.name;

		endSpellsListCanvas = endCanvas.transform.Find("Panel/SpellsCanvas/SpellsList");
		foreach(UserSpell us in user.spells) {
			GameObject spell = Instantiate(endSpellIconPrefab) as GameObject;
			spell.transform.SetParent(endSpellsListCanvas);
			SpellItem sItem = spellsController.GetSpellItem(us.name);
			spell.transform.Find("SpellName").GetComponent<Text>().text = sItem.showName;
			spell.transform.Find("SpellIcon").GetComponent<Image>().sprite = sItem.image;
			spell.transform.Find("SpellUses").GetComponent<Text>().text = us.uses.ToString();
		}
	}

}

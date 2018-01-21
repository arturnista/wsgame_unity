using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellIcon : MonoBehaviour {

	public string spellName;

	private bool isSelected;

	private SyncController syncController;
	private GameObject selectImage;

	private Image image;
	private Button button;
	private Text selectText;

	void Awake() {
		button = GetComponent<Button>();
		button.onClick.AddListener(this.Toggle);

		selectImage = transform.Find("SelectedImage").gameObject;
		selectImage.SetActive(false);
		selectText = transform.Find("SelectedText").GetComponent<Text>();
		selectText.gameObject.SetActive(false);
		isSelected = false;

		image = transform.Find("SpellImage").GetComponent<Image>();

		syncController = GameObject.FindObjectOfType<SyncController>();
	}

	public void SetData(SpellItem spell) {
		spellName = spell.name;
		image.sprite = spell.image;
	}

	void Toggle() {
		if(!isSelected) {
			syncController.SelectSpell(spellName);
		} else {
			syncController.DeselectSpell(spellName);
		}
	}

	public void Select(int idx) {
		selectImage.SetActive(true);
		selectText.gameObject.SetActive(true);
		if(idx == 0) selectText.text = "Q";
		else if(idx == 1) selectText.text = "W";
		else if(idx == 2) selectText.text = "E";
		else if(idx == 3) selectText.text = "R";
		isSelected = true;
	}

	public void Deselect() {
		selectText.gameObject.SetActive(false);
		selectImage.SetActive(false);
		isSelected = false;
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellIcon : MonoBehaviour {

	public string name;

	private bool isSelected;

	private SyncController syncController;
	private GameObject selectImage;

	private Button button;

	void Awake() {
		button = GetComponent<Button>();
		button.onClick.AddListener(this.Toggle);

		selectImage = transform.Find("SelectedImage").gameObject;
		selectImage.SetActive(false);
		isSelected = false;

		syncController = GameObject.FindObjectOfType<SyncController>();
	}

	void Toggle() {
		if(!isSelected) {
			selectImage.SetActive(true);
			syncController.SelectSpell(name);
			isSelected = true;
		} else {
			selectImage.SetActive(false);
			syncController.DeselectSpell(name);
			isSelected = false;
		}
	}

}

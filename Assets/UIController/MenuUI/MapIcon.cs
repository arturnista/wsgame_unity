using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Button))]
public class MapIcon : MonoBehaviour {

	public string mapName;

	private MenuUIController menuUIController;
	private Button button;
	private Image mapBackground;

	void Awake () {
		menuUIController = GameObject.FindObjectOfType<MenuUIController>();
		button = GetComponent<Button>();
		button.onClick.AddListener(this.SelectMap);

		mapBackground = transform.Find("MapBackground").GetComponent<Image>();
	}

	void Update() {
		if(!this.gameObject.activeSelf) return;

		if(menuUIController.GetMapName() == mapName) this.Active();
		else this.Deactive();
	}
	
	void SelectMap() {
		menuUIController.SetMapName(mapName);
	}

	void Active() {
		mapBackground.color = new Color(1f, 1f, 1f, 1f);
	}

	void Deactive() {
		mapBackground.color = new Color(1f, 1f, 1f, .3f);		
	}
}

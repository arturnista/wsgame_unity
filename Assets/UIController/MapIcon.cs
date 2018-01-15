using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Button))]
public class MapIcon : MonoBehaviour {

	public string mapName;

	private UIController uiController;
	private Button button;
	private Image mapBackground;

	void Awake () {
		uiController = GameObject.FindObjectOfType<UIController>();
		button = GetComponent<Button>();
		button.onClick.AddListener(this.SelectMap);

		mapBackground = transform.Find("MapBackground").GetComponent<Image>();
	}

	void Update() {
		if(!this.gameObject.activeSelf) return;

		if(uiController.GetMapName() == mapName) this.Active();
		else this.Deactive();
	}
	
	void SelectMap() {
		uiController.SetMapName(mapName);
	}

	void Active() {
		mapBackground.color = new Color(1f, 1f, 1f, 1f);
	}

	void Deactive() {
		mapBackground.color = new Color(1f, 1f, 1f, .3f);		
	}
}

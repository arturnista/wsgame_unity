using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Button))]
public class MapIcon : MonoBehaviour {

	public string mapName;

	private UIController uiController;
	private Button button;

	void Awake () {
		uiController = GameObject.FindObjectOfType<UIController>();
		button = GetComponent<Button>();
		button.onClick.AddListener(this.SelectMap);
	}
	
	void SelectMap() {
		uiController.SetMapName(mapName);
	}
}

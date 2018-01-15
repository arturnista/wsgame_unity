using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGrid : MonoBehaviour {

	public Color normalColor;
	public Color destroyedColor;

	private int index;

	private float size;
	private Vector3 position;
	private string status;

	private SpriteRenderer spriteRenderer;
	private SpriteRenderer statusSprite;

	void Awake () {
		spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
		statusSprite = transform.Find("StatusSprite").GetComponent<SpriteRenderer>();
	}

	public void SetData(int index, JSONObject data) {
		this.index = index;

		size = data["size"].n;
		status = data["status"].str;

		float xMapPos = data["position"]["x"].n;
		float yMapPos = data["position"]["y"].n;
		position = new Vector2 (xMapPos, yMapPos);

		switch (status) {
			case "normal": 
				spriteRenderer.color = normalColor;
				statusSprite.color = Color.black;
				break;
			case "toDestroy": 
				spriteRenderer.color = normalColor;
				statusSprite.color = Color.red;
				break;
			case "toRevive": 
				spriteRenderer.color = destroyedColor;
				statusSprite.color = Color.blue;
				break;
			case "destroyed": 
				spriteRenderer.color = destroyedColor;
				statusSprite.color = Color.black;
				break;
		}

	}
	
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGrid : MonoBehaviour {

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
				spriteRenderer.color = Color.white;
				statusSprite.color = Color.black;
				break;
			case "toDestroy": 
				spriteRenderer.color = Color.white;
				statusSprite.color = Color.red;
				break;
			case "toRevive": 
				spriteRenderer.color = new Color(0.4f, 0.4f, 0.4f, 1f);
				statusSprite.color = Color.blue;
				break;
			case "destroyed": 
				spriteRenderer.color = new Color(0.4f, 0.4f, 0.4f, 1f);
				statusSprite.color = Color.black;
				break;
		}

	}
	
}

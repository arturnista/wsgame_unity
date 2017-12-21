using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SyncObject {
	
	private float life;
	private float knockback;
	private string status;

	private SpriteRenderer spriteRenderer;

	public float Life {
		get {
			return life;
		}
	}

	public float Knockback {
		get {
			return knockback;
		}
	}

	public string Status {
		get {
			return status;
		}
	}

	void Awake() {
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}

	protected override void Update() {
			
	}

	public override void SetData(JSONObject data) {
		base.SetData(data);

		this.life = data["life"].n;
		this.knockback = data["knockbackValue"].n;
		this.status = data["status"].str;
		Color color;
		ColorUtility.TryParseHtmlString(data["color"].str, out color);
		this.spriteRenderer.color = color;
	}
	
}

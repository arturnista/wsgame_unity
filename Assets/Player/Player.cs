﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SyncObject {

	private float life;

	public float Life {
		get {
			return life;
		}
	}

	protected override void Update() {
			
	}

	public override void SetData(JSONObject data) {
		base.SetData(data);

		this.life = data["life"].n;
	}
	
}

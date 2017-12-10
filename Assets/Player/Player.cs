using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SyncObject {

	private float life;

	protected override void Update() {
		// Debug.Log("life: " + this.life);
	}

    public override void SetData(JSONObject data) {
		base.SetData(data);

		life = data["life"].n;
    }
}

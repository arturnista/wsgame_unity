﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : Spell {

    private TrailRenderer trailRenderer;

    void Awake() {
        trailRenderer = GetComponent<TrailRenderer>();
    }

	public override void Destroy() {
        // if(destroyPrefab) {
        //     Instantiate(destroyPrefab, transform.position, transform.rotation);
        // }
        Destroy(this.gameObject);
    }

	public override void SetData(JSONObject data) {
		base.SetData(data);
        float size = data["collider"]["size"].n;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : Spell {

    public GameObject destroyPrefab;

    void Awake() {
        
    }

	public override void Destroy() {
        // if(destroyPrefab) {
        //     Instantiate(destroyPrefab, transform.position, transform.rotation);
        // }
        Destroy(this.gameObject);
    }

	public override void SetData(JSONObject data) {
		base.SetData(data);
    }

}

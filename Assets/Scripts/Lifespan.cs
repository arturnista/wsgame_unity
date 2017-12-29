using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifespan : MonoBehaviour {

	public float time;
	private float currentTime = 0f;

	void Update () {
		currentTime += Time.deltaTime;
		if(currentTime > time) Destroy(this.gameObject);
	}
}

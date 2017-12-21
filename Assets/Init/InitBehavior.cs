using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class InitBehavior : MonoBehaviour {

	public float timeToLoad = .5f;

	private float timePassed;
	void Start () {
		// SceneManager.LoadScene("Menu");    
	}
	
	void Update () {
		timePassed += Time.deltaTime;
		if(timePassed > timeToLoad) {
			SceneManager.LoadScene("Menu");    
		}
	}
}

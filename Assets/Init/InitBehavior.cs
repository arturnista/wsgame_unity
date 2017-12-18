using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class InitBehavior : MonoBehaviour {

	private float timePassed;
	void Start () {
		// SceneManager.LoadScene("Menu");    
	}
	
	void Update () {
		timePassed += Time.deltaTime;
		if(timePassed > 2f) {
			SceneManager.LoadScene("Menu");    
		}
	}
}

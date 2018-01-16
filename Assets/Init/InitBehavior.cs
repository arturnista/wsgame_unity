using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class InitBehavior : MonoBehaviour {
	
	public GameObject localSyncController;
	public GameObject remoteSyncController;

	public float timeToLoad;

	private float timePassed;

	void Start () {
		if(Application.isEditor) {
			Instantiate(localSyncController, Vector3.zero, Quaternion.identity);
			SceneManager.LoadScene("Menu");
		} else {
			Instantiate(remoteSyncController, Vector3.zero, Quaternion.identity);
		}
	}
	
	void Update () {
		timePassed += Time.deltaTime;
		if(timePassed > timeToLoad) {
			SceneManager.LoadScene("Menu");    
		}
	}
}

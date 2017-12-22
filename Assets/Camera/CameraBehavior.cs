using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {

	public int minimumSize = 300;

	Camera thisCamera;
	SyncController syncController;
	MapController mapController;
	Player player;

	private float smoothVelocity;
	
	void Awake () {
		syncController = GameObject.FindObjectOfType<SyncController>();
		mapController = GameObject.FindObjectOfType<MapController>();

		thisCamera = GetComponent<Camera>();
		thisCamera.orthographicSize = minimumSize;
	}
	
	// Update is called once per frame
	void Update () {
		if(player == null) {
			player = syncController.GetPlayer();
			return;
		}
		
		float dist = Vector2.Distance(mapController.GetPosition(), player.transform.position) * 1.3f;
		
		if(dist > minimumSize) thisCamera.orthographicSize = Mathf.SmoothDamp(thisCamera.orthographicSize, dist, ref smoothVelocity, 1f);
		else thisCamera.orthographicSize = Mathf.SmoothDamp(thisCamera.orthographicSize, minimumSize, ref smoothVelocity, 1f);

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {

	public enum CameraMode {
		ToStart,
		Game,
		Observer
	}

	public int minimumSizeOffset = 100;
	private int minimumSize = 300;
	private CameraMode mode;

	Camera thisCamera;
	SyncController syncController;
	MapController mapController;
	Player player;
	Player[] players;

	private float smoothVelocity;
	
	void Awake () {
		syncController = GameObject.FindObjectOfType<SyncController>();
		mapController = GameObject.FindObjectOfType<MapController>();

		thisCamera = GetComponent<Camera>();
		thisCamera.orthographicSize = minimumSize;

		mode = CameraMode.ToStart;
	}
	
	// Update is called once per frame
	void Update () {
		if(mode == CameraMode.ToStart) {
			if(mapController.started) {
				mode = CameraMode.Game;
				minimumSize = (int) (mapController.GetSize() / 2) + minimumSizeOffset;
			}
			return;
		}

		float dist = 0f;
		if(mode == CameraMode.Game) {
			if(player == null) {
				player = syncController.GetPlayer();
				return;
			}
			dist = Vector2.Distance(mapController.GetPosition(), player.transform.position) * 1.3f;
		} else if(mode == CameraMode.Observer) {
			foreach(Player pl in players) {
				if(pl.Status != "alive") continue;
				
				float plDist = Vector2.Distance(mapController.GetPosition(), pl.transform.position) * 1.3f;				
				if(plDist > dist) dist = plDist;
			}
		}
		
		if(dist > minimumSize) thisCamera.orthographicSize = Mathf.SmoothDamp(thisCamera.orthographicSize, dist, ref smoothVelocity, 1f);
		else thisCamera.orthographicSize = Mathf.SmoothDamp(thisCamera.orthographicSize, minimumSize, ref smoothVelocity, 1f);

	}

	public void SetObserver() {
		if(mode == CameraMode.Observer) return;

		players = GameObject.FindObjectsOfType<Player>();
		mode = CameraMode.Observer;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

	public GameObject obstaclePrefab;

	private Vector3 center;
	private Transform obstaclesParent;
	private Transform ground;
	private float decreasePerSecond;
	private float size;
	private float halfSize;

	void Awake () {
		obstaclesParent = transform.Find ("Obstacles");
		ground = transform.Find ("Ground");
		decreasePerSecond = 0;
	}

	void Update () {
		this.size -= decreasePerSecond * Time.deltaTime;
		this.halfSize = size / 2;
		ground.localScale = new Vector2 (this.size, this.size);
	}

	public void UpdateMap (JSONObject data) {
		this.size = data["size"].n;
		this.halfSize = size / 2;
		ground.localScale = new Vector2 (this.size, this.size);

		float xMapPos = data["position"]["x"].n;
		float yMapPos = data["position"]["y"].n;
		ground.position = new Vector2 (xMapPos, yMapPos);

		this.decreasePerSecond = data["decreasePerSecond"].n;
	}

	public void CreateMap (JSONObject data) {
		this.size = data["size"].n;
		this.halfSize = size / 2;
		ground.localScale = new Vector2 (this.size, this.size);

		this.decreasePerSecond = data["decreasePerSecond"].n;

		float xMapPos = data["position"]["x"].n;
		float yMapPos = data["position"]["y"].n;
		ground.position = new Vector2 (xMapPos, yMapPos);

		List<JSONObject> obstaclesReceived = data["obstacles"].list;
		for (int i = 0; i < obstaclesReceived.Count; i++) {
			float xPos = obstaclesReceived[i]["position"]["x"].n;
			float yPos = obstaclesReceived[i]["position"]["y"].n;
			Vector3 pos = new Vector2 (xPos, yPos);

			float obsSize = obstaclesReceived[i]["collider"]["size"].n;

			GameObject obs = Instantiate (obstaclePrefab, pos, Quaternion.identity) as GameObject;
			obs.transform.parent = obstaclesParent;
			obs.transform.localScale = new Vector2 (obsSize, obsSize);
		}

		Vector3 cameraPosition = ground.position;
		cameraPosition.z = -10f;

		Camera.main.transform.position = cameraPosition;
		Camera.main.orthographicSize = Mathf.Max(this.halfSize * 1.3f, 200);
	}

	public Vector3 GetPosition() {
		return ground.position;
	}
}
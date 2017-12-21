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
		ground.localScale = new Vector3 (this.size, 1, this.size);
	}

	public void UpdateMap (JSONObject data) {
		this.size = data["size"].n;
		this.halfSize = size / 2;
		ground.localScale = new Vector3 (this.size, 1, this.size);

		float xMapPos = data["position"]["x"].n;
		float yMapPos = data["position"]["y"].n;
		ground.position = new Vector3 (xMapPos, 0, yMapPos);

		this.decreasePerSecond = data["decreasePerSecond"].n;
	}

	public void CreateMap (JSONObject data) {
		this.size = data["size"].n;
		this.halfSize = size / 2;
		ground.localScale = new Vector3 (this.size, 1, this.size);

		this.decreasePerSecond = data["decreasePerSecond"].n;

		float xMapPos = data["position"]["x"].n;
		float yMapPos = data["position"]["y"].n;
		ground.position = new Vector3 (xMapPos, 0, yMapPos);

		List<JSONObject> obstaclesReceived = data["obstacles"].list;
		for (int i = 0; i < obstaclesReceived.Count; i++) {
			float xPos = obstaclesReceived[i]["position"]["x"].n;
			float yPos = obstaclesReceived[i]["position"]["y"].n;
			Vector3 pos = new Vector3 (xPos, 5, yPos);

			float obsSize = obstaclesReceived[i]["collider"]["size"].n;

			GameObject obs = Instantiate (obstaclePrefab, pos, Quaternion.identity) as GameObject;
			obs.transform.parent = obstaclesParent;
			obs.transform.localScale = new Vector3 (obsSize, 10, obsSize);
		}

	}
}
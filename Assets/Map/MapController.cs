using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

	public GameObject obstaclePrefab;

    private Vector3 center;
	private Transform obstaclesParent;
    private Transform ground;

	void Awake () {
		obstaclesParent = transform.Find("Obstacles");
		ground = transform.Find("Ground");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CreateMap(JSONObject data) {
        float size = data["size"].n;
        float halfSize = size / 2;
        ground.localScale = new Vector2(size, size);
        ground.position = new Vector2(halfSize, halfSize);

        List<JSONObject> obstaclesReceived = data["obstacles"].list;
        for (int i = 0; i < obstaclesReceived.Count; i++)
        {
            float xPos = obstaclesReceived[i]["position"]["x"].n;
            float yPos = obstaclesReceived[i]["position"]["y"].n;
			Vector3 pos = new Vector2(xPos, yPos);

			float obsSize = obstaclesReceived[i]["collider"]["size"].n;

            GameObject obs = Instantiate(obstaclePrefab, pos, Quaternion.identity) as GameObject;
            obs.transform.parent = obstaclesParent;
			obs.transform.localScale = new Vector2(obsSize, obsSize);
        }

		Vector3 cameraPosition = ground.position;
		cameraPosition.z = -10f;

		Camera.main.transform.position = cameraPosition;
		Camera.main.orthographicSize = halfSize * 1.3f;
	}
}

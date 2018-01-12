using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : Map {

	public GameObject blockPrefab;

	private Transform blocksParent;
	private List<BlockGrid> blocks;

	private float size;
	private Vector3 position;

	void Awake () {
		blocksParent = transform.Find ("Blocks");

		blocks = new List<BlockGrid>();
	}
	
	void Update () {
		
	}

	public override void UpdateMap (JSONObject data) {
		Debug.Log(data);
		List<JSONObject> blocksReceived = data["blocks"].list;
		for (int i = 0; i < blocksReceived.Count; i++) {
			float xPos = blocksReceived[i]["position"]["x"].n;
			float yPos = blocksReceived[i]["position"]["y"].n;
			Vector3 pos = new Vector2 (xPos, yPos);

			blocks[i].SetData(i, blocksReceived[i]);
		}
	}

	public override void CreateMap (JSONObject data) {
		this.size = data["size"].n;

		float xMapPos = data["position"]["x"].n;
		float yMapPos = data["position"]["y"].n;
		position = new Vector2 (xMapPos, yMapPos);

		List<JSONObject> blocksReceived = data["blocks"].list;
		for (int i = 0; i < blocksReceived.Count; i++) {
			float xPos = blocksReceived[i]["position"]["x"].n;
			float yPos = blocksReceived[i]["position"]["y"].n;
			Vector3 pos = new Vector2 (xPos, yPos);

			float blkSize = blocksReceived[i]["size"].n;

			BlockGrid blk = Instantiate (blockPrefab, pos, Quaternion.identity).GetComponent<BlockGrid>();
			blk.transform.parent = blocksParent;
			blk.transform.localScale = new Vector2 (blkSize, blkSize);

			blk.SetData(i, blocksReceived[i]);
			blocks.Add(blk);
		}
		
		Vector3 cameraPosition = position;
		cameraPosition.z = -10f;
		Camera.main.transform.position = cameraPosition;
	}

	public override Vector3 GetPosition() {
		return position;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

	public List<MapItem> mapsAvailable;
	private Map currentMap;

	public bool started {
		get {
			return currentMap != null;
		}
	}

	void Awake () {

	}

	void Update () {
		
	}

	public void UpdateMap (JSONObject data) {
		currentMap.UpdateMap(data);
	}

	public void CreateMap (JSONObject data) {
		MapItem mapSelected = mapsAvailable.Find(x => x.mapName == data["name"].str);
		currentMap = Instantiate(mapSelected.prefab).GetComponent<Map>();
		currentMap.CreateMap(data);
	}

	public Vector3 GetPosition() {
		return currentMap.GetPosition();
	}

	public float GetSize() {
		return currentMap.GetSize();
	}
}
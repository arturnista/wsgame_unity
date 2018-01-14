using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

	public virtual void UpdateMap (JSONObject data) {
		
	}

	public virtual void CreateMap (JSONObject data) {
		
	}

	public virtual Vector3 GetPosition() {
		return Vector3.zero;
	}

	public virtual float GetSize() {
		return 0;
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User {

	public string status;
	public Color color;

	public void SetData (JSONObject data) {
		ColorUtility.TryParseHtmlString(data["color"].str, out this.color);
		this.status = data["status"].str;
	}
}

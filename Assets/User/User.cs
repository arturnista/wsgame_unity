using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User {

	public string id;
	public string name;
	public bool isOwner;
	public string status;
	public Color color;

	public User (JSONObject data, bool isOwner) {
		this.id = data["id"].str;
		ColorUtility.TryParseHtmlString(data["color"].str, out this.color);
		this.status = data["status"].str;
		this.name = data["name"].str;
		this.isOwner = isOwner;
	}
}

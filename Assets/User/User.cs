using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSpell {
	public string name;
	public int uses;

	public UserSpell(string n) {
		name = n;
		uses = 0;
	}
}

public class User {

	public string id;
	public string name;
	public bool isOwner;
	public string status;
	public Color color;
	public int winCount;
    public List<UserSpell> spells;

	public User (JSONObject data, bool isOwner) {
		this.id = data["id"].str;
		ColorUtility.TryParseHtmlString(data["color"].str, out this.color);
		this.status = data["status"].str;
		this.name = data["name"].str;
		this.winCount = (int) data["winCount"].n;
		this.isOwner = isOwner;

		this.spells = new List<UserSpell>();
	}

	public void Update(JSONObject data) {
		ColorUtility.TryParseHtmlString(data["color"].str, out this.color);
		this.status = data["status"].str;
		this.name = data["name"].str;
		this.winCount = (int) data["winCount"].n;
	}

	public void Reset(JSONObject data) {
		this.Update(data);
		this.spells = new List<UserSpell>();
	}

	public void AddSpell(string name) {
		Debug.Log("Nome: " + name);

		UserSpell us = spells.Find(x => x.name == name);
		if(us == null) spells.Add( new UserSpell(name) );
	}

	public void RemoveSpell(string name) {
		Debug.Log("Nome: " + name);

		UserSpell us = spells.Find(x => x.name == name);
		if(us != null) spells.Remove( us );
	}

	public void UseSpell(string name) {
		Debug.Log("Spell: " + name);
		
		UserSpell us = spells.Find(x => x.name == name);
		if(us != null) us.uses++;
	}
}

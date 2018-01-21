using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellsController : MonoBehaviour {

	public List<SpellItem> spells;

	void Awake () {
		DontDestroyOnLoad(this.gameObject);
	}
	
	void Update () {
		
	}

	public void SetSpellData(string spellName, JSONObject data) {
		if(!data.HasField("name")) return;

		SpellItem spell = spells.Find(x => x.name == spellName);

		if(spell == null) {
			Debug.LogError("Spell " + spellName + " not founded!");
			return;
		}

		spell.showName = data["name"].str;
		spell.description = data["description"].str;
		spell.type = data["type"].str;
		spell.cooldown = data["cooldown"].n;
		if(data.HasField("knockbackMultiplier")) spell.knockbackMultiplier = data["knockbackMultiplier"].n;
		if(data.HasField("knockbackIncrement")) spell.knockbackIncrement = data["knockbackIncrement"].n;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSpellIcon : MonoBehaviour {

	public string spellName;

	private Image spellImage;
	private float cooldown;

	private bool spellAvailable = true;
	private float useTime;

	void Awake () {
		spellImage = GetComponentInChildren<Image>();
	}

	void Update() {
		if(spellAvailable) return;
		float alpha = (Time.time - useTime) / cooldown;
		spellImage.color = new Color(.3f, .3f, 1f, alpha);
		if(alpha >= 1) {
			spellAvailable = true;
			spellImage.color = Color.white;
		}
	}

	public void SetData (SpellItem spell) {
		spellName = spell.name;
		spellImage.sprite = spell.image;

		cooldown = spell.cooldown / 1000;
	}

	public void UseSpell() {
		spellAvailable = false;
		useTime = Time.time;
	}
	
}

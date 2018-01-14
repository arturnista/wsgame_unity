using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDestroy : Spell {

	public Color color;
	public float duration = 1f;
	private float startTime;

	private float initialSize = 0f;
	private float fullSize = 100f;
	private float currentSize;

	private SpriteRenderer durationSprite;
	private SpriteRenderer areaSprite;

	void Awake () {
		durationSprite = transform.Find("DurationSprite").GetComponent<SpriteRenderer>();
		areaSprite = transform.Find("AreaSprite").GetComponent<SpriteRenderer>();

		currentSize = initialSize;

		areaSprite.transform.localScale = new Vector3(fullSize, fullSize);
		durationSprite.transform.localScale = new Vector3(initialSize, initialSize);

		areaSprite.color = new Color(color.r, color.g, color.b, .3f);
		durationSprite.color = color;
	}

	protected override void Start() {
		base.Start();

		startTime = Time.time;		
	}
	
	protected override void Update () {
		base.Update();

		float t = ( Time.time - startTime ) / duration;
		currentSize = Mathf.Lerp(initialSize, fullSize, t);
		durationSprite.transform.localScale = new Vector3(currentSize, currentSize);
	}

	public override void SetData(JSONObject data) {
		base.SetData(data);

		fullSize = data["radius"].n;
		initialSize = 0f;
		currentSize = initialSize;

		areaSprite.transform.localScale = new Vector3(fullSize, fullSize);
		durationSprite.transform.localScale = new Vector3(initialSize, initialSize);

		startTime = Time.time;	
		duration = data["duration"].n / 1000f;	
		GetComponent<Lifespan>().time = duration;
    }
}

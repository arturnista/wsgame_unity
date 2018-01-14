using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncObject : MonoBehaviour
{

    public string id;

    protected Vector3 velocity;

    // Use this for initialization
    protected virtual void Start() {

    }

    // Update is called once per frame
    protected virtual void Update() {
        transform.position += this.velocity * Time.deltaTime;
    }

    public virtual void SetData(JSONObject data) {
        this.id = data["id"].str;

        float xPos = data["position"]["x"].n;
        float yPos = data["position"]["y"].n;
        transform.position = new Vector2(xPos, yPos);

        if(data.HasField("velocity")) {
            float xVel = data["velocity"]["x"].n;
            float yVel = data["velocity"]["y"].n;
            this.velocity = new Vector2(xVel, yVel);
        }

        if(data.HasField("collider")) {
            float size = data["collider"]["size"].n;
            transform.localScale = new Vector2(size, size);
        }
    }
}

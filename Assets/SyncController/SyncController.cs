﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class SyncController : MonoBehaviour {

	public GameObject playerPrefab;
    public GameObject fireballPrefab;

    private MapController mapController;

	private List<Player> playersList;
    private List<Spell> spellsList;
	private SocketIOComponent socket;

	private string playerId;

	public void Awake() {
        mapController = GameObject.FindObjectOfType<MapController>();
		playersList = new List<Player>();
        spellsList = new List<Spell>();
		socket = GetComponent<SocketIOComponent>();
	}

    public void Start() {
		socket.On("open", Open);
        socket.On("player_create", PlayerCreated);

        socket.On("map_create", CreateMap);
        socket.On("map_update", UpdateMap);

		socket.On("sync", Sync);

		socket.On("gameobject_delete", DeleteObject);

        socket.On("error", Error);
        socket.On("close", Close);
    }

	void Open(SocketIOEvent e) {
		Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
	}

    void PlayerCreated(SocketIOEvent e) {
        this.playerId = e.data["id"].str;
    }

    void CreateMap(SocketIOEvent e) {
        mapController.CreateMap(e.data);
    }

    void UpdateMap(SocketIOEvent e) {
        mapController.UpdateMap(e.data);
    }

    void Sync(SocketIOEvent e) {
		List<JSONObject> receivedPlayersList = e.data["players"].list;
		for (int i = 0; i < receivedPlayersList.Count; i++) {
			JSONObject player = receivedPlayersList[i];

			Player playerInList = playersList.Find(x => x.id == player["id"].str);
            if(playerInList == null) {
                GameObject go = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity) as GameObject;
				playerInList = go.GetComponent<Player>();
				playersList.Add(playerInList);
			}
            playerInList.SetData( player );
        }

        List<JSONObject> receivedSpellsList = e.data["spells"].list;
        for (int i = 0; i < receivedSpellsList.Count; i++)
        {
            JSONObject spell = receivedSpellsList[i];
            Spell spellOnList = spellsList.Find(x => x.id == spell["id"].str);
            if (spellOnList == null) {
                if(spell["type"].str == "fireball") {
                    GameObject go = Instantiate(fireballPrefab, Vector2.zero, Quaternion.identity) as GameObject;
                    spellOnList = go.GetComponent<Spell>();
                }
                spellsList.Add(spellOnList);
            }
            spellOnList.SetData(spell);
        }
    }

	void DeleteObject(SocketIOEvent e) {
		Player playerInList = playersList.Find(x => x.id == e.data["id"].str);
		if(playerInList) {
            playersList.Remove(playerInList);
		    Destroy(playerInList.gameObject);
        }

		Spell spellOnList = spellsList.Find(x => x.id == e.data["id"].str);
		if(spellOnList) {
            spellsList.Remove(spellOnList);
		    Destroy(spellOnList.gameObject);
        }
	}

	void Error(SocketIOEvent e) {
		Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
	}

	void Close(SocketIOEvent e) {
		Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
	}

	public Player GetPlayer() {
        Player playerInList = playersList.Find(x => x.id == this.playerId);
		return playerInList;
    }

    public void RestartGame() {
        JSONObject data = new JSONObject();
        data.AddField("id", this.playerId);
        socket.Emit("game_restart", data);        
    }

    public void MovePlayer(Vector2 position) {
		JSONObject data = new JSONObject();
		JSONObject positionJson = new JSONObject();

        positionJson.AddField("x", position.x);
        positionJson.AddField("y", position.y);

		data.AddField("position", positionJson);
        data.AddField("id", this.playerId);

        socket.Emit("player_move", data);
    }

    public void UseFireball(Vector2 position, Vector2 direction) {
        JSONObject data = new JSONObject();
        JSONObject positionJson = new JSONObject();

        positionJson.AddField("x", position.x);
        positionJson.AddField("y", position.y);

        JSONObject directionJson = new JSONObject();

        directionJson.AddField("x", direction.x);
        directionJson.AddField("y", direction.y);

        data.AddField("id", this.playerId);
        data.AddField("position", positionJson);
        data.AddField("direction", directionJson);

        socket.Emit("player_spell_fireball", data);
    }
}

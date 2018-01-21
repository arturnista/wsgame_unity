using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;
using UnityEngine.Networking;

public class SyncController : MonoBehaviour {

    private string serverUrl;

	public GameObject playerPrefab;
    public GameObject fireballPrefab;
    public GameObject followerPrefab;
    public GameObject boomerangPrefab;
    public GameObject explosionPrefab;

    private GameUIController gameUIController;
    private MenuUIController menuUIController;
    private MapController mapController;
    private CameraBehavior cameraBehavior;
    private ActionController actionController;
    private SpellsController spellsController;

	private List<Player> playersList;
    private List<Spell> spellsList;
	private SocketIOComponent socket;

    private List<User> usersInRoom;
    private Color userColor;

    private string roomName;
    private string userName;
    private bool isGameRunning = false;

	private string playerId;
	private string userId;
    private bool isPlayerAlive;

    private List<string> spellsSelected;

	public void Awake() {
        DontDestroyOnLoad(this.gameObject);

        spellsController = GameObject.FindObjectOfType<SpellsController>();

		playersList = new List<Player>();
        spellsList = new List<Spell>();
        spellsSelected = new List<string>();
		socket = GetComponent<SocketIOComponent>();

        roomName = "";
        userName = PlayerPrefs.GetString("Name", "");
        serverUrl = socket.url;
	}

    public void Start() {
		socket.On("open", Open);
        socket.On("error", Error);
        socket.On("close", Close);

		socket.On("myuser_info", DefineMyUserInfo);
        socket.On("myuser_joined_room", MyUserJoinedRoom);

        socket.On("user_joined_room", UserJoinedRoom);
        socket.On("user_ready", UserReady);
        socket.On("user_waiting", UserWaiting);
        socket.On("user_selected_spell", UserSelectSpell);
        socket.On("user_deselected_spell", UserDeselectSpell);
        socket.On("user_left_room", UserLeftRoom);

		socket.On("game_will_start", GameWillStart);
		socket.On("game_start", GameStart);
        
		socket.On("game_will_end", GameWillEnd);
		socket.On("game_end", GameEnd);

        socket.On("player_create", PlayerCreated);
        socket.On("player_use_spell", PlayerUseSpell);

        socket.On("map_create", CreateMap);
        socket.On("map_update", UpdateMap);

		socket.On("sync", Sync);

		socket.On("gameobject_delete", DeleteObject);

        SceneManager.activeSceneChanged += OnChangeLevel;
        StartCoroutine(GetSpells());
        StartCoroutine(GetRooms());
    }
 
    IEnumerator GetSpells() {
        UnityWebRequest www = UnityWebRequest.Get(serverUrl + "/spells");
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            JSONObject spells = new JSONObject(www.downloadHandler.text);
            foreach (string sKey in spells.keys) {
                JSONObject sData = spells[sKey];
                spellsController.SetSpellData(sKey, sData);
            }
        }
    }
 
    IEnumerator GetRooms() {
        UnityWebRequest www = UnityWebRequest.Get(serverUrl + "/rooms");
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            JSONObject rooms = new JSONObject(www.downloadHandler.text);
            Debug.Log("Rooms: " + rooms);
        }
    }

    private void OnChangeLevel(Scene origin, Scene current) {
        
        if(current.name == "Game") {
            gameUIController = GameObject.FindObjectOfType<GameUIController>();
            this.isGameRunning = true;
            mapController = GameObject.FindObjectOfType<MapController>();
            cameraBehavior = GameObject.FindObjectOfType<CameraBehavior>();
            actionController = GameObject.FindObjectOfType<ActionController>();
            actionController.SetSpells(spellsSelected);
        } else if(current.name == "Menu") {
            menuUIController = GameObject.FindObjectOfType<MenuUIController>();
            menuUIController.UserStatusUpdate(usersInRoom);
            menuUIController.SetUserName(userName);
            if(userColor != null) menuUIController.SetPlayerColor(userColor);
        }

    }

	void Open(SocketIOEvent e) {
		Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
	}

	void Error(SocketIOEvent e) {
		Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
	}

	void Close(SocketIOEvent e) {
		Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
	}

	void DefineMyUserInfo(SocketIOEvent e) {
		Debug.Log("[SocketIO] User created");
        this.userId = e.data["id"].str;
	}

    void MyUserJoinedRoom(SocketIOEvent e) {
		Debug.Log("[SocketIO] User joined room");
        string ownerId = e.data["room"]["owner"]["id"].str;
        bool isUserRoomOwner = this.userId == ownerId;
        this.roomName = e.data["room"]["name"].str;
        menuUIController.MyUserJoinedRoom(this.roomName, isUserRoomOwner);
        
        usersInRoom = new List<User>();
        foreach (JSONObject uData in e.data["room"]["users"].list) {
            User u = new User(uData, ownerId == uData["id"].str);
            usersInRoom.Add(u);    
        }
        menuUIController.UserStatusUpdate(usersInRoom);
        
		ColorUtility.TryParseHtmlString(e.data["user"]["color"].str, out this.userColor);
        menuUIController.SetPlayerColor(userColor);
    }

    void UserJoinedRoom(SocketIOEvent e) {
        if(this.userId == e.data["user"]["id"].str) return;

        User user = new User(e.data["user"], e.data["room"]["owner"]["id"].str == e.data["user"]["id"].str);
        usersInRoom.Add(user);

        menuUIController.UserStatusUpdate(usersInRoom);
    }

    void UserReady(SocketIOEvent e) {
        User userInList = usersInRoom.Find(x => x.id == e.data["user"].str);
        userInList.status = "ready";
        menuUIController.UserStatusUpdate(usersInRoom);
    }

    void UserWaiting(SocketIOEvent e) {
        User userInList = usersInRoom.Find(x => x.id == e.data["user"].str);
        userInList.status = "waiting";
        menuUIController.UserStatusUpdate(usersInRoom);
    }

    void UserSelectSpell(SocketIOEvent e) {
        string uid = e.data["user"].str;
        string spellName = e.data["spellName"].str;

        if(uid != this.userId) {
            User u = usersInRoom.Find(x => x.id == uid);
            u.spells.Add(spellName);
            return;
        }

        spellsSelected.Add(spellName);
        menuUIController.SelectSpell(e.data, spellsSelected.Count - 1);
    }

    void UserDeselectSpell(SocketIOEvent e) {
        string uid = e.data["user"].str;
        string spellName = e.data["spellName"].str;

        if(uid != this.userId) {
            User u = usersInRoom.Find(x => x.id == uid);
            u.spells.Remove(spellName);
            return;
        }
        
        spellsSelected.Remove(spellName);
        menuUIController.DeselectSpell(spellName);
    }

    void UserLeftRoom(SocketIOEvent e) {
        User userInList = usersInRoom.Find(x => x.id == e.data["id"].str);
        usersInRoom.Remove(userInList);
        menuUIController.UserStatusUpdate(usersInRoom);
    }

    void GameWillStart(SocketIOEvent e) {
		Debug.Log("[SocketIO] Game will start");
        SceneManager.LoadScene("Game");    
    }

    void GameStart(SocketIOEvent e) {
		Debug.Log("[SocketIO] Game start");
        this.isPlayerAlive = true;
    }

    void GameWillEnd(SocketIOEvent e) {
		Debug.Log("[SocketIO] Game will end");
        bool winner = this.playerId == e.data["winner"]["id"].str;
        gameUIController.EndGame(winner);
    }

    void GameEnd(SocketIOEvent e) {
		Debug.Log("[SocketIO] Game ended");
        
        foreach (JSONObject uData in e.data["users"].list) {
            User u = usersInRoom.Find(x => x.id == uData["id"].str);
            if(u != null) u.Update(uData);
        }
        
        this.isGameRunning = false;
        SceneManager.LoadScene("Menu");    

        spellsSelected = new List<string>();
        menuUIController.UserStatusUpdate(usersInRoom);

    }

    void PlayerCreated(SocketIOEvent e) {
		Debug.Log("[SocketIO] Player created");
        this.playerId = e.data["id"].str;
    }

    void PlayerUseSpell(SocketIOEvent e) {
        string spellName = e.data["spellName"].str;
        gameUIController.UseSpell(spellName);

        Vector3 position = JSONTemplates.ToVector2(e.data["position"]);

        switch (spellName) {
            case "explosion":
                Spell s = Instantiate(explosionPrefab, position, Quaternion.identity).GetComponent<Spell>();
                s.SetData(e.data);
                break;
        }
    }

    void CreateMap(SocketIOEvent e) {
        mapController.CreateMap(e.data);
    }

    void UpdateMap(SocketIOEvent e) {
        if(!isGameRunning) return;
        
        mapController.UpdateMap(e.data);
    }

    void Sync(SocketIOEvent e) {
        if(!isGameRunning) return;

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

            if(player["id"].str == this.playerId) {
                if(this.isPlayerAlive && player["status"].str != "alive") {
                    this.isPlayerAlive = false;
                    cameraBehavior.SetObserver();
                }
            }
        }

        List<JSONObject> receivedSpellsList = e.data["spells"].list;
        for (int i = 0; i < receivedSpellsList.Count; i++)
        {
            JSONObject spell = receivedSpellsList[i];
            Spell spellOnList = spellsList.Find(x => x.id == spell["id"].str);
            if (spellOnList == null) {
                string spellType = spell["type"].str;
                if(spellType == "fireball") {
                    spellOnList = Instantiate(fireballPrefab, JSONTemplates.ToVector2(spell["position"]), Quaternion.identity).GetComponent<Spell>();
                } else if(spellType == "follower") {
                    spellOnList = Instantiate(followerPrefab, JSONTemplates.ToVector2(spell["position"]), Quaternion.identity).GetComponent<Spell>();
                } else if(spellType == "boomerang") {
                    spellOnList = Instantiate(boomerangPrefab, JSONTemplates.ToVector2(spell["position"]), Quaternion.identity).GetComponent<Spell>();
                }
                spellsList.Add(spellOnList);
            }
            spellOnList.SetData(spell);
        }
    }

	void DeleteObject(SocketIOEvent e) {
        if(!isGameRunning) return;

		Spell spellOnList = spellsList.Find(x => x.id == e.data["id"].str);
		if(spellOnList) {
            spellsList.Remove(spellOnList);
            spellOnList.Destroy();
        }
	}

	public Player GetPlayer() {
        Player pp = playersList.Find(x => x.id == this.playerId);
		return pp;
    }

	public User GetUser() {
        if(usersInRoom == null) return null;
        User us = usersInRoom.Find(x => x.id == this.userId);
		return us;
    }

    public void SetRoomName(string name) {
        this.roomName = name;
    }

    public void SetUserName(string name) {
        this.userName = name;
    }

    public string GetRoomName() {
        return this.roomName;
    }

    public void CreateGame() {
        JSONObject data = new JSONObject();
        data.AddField("name", roomName);
        data.AddField("userName", userName);
        socket.Emit("room_create", data); 

        PlayerPrefs.SetString("Name", userName);
    }

    public void JoinGame() {
        JSONObject data = new JSONObject();
        data.AddField("name", roomName);
        data.AddField("userName", userName);
        socket.Emit("room_join", data);  

        PlayerPrefs.SetString("Name", userName);     
    }

    public void ToggleReady() {
        User us = this.GetUser();
        if(us.status == "ready") {
            socket.Emit("user_waiting");
        } else {
            socket.Emit("user_ready");            
        }
    }

    public void SelectSpell(string spellName) {
		JSONObject data = new JSONObject();
        data.AddField("spellName", spellName);

        socket.Emit("user_select_spell", data);
    }

    public void DeselectSpell(string spellName) {
		JSONObject data = new JSONObject();
        data.AddField("spellName", spellName);

        socket.Emit("user_deselect_spell", data);
    }

    public List<SpellItem> GetSpellsSelected() {
        List<SpellItem> spellSel = new List<SpellItem>();
        foreach(string name in spellsSelected) {
            SpellItem s = spellsController.spells.Find(x => name == x.name);
            spellSel.Add(s);
        }
        return spellSel;
    }

    public void StartGame(string mapName) {
		JSONObject data = new JSONObject();
        if(mapName.Length > 0) data.AddField("map", mapName);

        socket.Emit("game_start", data);
    }

    public void MovePlayer(Vector2 position) {
        if(!isGameRunning) return;

		JSONObject data = new JSONObject();
		JSONObject positionJson = new JSONObject();

        positionJson.AddField("x", position.x);
        positionJson.AddField("y", position.y);

		data.AddField("position", positionJson);
        data.AddField("id", this.playerId);

        socket.Emit("player_move", data);
    }

    public void UseSpell(string spellName, Vector2 position, Vector2 direction) {
        if(!isGameRunning) return;
        
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

        socket.Emit("player_spell_" + spellName, data);
    }

}

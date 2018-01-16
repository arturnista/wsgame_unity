using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using socket.io;

public class SyncController : MonoBehaviour {

	public GameObject playerPrefab;
    public GameObject fireballPrefab;
    public GameObject followerPrefab;
    public GameObject explosionPrefab;

    private UIController uiController;
    private MapController mapController;
    private CameraBehavior cameraBehavior;
    private ActionController actionController;

	private List<Player> playersList;
    private List<Spell> spellsList;
	private Socket socket;

    public bool isUserInRoom = false;
    public bool isUserRoomOwner = false;
    public bool isUserReady = false;
    private List<User> usersInRoom;
    private Color userColor;

    private int _called;

    private string roomName;
    private string userName;
    private bool isGameRunning = false;

	private string playerId;
	private string userId;
    private bool isPlayerAlive;

    private List<string> spellsSelected;

	public void Awake() {
        DontDestroyOnLoad(this.gameObject);

		playersList = new List<Player>();
        spellsList = new List<Spell>();
        spellsSelected = new List<string>();
        string serverUrl = "http://localhost:5002";
        socket = Socket.Connect(serverUrl);

        roomName = "";
        userName = PlayerPrefs.GetString("Name", "");
	}

    public void Start() {
		socket.On(SystemEvents.connect, Open);
        // socket.On(SystemEvents.connectError, Error);
        socket.On(SystemEvents.disconnect, Close);

		socket.On("myuser_info", DefineMyUserInfo);
		socket.On("myuser_rooms", SetRoomsAvailable);
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

        _called = 0;
		socket.On("sync", Sync);

		socket.On("gameobject_delete", DeleteObject);

        SceneManager.activeSceneChanged += OnChangeLevel;
    }

    private void OnChangeLevel(Scene origin, Scene current) {
        uiController = GameObject.FindObjectOfType<UIController>();
        
        if(current.name == "Game") {
            this.isGameRunning = true;
            mapController = GameObject.FindObjectOfType<MapController>();
            cameraBehavior = GameObject.FindObjectOfType<CameraBehavior>();
            actionController = GameObject.FindObjectOfType<ActionController>();
            actionController.SetSpells(spellsSelected);
        } else if(current.name == "Menu") {
            uiController.UserStatusUpdate(usersInRoom);
            uiController.SetUserName(userName);
            if(userColor != null) uiController.SetPlayerColor(userColor);
        }

    }

	void Open() {
		Debug.Log("[SocketIO] Open received");
	}

	void Error() {
		Debug.Log("[SocketIO] Error received");
	}

	void Close() {
		Debug.Log("[SocketIO] Close received");
	}

	void DefineMyUserInfo(string e) {
        JSONObject data = new JSONObject(e);
		Debug.Log("[SocketIO] User created " + data);
        this.userId = data["id"].str;
	}

    void SetRoomsAvailable(string e) {
        JSONObject data = new JSONObject(e);
		Debug.Log("[SocketIO] " + data["rooms"]);
        
    }

    void MyUserJoinedRoom(string e) {
		Debug.Log("[SocketIO] User joined room " + e);
        JSONObject data = new JSONObject(e);
        string ownerId = data["room"]["owner"]["id"].str;
        this.isUserRoomOwner = this.userId == ownerId;
        this.roomName = data["room"]["name"].str;
        uiController.MyUserJoinedRoom(this.roomName, this.isUserRoomOwner);
        
        usersInRoom = new List<User>();
        foreach (JSONObject uData in data["room"]["users"].list) {
            User u = new User(uData, ownerId == uData["id"].str);
            usersInRoom.Add(u);            
        }
        uiController.UserStatusUpdate(usersInRoom);
        
		ColorUtility.TryParseHtmlString(data["user"]["color"].str, out this.userColor);
        uiController.SetPlayerColor(userColor);
        
        isUserInRoom = true;
    }

    void UserJoinedRoom(string e) {
        JSONObject data = new JSONObject(e);
        if(this.userId == data["user"]["id"].str) return;

        User user = new User(data["user"], data["room"]["owner"]["id"].str == data["user"]["id"].str);
        usersInRoom.Add(user);

        uiController.UserStatusUpdate(usersInRoom);
    }

    void UserReady(string e) {
        JSONObject data = new JSONObject(e);
        User userInList = usersInRoom.Find(x => x.id == data["user"].str);
        userInList.status = "ready";
        uiController.UserStatusUpdate(usersInRoom);
    }

    void UserWaiting(string e) {
        JSONObject data = new JSONObject(e);
        User userInList = usersInRoom.Find(x => x.id == data["user"].str);
        userInList.status = "waiting";
        uiController.UserStatusUpdate(usersInRoom);
    }

    void UserSelectSpell(string e) {
        JSONObject data = new JSONObject(e);
        string uid = data["user"].str;
        string spellName = data["spellName"].str;

        if(uid != this.userId) {
            User u = usersInRoom.Find(x => x.id == uid);
            u.spells.Add(spellName);
            return;
        }

        spellsSelected.Add(spellName);
        uiController.SelectSpell(data, spellsSelected.Count - 1);
    }

    void UserDeselectSpell(string e) {
        JSONObject data = new JSONObject(e);
        string uid = data["user"].str;
        string spellName = data["spellName"].str;

        if(uid != this.userId) {
            User u = usersInRoom.Find(x => x.id == uid);
            u.spells.Remove(spellName);
            return;
        }
        
        spellsSelected.Remove(spellName);
        uiController.DeselectSpell(spellName);
    }

    void UserLeftRoom(string e) {
        JSONObject data = new JSONObject(e);
        User userInList = usersInRoom.Find(x => x.id == data["id"].str);
        usersInRoom.Remove(userInList);
        uiController.UserStatusUpdate(usersInRoom);
    }

    void GameWillStart(string e) {
        JSONObject data = new JSONObject(e);
		Debug.Log("[SocketIO] Game will start");
        SceneManager.LoadScene("Game");    
    }

    void GameStart(string e) {
        JSONObject data = new JSONObject(e);
		Debug.Log("[SocketIO] Game start");
        this.isPlayerAlive = true;
    }

    void GameWillEnd(string e) {
        JSONObject data = new JSONObject(e);
		Debug.Log("[SocketIO] Game will end");
        bool winner = this.playerId == data["winner"]["id"].str;
        uiController.EndGame(winner);
    }

    void GameEnd(string e) {
        JSONObject data = new JSONObject(e);
		Debug.Log("[SocketIO] Game ended");
        this.isGameRunning = false;
        SceneManager.LoadScene("Menu");    
        this.isUserReady = false;

        spellsSelected = new List<string>();
        foreach (User u in usersInRoom) u.status = "waiting";
        uiController.UserStatusUpdate(usersInRoom);
    
    }

    void PlayerCreated(string e) {
        JSONObject data = new JSONObject(e);
		Debug.Log("[SocketIO] Player created");
        this.playerId = data["id"].str;
    }

    void PlayerUseSpell(string e) {
        JSONObject data = new JSONObject(e);
        string spellName = data["name"].str;

        float xPos = data["position"]["x"].n;
        float yPos = data["position"]["y"].n;
        Vector3 position = new Vector2(xPos, yPos);

        switch (spellName) {
            case "explosion":
                Spell s = Instantiate(explosionPrefab, position, Quaternion.identity).GetComponent<Spell>();
                s.SetData(data);
                break;
        }
    }

    void CreateMap(string e) {
        JSONObject data = new JSONObject(e);
        mapController.CreateMap(data);
    }

    void UpdateMap(string e) {
        JSONObject data = new JSONObject(e);
        if(!isGameRunning) return;
        
        mapController.UpdateMap(data);
    }

    void Sync(string e) {
        JSONObject data = new JSONObject(e);
        if(!isGameRunning) return;

        List<JSONObject> receivedPlayersList = data["players"].list;
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

        List<JSONObject> receivedSpellsList = data["spells"].list;
        for (int i = 0; i < receivedSpellsList.Count; i++)
        {
            JSONObject spell = receivedSpellsList[i];
            Spell spellOnList = spellsList.Find(x => x.id == spell["id"].str);
            if (spellOnList == null) {
                if(spell["type"].str == "fireball") {
                    float xPos = spell["position"]["x"].n;
                    float yPos = spell["position"]["y"].n;
                    
                    GameObject go = Instantiate(fireballPrefab, new Vector2(xPos, yPos), Quaternion.identity) as GameObject;
                    spellOnList = go.GetComponent<Spell>();
                }
                if(spell["type"].str == "follower") {
                    float xPos = spell["position"]["x"].n;
                    float yPos = spell["position"]["y"].n;
                    
                    GameObject go = Instantiate(followerPrefab, new Vector2(xPos, yPos), Quaternion.identity) as GameObject;
                    spellOnList = go.GetComponent<Spell>();
                }
                spellsList.Add(spellOnList);
            }
            spellOnList.SetData(spell);
        }
    }

	void DeleteObject(string e) {
        JSONObject data = new JSONObject(e);
        if(!isGameRunning) return;

		Spell spellOnList = spellsList.Find(x => x.id == data["id"].str);
		if(spellOnList) {
            spellsList.Remove(spellOnList);
            spellOnList.Destroy();
        }
	}

	public Player GetPlayer() {
        Player pp = playersList.Find(x => x.id == this.playerId);
		return pp;
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
        socket.EmitJson("room_create", data.ToString()); 

        PlayerPrefs.SetString("Name", userName);
    }

    public void JoinGame() {
        JSONObject data = new JSONObject();
        data.AddField("name", roomName);
        data.AddField("userName", userName);
        socket.EmitJson("room_join", data.ToString());  

        PlayerPrefs.SetString("Name", userName);     
    }

    public void ToggleReady() {
        if(isUserReady) {
            socket.Emit("user_waiting");
        } else {
            socket.Emit("user_ready");            
        }
        isUserReady = !isUserReady;
    }

    public void SelectSpell(string spellName) {
		JSONObject data = new JSONObject();
        data.AddField("spellName", spellName);

        socket.EmitJson("user_select_spell", data.ToString());
    }

    public void DeselectSpell(string spellName) {
		JSONObject data = new JSONObject();
        data.AddField("spellName", spellName);

        socket.EmitJson("user_deselect_spell", data.ToString());
    }

    public void StartGame(string mapName) {
		JSONObject data = new JSONObject();
        if(mapName.Length > 0) data.AddField("map", mapName);

        socket.EmitJson("game_start", data.ToString());
    }

    public void MovePlayer(Vector2 position) {
        if(!isGameRunning) return;

		JSONObject data = new JSONObject();
		JSONObject positionJson = new JSONObject();

        positionJson.AddField("x", position.x);
        positionJson.AddField("y", position.y);

		data.AddField("position", positionJson);
        data.AddField("id", this.playerId);

        socket.EmitJson("player_move", data.ToString());
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

        socket.EmitJson("player_spell_" + spellName, data.ToString());
    }

}

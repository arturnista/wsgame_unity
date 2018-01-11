using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;

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
	private SocketIOComponent socket;

    public bool isUserInRoom = false;
    public bool isUserRoomOwner = false;
    public bool isUserReady = false;
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

		playersList = new List<Player>();
        spellsList = new List<Spell>();
        spellsSelected = new List<string>();
		socket = GetComponent<SocketIOComponent>();

        roomName = "";
        userName = PlayerPrefs.GetString("Name", "");
	}

    public void Start() {
		socket.On("open", Open);
        socket.On("error", Error);
        socket.On("close", Close);

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

    void SetRoomsAvailable(SocketIOEvent e) {
		Debug.Log("[SocketIO] " + e.data["rooms"]);
        
    }

    void MyUserJoinedRoom(SocketIOEvent e) {
		Debug.Log("[SocketIO] User joined room");
        string ownerId = e.data["room"]["owner"]["id"].str;
        this.isUserRoomOwner = this.userId == ownerId;
        this.roomName = e.data["room"]["name"].str;
        uiController.MyUserJoinedRoom(this.roomName, this.isUserRoomOwner);
        
        usersInRoom = new List<User>();
        foreach (JSONObject uData in e.data["room"]["users"].list) {
            User u = new User(uData, ownerId == uData["id"].str);
            usersInRoom.Add(u);            
        }
        uiController.UserStatusUpdate(usersInRoom);
        
		ColorUtility.TryParseHtmlString(e.data["user"]["color"].str, out this.userColor);
        uiController.SetPlayerColor(userColor);
        
        isUserInRoom = true;
    }

    void UserJoinedRoom(SocketIOEvent e) {
        if(this.userId == e.data["user"]["id"].str) return;

        User user = new User(e.data["user"], e.data["room"]["owner"]["id"].str == e.data["user"]["id"].str);
        usersInRoom.Add(user);

        uiController.UserStatusUpdate(usersInRoom);
    }

    void UserReady(SocketIOEvent e) {
        User userInList = usersInRoom.Find(x => x.id == e.data["user"].str);
        userInList.status = "ready";
        uiController.UserStatusUpdate(usersInRoom);
    }

    void UserWaiting(SocketIOEvent e) {
        User userInList = usersInRoom.Find(x => x.id == e.data["user"].str);
        userInList.status = "waiting";
        uiController.UserStatusUpdate(usersInRoom);
    }

    void UserSelectSpell(SocketIOEvent e) {
        if(e.data["user"].str != this.userId) return;

        string spellName = e.data["spellName"].str;
        spellsSelected.Add(spellName);
        uiController.SelectSpell(spellName, spellsSelected.Count - 1);
    }

    void UserDeselectSpell(SocketIOEvent e) {
        if(e.data["user"].str != this.userId) return;
        
        string spellName = e.data["spellName"].str;
        spellsSelected.Remove(spellName);
        uiController.DeselectSpell(spellName);
    }

    void UserLeftRoom(SocketIOEvent e) {
        User userInList = usersInRoom.Find(x => x.id == e.data["id"].str);
        usersInRoom.Remove(userInList);
        uiController.UserStatusUpdate(usersInRoom);
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
        uiController.EndGame(winner);
    }

    void GameEnd(SocketIOEvent e) {
		Debug.Log("[SocketIO] Game ended");
        this.isGameRunning = false;
        SceneManager.LoadScene("Menu");    
        this.isUserReady = false;
        spellsSelected = new List<string>();
    }

    void PlayerCreated(SocketIOEvent e) {
		Debug.Log("[SocketIO] Player created");
        this.playerId = e.data["id"].str;
    }

    void PlayerUseSpell(SocketIOEvent e) {
        string spellName = e.data["name"].str;

        float xPos = e.data["player"]["position"]["x"].n;
        float yPos = e.data["player"]["position"]["y"].n;
        Vector3 position = new Vector2(xPos, yPos);

        switch (spellName) {
            case "explosion":
                Instantiate(explosionPrefab, position, Quaternion.identity);
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

        socket.Emit("user_select_spell", data);
    }

    public void DeselectSpell(string spellName) {
		JSONObject data = new JSONObject();
        data.AddField("spellName", spellName);

        socket.Emit("user_deselect_spell", data);
    }

    public void StartGame() {
        socket.Emit("game_start");
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

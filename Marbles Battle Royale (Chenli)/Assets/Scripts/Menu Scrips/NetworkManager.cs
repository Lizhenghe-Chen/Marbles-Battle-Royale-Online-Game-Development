using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using System.IO;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;
    PhotonView pV;
    [Tooltip("when In debug state, mark this so that no need to type room name to create room over and over again ")]
    public int maxRoomPlayers = 10;
    // [SerializeField] bool IsInDebugMode = false;
    public GameObject Start_Debug_Meun;
    public TextMeshProUGUI infoText;
    public string moreInfoURL = "https://doggychen.com/personal-unity-online-game-development";
    MenuManager MenuManager;
    RoomManager roomManager;
    public KeepSetting keepSetting;
    //================================================================
    public TMP_InputField roomNameInput;
    public TMP_InputField userNameInput;
    [SerializeField] TMP_Text ErrorText;
    //================================================================
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    //================================================================
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;
    //================================================================
    [SerializeField] GameObject startGameButton;
    [SerializeField] SoundController soundController;
    [SerializeField] TMP_Text startGameNotice, CreateRoomNotice;
    //========== when add a new menu page, enter new name manually and find their object================
    //[Header("when add a new menu page, enter new name manually and find their object\n")]
    [Tooltip("Find Automatically, when add a new menu page, add new name manually and find their object ")]
    public GameObject TitleMenu, LoadingMenu, CreateRoomMenu, RoomMenu, ErrorMenu, FindRoomMenu;

    // [SerializeField]
    // GameObject[]
    //     menus = { TitleMenu, LoadingMenu, RoomMenu, ErrorMenu, FindRoomMenu };
    void Awake()
    {
        pV = GetComponent<PhotonView>();
        Instance = this;
    }

    public int start = 0;

    void Start()
    {
        SetMenu();
        GameObject prefab = (GameObject)Resources.Load(Path.Combine("PhotonPrefabs", "Marble"), typeof(GameObject));//speed up for MenuManager
        PhotonNetwork.ConnectUsingSettings();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();

    }
    private void SetMenu()
    {
        MenuManager = this.GetComponent<MenuManager>();
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        keepSetting = GameObject.Find("KeepSetting").GetComponent<KeepSetting>();
        GameObject[] _menus = MenuManager.menuList;
        foreach (GameObject menu in _menus)
        {
            menu.SetActive(true);
        }

        TitleMenu = GameObject.Find(nameof(TitleMenu));
        LoadingMenu = GameObject.Find(nameof(LoadingMenu));
        CreateRoomMenu = GameObject.Find(nameof(CreateRoomMenu));
        RoomMenu = GameObject.Find(nameof(RoomMenu));
        ErrorMenu = GameObject.Find(nameof(ErrorMenu));
        FindRoomMenu = GameObject.Find(nameof(FindRoomMenu));
        MenuManager.OpenMenu(LoadingMenu);

        if (keepSetting.start >= 1)
        {
            userNameInput.text = keepSetting.playerName;
            roomNameInput.text = keepSetting.roomName;
            MenuManager.characterDetails.text = MenuManager.GenerateIntro(keepSetting.playerType);
            MenuManager.playerType = keepSetting.playerType;
            // MenuManager.PlayerSelection.text = "Selected Player: " + keepSetting.playerType;
            roomManager.showTutorialToggle.isOn = keepSetting.showTutorial;
        }
    }
    private void Update()
    {
        if (!PhotonNetwork.IsConnected)
        {
            ErrorText.text = "Opss! Check your internet connection and try again.";
            MenuManager.OpenConnectionFailedMenu(ErrorMenu);
            PhotonNetwork.ConnectUsingSettings();//try to reconnect
        }
        //  Debug.Log(PhotonNetwork.LevelLoadingProgress);
        //Debug.Log(PhotonNetwork.NetworkingClient);
    }
    // public void OnUserNameChanged()
    // {
    //     PhotonNetwork.NickName = userNameInput.text;
    //     if (userNameInput.text == null)
    //     {
    //         Debug.Log("null Player!!");
    //         userNameInput.text = "Player" + Random.Range(0, 99).ToString("00");
    //     }
    // }

    //================================================================
    public override void OnConnectedToMaster()
    {
         MenuManager.OpenMenu(Start_Debug_Meun);
        if (start == 0)
        {
           

            start++;
        }
        if (keepSetting.start == 1)
        {
            userNameInput.text = "Player" + Random.Range(0, 99).ToString("00");
        }
        keepSetting.start++;
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true; //make sure the scene is updated
    }

    public override void OnJoinedLobby()
    {
        infoText.text = "connected to server: " + PhotonNetwork.CloudRegion;

        PhotonNetwork.NickName = userNameInput.text;

        cachedRoomList.Clear();

        Debug.Log("Joined!");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ErrorText.text = "Tried to join Arena, but failed!\n" + message;
        MenuManager.OpenMenu(ErrorMenu);
        Debug.Log("Tried to join Arena, but failed!");
    }
    //================================================================
    public void createRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            Debug.Log("IsNullOrEmpty!");
            return;
        }
        MenuManager.OpenMenu(LoadingMenu);
        if (roomManager.isTrainingGround)
        {
            roomNameInput.text = "TrainingGround";
            PhotonNetwork.CreateRoom(roomNameInput.text);
        }//cast room name
        else
        {
            keepSetting.roomName = roomNameInput.text;
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)maxRoomPlayers; // for example
            PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
        }

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if (message == "A game with the specified id already exist." && roomManager.isTrainingGround)
        {
            MenuManager.OpenMenu(LoadingMenu);
            PhotonNetwork.JoinRoom("TrainingGround"); //if joined successfully, OnJoinedRoom() will be called
            return;
        }
        ErrorText.text = "Tried to join Arena, but failed!\n" + message;
        MenuManager.OpenMenu(ErrorMenu);
        Debug.Log("Tried to join Arena, but failed!" + message);
    }

    //================================================================
    public void JoinRoom(RoomInfo roomInfo)
    {
        MenuManager.OpenMenu(LoadingMenu);
        PhotonNetwork.JoinRoom(roomInfo.Name); //if joined successfully, OnJoinedRoom() will be called

    }

    public override void OnJoinedRoom()
    {
        CheckName(PhotonNetwork.PlayerList);
        MenuManager.OpenMenu(RoomMenu);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        //refresh the playerListContent
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {

            //(Object original, Transform parent); clone a Object to target parent
            Instantiate(PlayerListItemPrefab, playerListContent)
                .GetComponent<PlayerListItem>() //get PlayerListItem.cs
                .SetUp(player); //call .SetUp to get player's name to show
        }
        IsMasterInfo();

        Debug.Log("Joined Arena!");
    }
    void IsMasterInfo()
    {
        if (roomManager.isTrainingGround)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;//player cannnot join from "find Room"
            startGameNotice.text = "All players can join to 'TrainingGround', No winner in this mode";

        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                startGameButton.SetActive(true); //only host clients can start game
                startGameNotice.text = "You are the Host, you can Start the Game!\n \nNotice that once the game start, no players can join or rejoin this room";
            }
            else
            {
                startGameButton.SetActive(false); startGameNotice.text = "waiting the Host Start the Game...\n \nNotice that once the game start, no players can join or rejoin this room";
            }
        }

    }
    void CheckName(Player[] players)
    {
        if (players.Length > maxRoomPlayers)// maximum number of players
        {
            Debug.Log("Tried to join Arena, but failed because full room!");
            MenuManager.checkInfoOK = false;

            MenuManager.PlayerSelection.text = "Tried to join room '" + PhotonNetwork.CurrentRoom.Name + "' but failed!\n"
              + "because the room reached maxium " + maxRoomPlayers + " players!";
            LeaveRoom();
        }
        List<Player> checkList = new List<Player>();
        foreach (Player player in players)
        {
            checkList.Add(player);
        }
        int index = 0;
        foreach (Player player in checkList)
        {
            if (PhotonNetwork.NickName == player.NickName) { index++; }

            if (index >= 2)
            {
                // ErrorText.text = "Tried to join Arena, but failed!\n" + "because your Name: " + PhotonNetwork.NickName + "is same as another player.";
                // MenuManager.OpenMenu(ErrorMenu);
                Debug.Log("Tried to join Arena, but failed because dupicate name!");
                MenuManager.checkInfoOK = false;
                MenuManager.PlayerSelection.text = "Tried to join room '" + PhotonNetwork.CurrentRoom.Name + "' but failed!\n"
                 + "because your Name: '" + PhotonNetwork.NickName + "' is same as another player."; ;
                LeaveRoom();
                index = 0;
                return;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //when other players join this room
    {
        Instantiate(PlayerListItemPrefab, playerListContent)
            .GetComponent<PlayerListItem>()
            .SetUp(newPlayer);
    }
    // public List<RoomInfo> DebugroomList = new List<RoomInfo>();
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // //refresh the roomListContent
        foreach (Transform trans in roomListContent)
        {
            Debug.Log("Deleted:" + trans.GetComponent<RoomListItem>().info.ToString());
            Destroy(trans.gameObject);
        }

        UpdateCachedRoomList(roomList);
        foreach (KeyValuePair<string, RoomInfo> item in cachedRoomList)
        {
            Debug.Log("value: " + item.Value.ToString());
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(item.Value);
        }
        Debug.Log("======");

    }
    //   https://doc.photonengine.com/en-us/pun/current/lobby-and-matchmaking/matchmaking-and-lobby
    public Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }
    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
    }
    // public Toggle m_Toggle;

    public void StartGame()
    {
        if (roomManager.isTrainingGround)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer); Debug.LogWarning("switched Host to current");
        }
        else { PhotonNetwork.CurrentRoom.IsVisible = false; }

        pV.RPC("LeavingRoom", RpcTarget.All);
        MenuManager.OpenMenu(LoadingMenu);
        Invoke("LodingGameScene", 1f);
    }
    [PunRPC]
    void LeavingRoom()
    {
        MenuManager.OpenMenu(LoadingMenu);
        soundController.isLeaveRoom = true;
        //isLeaveRoom = true;
    }
    void LodingGameScene()
    {
        PhotonNetwork.LoadLevel(1); //Level 0 is the start menu, Level 1 is the Gaming Scene
                                    //if (m_Toggle.isOn) PhotonNetwork.CurrentRoom.IsVisible = true; else PhotonNetwork.CurrentRoom.IsVisible = false;

        // if (roomManager.isTrainingGround) { PhotonNetwork.CurrentRoom.IsVisible = true; } else { PhotonNetwork.CurrentRoom.IsVisible = false; }
        //  PhotonNetwork.JoinRoom(arena);
    }

    //================================================================
    public void LeaveRoom()
    {

        PhotonNetwork.LeaveRoom();
        MenuManager.OpenMenu(LoadingMenu);
        Debug.Log("Leaved Room");
    }

    public override void OnLeftRoom() // when current player leaves current room successfully
    {
        MenuManager.OpenMenu(Start_Debug_Meun);
    }

    public override void OnMasterClientSwitched(Player newMasterClient) //after master client leaved, let new master client handle the start game right
    {
        IsMasterInfo();
    }
    //================================================================
    public void ForceQuit()
    {
        Destroy(GameObject.Find("RoomManager").gameObject);
        PhotonNetwork.LoadLevel(0);
        PhotonNetwork.LeaveRoom();

        // PhotonNetwork.Disconnect();

        Debug.Log("ForceQuit");
    }
    public void JoinTrainingGround()
    {

        CreateRoomNotice.text = "Tip: All players will be added and join to 'TrainingGround' room Automatically. No winner in this mode";
        roomNameInput.text = "TrainingGround";
        // PhotonNetwork.CreateRoom("TrainingGround");
        // CheckName();
        roomManager.isTrainingGround = true;
        // PhotonNetwork.LoadLevel(1);
    }
    public void JoinNormalRoom()
    {
        roomManager.isTrainingGround = false;
        roomNameInput.text = keepSetting.roomName;
        CreateRoomNotice.text = "Tip: Room Name must not NULL and dupicate";
    }
    public void OpenURL() { Application.OpenURL(moreInfoURL); }

}

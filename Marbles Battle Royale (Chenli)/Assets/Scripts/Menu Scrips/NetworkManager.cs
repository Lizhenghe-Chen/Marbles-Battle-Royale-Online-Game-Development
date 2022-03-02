using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    PhotonView pV;
    [Tooltip("when In debug state, mark this so that no need to type room name to create room over and over again ")][SerializeField] bool IsInDebugMode = false;
    public static NetworkManager Instance;

    public GameObject Start_Debug_Meun;

    public TextMeshProUGUI infoText;
    public string moreInfoURL;
    MenuManager MenuManager;

    public bool isLeaveRoom = false;

    //================================================================
    [SerializeField] TMP_InputField roomNameInput;
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
    [SerializeField] TMP_Text startGameNotice;

    //========== when add a new menu page, enter new name manually and find their object================
    //[Header("when add a new menu page, enter new name manually and find their object\n")]
    public GameObject

            TitleMenu, LoadingMenu, CreateRoomMenu, RoomMenu, ErrorMenu, FindRoomMenu;

    // [SerializeField]
    // GameObject[]
    //     menus = { TitleMenu, LoadingMenu, RoomMenu, ErrorMenu, FindRoomMenu };
    void Awake()
    {
        pV = GetComponent<PhotonView>();
        isLeaveRoom = false;
        Instance = this;
        MenuManager = this.GetComponent<MenuManager>();
        GameObject[] _menus = MenuManager.menuList;

        // SetActive to make sure Find GameObject could work
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
        if (IsInDebugMode) { roomNameInput.text = "Test Room"; }
        MenuManager.OpenMenu(LoadingMenu);
    }
    void Update()
    {
        if (isLeaveRoom) { MenuManager.OpenMenu(LoadingMenu); }
        //  Debug.Log(PhotonNetwork.LevelLoadingProgress);
        //Debug.Log(PhotonNetwork.NetworkingClient);

    }
    int start = 0;

    void Start()
    {

        // Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
        userNameInput.text = "Player" + Random.Range(0, 99).ToString("00");
    }

    public void OnUserNameChanged()
    {
        PhotonNetwork.NickName = userNameInput.text;
        if (userNameInput.text == null)
        {
            Debug.Log("null Player!!");
            userNameInput.text = "Player" + Random.Range(0, 99).ToString("00");
        }
    }

    //================================================================
    public override void OnConnectedToMaster()
    {
        if (start == 0)
        {
            MenuManager.OpenMenu(Start_Debug_Meun);
            start++;
        }

        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true; //make sure the scene is updated
    }

    public override void OnJoinedLobby()
    {
        infoText.text = "connected to server: " + PhotonNetwork.CloudRegion;

        PhotonNetwork.NickName = userNameInput.text;

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
        PhotonNetwork.CreateRoom(roomNameInput.text);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ErrorText.text = "Tried to join Arena, but failed!\n" + message;
        MenuManager.OpenMenu(ErrorMenu);
        Debug.Log("Tried to join Arena, but failed!");
    }

    //================================================================
    public void JoinRoom(RoomInfo roomInfo)
    {
        MenuManager.OpenMenu(LoadingMenu);
        PhotonNetwork.JoinRoom(roomInfo.Name); //if joined successfully, OnJoinedRoom() will be called

    }

    public override void OnJoinedRoom()
    {
        Player[] players = PhotonNetwork.PlayerList;


        MenuManager.OpenMenu(RoomMenu);
        roomNameText.text = "Room " + PhotonNetwork.CurrentRoom.Name;
        CheckName(players);
        //refresh the playerListContent
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (Player player in players)
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
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true); //only host clients can start game
            startGameNotice.text = "You are the Master, you can Start the Game!";
        }
        else
        {
            startGameButton.SetActive(false); startGameNotice.text = "waiting the Master Start the Game...";
        }
    }
    void CheckName(Player[] players)
    {

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
                GetComponent<MenuManager>().isNameDublicated = true;
                GetComponent<MenuManager>().PlayerSelection.text = "Tried to join room '" + PhotonNetwork.CurrentRoom.Name + "' but failed!\n"
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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //refresh the roomListContent
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        //
        foreach (RoomInfo roomInfo in roomList)
        {
            //if the room has been removed, then skip this for loop
            if (roomInfo.RemovedFromList) continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomInfo);
        }
    }
    public Toggle m_Toggle;

    public void StartGame()
    {
        pV.RPC("SendInRoomInfo", RpcTarget.All);
        MenuManager.OpenMenu(LoadingMenu);
        Invoke("LodingGameScene", 1f);
    }
    [PunRPC]
    void SendInRoomInfo() { isLeaveRoom = true; }
    void LodingGameScene()
    {
        PhotonNetwork.LoadLevel(1); //Level 0 is the start menu, Level 1 is the Gaming Scene

        //if (m_Toggle.isOn) PhotonNetwork.CurrentRoom.IsVisible = true; else PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        //  PhotonNetwork.JoinRoom(arena);
    }

    //================================================================
    public void LeaveRoom()
    {
        MenuManager.OpenMenu(LoadingMenu);
        PhotonNetwork.LeaveRoom();
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
    public void OpenURL() { Application.OpenURL(moreInfoURL); }
}

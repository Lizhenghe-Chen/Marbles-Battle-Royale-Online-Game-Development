using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Realtime;//will use Player Type
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/*
* Copyright (C) 2022 Author: Lizhenghe.Chen.
* For personal study or educational use.
* Email: Lizhenghe.Chen@qq.com
*/
public class PlayerManager : MonoBehaviourPunCallbacks
{
    // public static PlayerManager instance;

    string PlayerSelection;

    public GameObject controller;//player ball objets
    PhotonView playerPhotonView;
    [Header("Below are each players's start points, must in order")]
    [SerializeField] int startPointsSize = 3;
    public List<Transform> startPoints;//must in order!

    public Vector3 deadPosition;

    [SerializeField] RoomManager RoomManager;
    [SerializeField] KeepSetting keepSetting;
    [SerializeField] GameInfoManager GameInfoManager;
    [SerializeField] string player_Name;
    [SerializeField] Image FadeIn_OutImage;
    //[Header("Below are each players data, should be sycn to all players in a room \n")]

    [Tooltip("Player's kill and death data, will be sync to all players in a room.")]
    public int killCount = 0, deathCount = 0;
    public float playerHealth = 100f; //this cannot be changed
    public float damageTimer = 1f;// this could change hit damage
    public static int avoidloop = 0;

    //================================================================
    [Header("Below parameter will be referenced by other scripts\n")]
    public float currentHealth;
    public bool isDead = false;
    public float billboardvalue;
    public int maxLife = 5;
    public string leftLifeTextContent;
    public double deathAltitude = -40f;
    public Vector3 initialScale;

    //================================================================
    //[Tooltip("How frequently (second) send player's information to server")]

    [SerializeField] GameObject[] playerList;
    [SerializeField] Canvas GameOverCanvas;
    [SerializeField] TMP_Text GameOverText;
    [SerializeField] string currentWinnerName;
    public float gameOverBackMenuTimer = 5;
    //================================================================
    [SerializeField] GuidanceText guidanceText;
  


    void Awake()
    {
        playerPhotonView = GetComponent<PhotonView>();
        RoomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        keepSetting = GameObject.Find("KeepSetting").GetComponent<KeepSetting>();
        player_Name = playerPhotonView.Owner.NickName;
    }

    void Start()
    {
        GameOverCanvas = GameObject.Find("GameOverCanvas").GetComponent<Canvas>();
        GameOverText = GameOverCanvas.transform.Find("GameOverText").GetComponent<TMP_Text>();
        GameOverCanvas.enabled = false;
        GameInfoManager = GameObject.Find("GameInfoCanvas/GameInfo").GetComponent<GameInfoManager>();


        if (!playerPhotonView.IsMine) //is the photon View is hadle on the local player?
        { return; }
        //this is very important to put these behand the isMine Judgement to make sure the sync not mess up
        currentHealth = playerHealth;
        //make sure all players have this playerManager data
        leftLifeTextContent = "Rest Life: " + (maxLife - deathCount-1);

        PlayerSelection = keepSetting.playerType;
        // Debug.Log(startPoints.Length);
        for (int i = 0; i <= startPointsSize; i++)
        {
            string path = "StartPoints/startPoint" + i;
            startPoints.Add(GameObject.Find(path).transform);
        }
        deadPosition = startPoints[0].position;

        CreateController();
        FadeIn_OutImage = controller.transform.Find("UI/Canvas/Image").GetComponent<Image>();
        //Let toturial guidance know which is local player
        if (RoomManager.isTrainingGround && keepSetting.showTutorial)
        {
            guidanceText = GameObject.Find("GameInfoCanvas/Tutorial/GuidanceText").GetComponent<GuidanceText>();
            guidanceText.LocalPlayer = controller;
            guidanceText.LocalPlayerRg = controller.GetComponent<Rigidbody>();
            guidanceText.localPlayerManager = this;
            // guidanceText.startPoint = startPoints[0].transform.position;
        }
        // billboardvalue = currentHealth / playerHealth;
        playerPhotonView.RPC("SentData", RpcTarget.All, currentHealth, killCount, deathCount, isDead);
    }

    void Update()
    {

        if (!playerPhotonView.IsMine) { return; }
        // billboardvalue = currentHealth / playerHealth;
        //playerPhotonView.RPC("SendHealthData", RpcTarget.All, currentHealth);
        // if (currentHealth <= 0)
        // {
        //     Die();
        // }

        // if (currentHealth <= 5) { Damage(0, false); }// avoid 0 damage bug


        // playerPhotonView.RPC("SentData", RpcTarget.All, currentHealth, billboardvalue);//death and kill count no need to use here to decrease some server pressure

        // time += Time.deltaTime;
        // if (time >= frequency)
        // {
        //     // Debug.Log("Sent Message");
        //     playerPhotonView.RPC("SentData", RpcTarget.All, currentHealth, billboardvalue, killCount, deathCount, isDead);
        //     time = 0;
        // }
    }
    void FixedUpdate()
    {
        if (RoomManager.isTrainingGround) { return; }
        Invoke("CheckPlayerList", 10f);
    }

    Vector3 RebirthPosition(Vector3 deadPosition)
    {
        for (int i = startPoints.Count - 1; i >= 0; i--)
        {
            if (deadPosition.x >= startPoints[i].position.x)
            {
                return startPoints[i].position;
            }
        }
        return startPoints[0].position;
    }
    void CreateController()
    {
        var position = Vector3.zero;
        // Instantiate player controller

        if (RoomManager.isTrainingGround) position = startPoints[0].position;//let player brith in same place when start in traning ground
        else
            position = new Vector3(startPoints[0].position.x + Random.Range(-10.0f, 10.0f), 20f, startPoints[0].position.z + Random.Range(-100.0f, 100.0f));

        // else
        // {
        //     var temp = RebirthPosition(deadPosition);
        //     if (temp == startPoints[0].position) //if player born in start position
        //     {
        //         position = new Vector3(temp.x + Random.Range(-10.0f, 10.0f), 20f, temp.z + Random.Range(-100.0f, 100.0f));
        //     }
        //     else
        //         position = new Vector3(temp.x + Random.Range(-10.0f, 10.0f), 20f, temp.z + Random.Range(-10.0f, 10f));
        // }

        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", PlayerSelection), position, Quaternion.identity, 0,
        new object[] { playerPhotonView.ViewID });//bring the current plaerManagerID to the game, it could let controllers & ClollisionDetect read and find their playerManager later
        //  Debug.Log("Instantiated player controller");
        //  Debug.Log(playerPhotonView.ViewID);
    }
    void CreateSpectator()
    {
        var position = new Vector3(0, 10f, 0);
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "spectator"), position, Quaternion.identity, 0,
        new object[] { playerPhotonView.ViewID });//bring the current plaerManagerID to the game, it could let controllers & ClollisionDetect read and find their playerManager later
        Debug.Log("Instantiated player Spectator");
        //  Debug.Log(playerPhotonView.ViewID);
    }

    public void TakeDamage(float finalDamage, string message, string other_Player_Name)
    {
        Damage(finalDamage, false, other_Player_Name);
        //playerPhotonView.RPC("Damage", RpcTarget.All, finalDamage);
        Debug.Log("**************** " + message + " ****************");
    }
    public void Damage(float finalDamage, bool isDamagedByDamageZone, string other_Player_Name)
    {
        currentHealth -= finalDamage;
        //playerPhotonView.RPC("DecreaseHealth", RpcTarget.All, finalDamage);
        playerPhotonView.RPC("SendHealthData", RpcTarget.All, currentHealth);

        if (currentHealth <= 0)
        {// to avoid player dead in zone and killd by other message show in same time:
            if (isDamagedByDamageZone && avoidloop == 0)
            {
                GameInfoManager.Refresh(player_Name + " X-> Damage zone...");
                Die();
                // Invoke("Die", 0.5f);
                return;
            }
            else
            {
                deadPosition = controller.transform.position;
                //  Kill(other_Player_Name);
                Die();
            }
        }
    }
    public void Die()
    {
        // FadeIn_OutImage.GetComponent<AnimateLoading>().LeavingLevel();

        currentHealth = playerHealth;// refresh the health;
        playerPhotonView.RPC("SentData", RpcTarget.All, currentHealth, killCount, deathCount, isDead);//
        avoidloop = 0;
        Debug.Log(this.playerPhotonView.Controller.NickName + " Dead!");

        Death();
        Invoke("LateDeadOption", 0);
        //playerPhotonView.RPC("Death", RpcTarget.All);

    }
    void LateDeadOption()
    {
        FadeIn_OutImage.GetComponent<AnimateLoading>().LoadingLevel();

        leftLifeTextContent = "Rest Life: " + (maxLife - deathCount-1);
        if (deathCount >= maxLife)
        {
            if (RoomManager.isTrainingGround && keepSetting.showTutorial && guidanceText.Goal < 9)
            {//constraint player death until..
                deathCount = 0;
                return;
            }
            //isDead = true;
            PhotonNetwork.Destroy(controller);
            playerPhotonView.RPC("IsDead", RpcTarget.All);
            CreateSpectator();
            GameInfoManager.Refresh(player_Name + " is eliminated ");
        }
        else
        {
            var temp = RebirthPosition(deadPosition);
            temp = new Vector3(temp.x + Random.Range(-10.0f, 10.0f), 20f, temp.z + Random.Range(-10.0f, 10f));
            controller.GetComponent<Rigidbody>().velocity = Vector3.zero;
            controller.transform.localScale = initialScale;
            controller.transform.position = temp;
        }
       

    }

    public void Kill(string victimName)
    {
        Debug.Log(" !!!!!!!!!!!!!!!!!!Killed : " + victimName + " !!!!!!!!!!!!!!!!!!");
        avoidloop++;
        //OnKill();
        playerPhotonView.RPC("OnKill", RpcTarget.All);
        GameInfoManager.Refresh(player_Name + " X -> " + victimName);
    }


    public void Health(float addHealth)
    { if (currentHealth > playerHealth) {Debug.LogWarning("?!?!?!?!?!") ;return; }//avoid health exceeed max health
        currentHealth += addHealth;
        //playerPhotonView.RPC("AddHealth", RpcTarget.All, addHealth);
        playerPhotonView.RPC("SendHealthData", RpcTarget.All, currentHealth);
    }
    void Death()
    {

        // deathCount++;
        playerPhotonView.RPC("OnDeath", RpcTarget.All);
    }

    public void isMyPhotonView(PhotonView playerPhotonView)
    {
        if (!playerPhotonView.IsMine && playerPhotonView)
        {
            return;
        }
    }
    void CheckPlayerList()
    {
        playerList = GameObject.FindGameObjectsWithTag("Player");
        if (playerList.Length <= 0)
        {
            GameOverCanvas.enabled = true;
            Countdown();
            // Debug.LogWarning("No Player in the room currently.");
        }
        else if (playerList.Length == 1)
        {
            currentWinnerName = playerList[0].GetComponent<PhotonView>().Owner.NickName;
            GameOverCanvas.enabled = true;
            Countdown();
        }
        else GameOverCanvas.enabled = false;


    }
    void Countdown()
    {
        if (!playerPhotonView.IsMine) { return; }
        if (gameOverBackMenuTimer > 0)
        {
            gameOverBackMenuTimer -= Time.fixedDeltaTime;
            string GameOverInfo;
            if (currentWinnerName == playerPhotonView.Owner.NickName)
            {
                //Debug.LogWarning("SameName");
                GameOverInfo = "Game Over!!\n" + "You Are The Winner!\n Back to Menu in " + (int)gameOverBackMenuTimer;
            }
            else { GameOverInfo = "Game Over!!\n" + "The Winner is: \"" + currentWinnerName + "\"\n Back to Menu in " + (int)gameOverBackMenuTimer; }

            // Debug.LogWarning(GameOverInfo);
            GameOverText.text = GameOverInfo;

        }
        else
        { //time reached
            LeaveRoom();
        }
    }
    public void LeaveRoom()
    {
        Destroy(GameObject.Find("RoomManager").gameObject);
        Destroy(controller);
        // PhotonNetwork.LeaveRoom();
        // GameInfoManager.Refresh(player_Name + " Left the game...");
        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.Disconnect();
        //if (PhotonNetwork.CurrentRoom != null) { PhotonNetwork.Disconnect(); }
        SceneManager.LoadScene(0); //Level 0 is the start menu, Level 1 is the Gaming Scene
                                   // PhotonNetwork.LoadLevel(0);
        Debug.Log("Leaved Room");
    }
    public override void OnPlayerLeftRoom(Player newPlayer) // when current player leaves current room successfully
    {
        if (!photonView.IsMine) { return; }
        GameInfoManager.GlobalRefresh(newPlayer.NickName + " Left the game...");
       
    }
    // public override void OnJoinedRoom()
    // {
    //     GameInfoManager.Refresh(player_Name + " Joined the game...");
    // }
    public override void OnPlayerEnteredRoom(Player newPlayer) //when other players join this room
    {
        if (!photonView.IsMine) { return; }

        playerPhotonView.RPC("SentData", RpcTarget.All, currentHealth, killCount, deathCount, isDead);//let new player get exist players' information


        GameInfoManager.GlobalRefresh(newPlayer.NickName + " Joined the game...");
        // Debug.Log(newPlayer.NickName);
    }
    //Set RPC Methods================================================================
    /*DecreaseHealth; AddHealth may not use since SendHealthData done*/
    [PunRPC]
    void DecreaseHealth(float finalDamage)
    {
        currentHealth -= finalDamage;
    }
    [PunRPC]
    void AddHealth(float addHealth)
    {
        if (currentHealth >= playerHealth) {Debug.LogWarning("?!?!?!?!?!") ;return; }//avoid health exceeed max health
        currentHealth += addHealth;
    }

    [PunRPC]
    void OnDeath()
    {

        deathCount++;

    }
    [PunRPC]
    void OnKill()
    {

        killCount++;

    }
    [PunRPC]
    void IsDead()
    {
        isDead = true;
    }
    [PunRPC]
    void SentData(float _currentHealth, int _killCount, int _deathCount, bool _isDead)
    {
        currentHealth = _currentHealth;
        // billboardvalue = _billboardvalue;
        isDead = _isDead;
        deathCount = _deathCount;
        killCount = _killCount;

    }
    [PunRPC]
    void SendHealthData(float _currentHealth)
    {
        currentHealth = _currentHealth;
    }
}

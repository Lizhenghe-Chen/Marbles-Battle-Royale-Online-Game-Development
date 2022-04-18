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
    public Vector3 deadPosition;
    [SerializeField] RoomManager RoomManager;
    [SerializeField] KeepSetting keepSetting;
    [SerializeField] GameInfoManager GameInfoManager;
    [SerializeField] CollisionDetect collisionDetect;
    [SerializeField] JumpController jumpController;
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
    public GameObject takeDamageMask, getHealthMask;
    [SerializeField] private Vector3 damageareaPosition, playerPosition;
    [SerializeField] private GameObject damagearea;
    [SerializeField] private float damagearea_playerDistance;

    [Header("Below are each players's start points, must in order")]
    [SerializeField] private List<Transform> startPoints;//must in order!

    //================================================================
    //[Tooltip("How frequently (second) send player's information to server")]

    private GameObject[] playerList;
    [SerializeField] private TMP_Text GameOverText;

    public float gameOverBackMenuTimer;
    //================================================================
    [SerializeField] private GuidanceText guidanceText;
    public ScoreBoardManager scoreBoardManager;


    private void Awake()
    {
        playerPhotonView = GetComponent<PhotonView>();
        RoomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        keepSetting = GameObject.Find("KeepSetting").GetComponent<KeepSetting>();
        player_Name = playerPhotonView.Owner.NickName;
    }

    private void Start()
    {
        GameInfoManager = GameObject.Find("GameInfoCanvas/GameInfo").GetComponent<GameInfoManager>();

        if (!playerPhotonView.IsMine) //is the photon View is hadle on the local player?
        { return; }
        GameInfoManager.GameOverCanvas = GameObject.Find("GameOverCanvas").GetComponent<Canvas>();
        GameOverText = GameInfoManager.GameOverCanvas.transform.Find("GameOverText").GetComponent<TMP_Text>();
        GameInfoManager.GameOverCanvas.enabled = false;

        damagearea = GameObject.Find("DamageArea");

        //this is very important to put these behand the isMine Judgement to make sure the sync not mess up
        currentHealth = playerHealth;
        //make sure all players have this playerManager data
        leftLifeTextContent = "Rest Life: " + (maxLife - deathCount - 1);

        PlayerSelection = keepSetting.playerType;
        // Debug.Log(startPoints.Length);
        foreach (Transform startPoint in GameObject.Find("StartPoints").transform)
        {
            startPoints.Add(startPoint);
        }

        deadPosition = startPoints[0].position;

        CreateController();
        collisionDetect = controller.GetComponent<CollisionDetect>();
        jumpController = controller.GetComponent<JumpController>();
        scoreBoardManager = controller.transform.Find("UI/ScoreBoard").GetComponent<ScoreBoardManager>();
        GameInfoManager.scoreBoardManager = scoreBoardManager;
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

        playerPhotonView.RPC("SentData", RpcTarget.All, currentHealth, killCount, deathCount, isDead);


        GameInfoManager.GlobalRefresh(player_Name + " Joined the game!!!");
        GameInfoManager.Invoke("RefreshScoreBoard", 1f);
    }

    // void Update()
    // {
    //     if (!playerPhotonView.IsMine) { return; }
    // }
    private void FixedUpdate()
    {
        billboardvalue = currentHealth / playerHealth;
        if (!playerPhotonView.IsMine) //is the photon View is hadle on the local player?
        { return; }
        Invoke("InvokeCheckPlayer", 5f);
        if (controller.tag == "Player")
        {
            HealthEffect();
            PoisoningEffect();// low FPS will affect damgae and health speed, so move it to FixedUpdate
            BelowDeathAltitude();

        }
    }

    private Vector3 RebirthPosition(Vector3 deadPosition)
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
    private void CreateController()// Instantiate player controller
    {
        var position = Vector3.zero;
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
        takeDamageMask = controller.transform.Find("UI/Canvas/TakeDamageMask").gameObject;
        getHealthMask = controller.transform.Find("UI/Canvas/GetHealthMask").gameObject;
        takeDamageMask.SetActive(false);
        getHealthMask.SetActive(false);
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
                Die(false);
                GameInfoManager.GlobalRefresh(player_Name + " X-> Damage zone...");
                // Invoke("Die", 0.5f);
                return;
            }
            else//else maybe killed by other 
            {
                deadPosition = controller.transform.position;
                //  Kill(other_Player_Name);
                Die(false);
                GameInfoManager.GlobalRefresh(other_Player_Name + " X -> " + player_Name);
            }
        }
    }
    public void Die(bool isfallDead)
    {
        // FadeIn_OutImage.GetComponent<AnimateLoading>().LeavingLevel();
        currentHealth = playerHealth;// refresh the health;
        jumpController.JumpTime = jumpController.JumpcoolingTime;
        jumpController.Rushtime = jumpController.RushcoolingTime;
        playerPhotonView.RPC("SentData", RpcTarget.All, currentHealth, killCount, deathCount, isDead);//
        avoidloop = 0;
        Debug.Log(this.playerPhotonView.Controller.NickName + " Dead!");

        playerPhotonView.RPC("OnDeath", RpcTarget.All);
        Invoke("LateDeadOption", 0);
        if (isfallDead) { GameInfoManager.GlobalRefresh(player_Name + " Fall dead"); }
        //playerPhotonView.RPC("Death", RpcTarget.All);

    }
    void LateDeadOption()
    {
        FadeIn_OutImage.GetComponent<AnimateLoading>().LoadingLevel();

        leftLifeTextContent = "Rest Life: " + (maxLife - deathCount - 1);
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
            GameInfoManager.GlobalRefresh(player_Name + " is eliminated ");
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
        playerPhotonView.RPC("OnKill", RpcTarget.All, victimName);
    }


    public void Health(float addHealth)
    {
        if (currentHealth > playerHealth)//avoid health exceeed max health
        {
            return;
        }
        currentHealth += addHealth;
        //playerPhotonView.RPC("AddHealth", RpcTarget.All, addHealth);
        playerPhotonView.RPC("SendHealthData", RpcTarget.All, currentHealth);
    }


    public void isMyPhotonView(PhotonView playerPhotonView)
    {
        if (!playerPhotonView.IsMine && playerPhotonView)
        {
            return;
        }
    }
    // private void CheckPlayerList()
    // {
    //     playerList = GameObject.FindGameObjectsWithTag("Player");
    //     if (playerList.Length <= 0)
    //     {
    //         Countdown();
    //         // Debug.LogWarning("No Player in the room currently.");
    //     }
    //     else if (playerList.Length == 1)
    //     {
    //         currentWinnerName = playerList[0].GetComponent<PhotonView>().Owner.NickName;
    //         GameOverCanvas.enabled = true;
    //         Countdown();
    //     }
    //     else GameOverCanvas.enabled = false;


    // }
    private void InvokeCheckPlayer()
    {
        if (!RoomManager.isTrainingGround && GameInfoManager.gameOver) { Countdown(GameInfoManager.currentWinnerName); }

    }
    private void Countdown(string currentWinnerName)
    {
        GameInfoManager.GameOverCanvas.enabled = true;
        if (gameOverBackMenuTimer > 0)
        {
            gameOverBackMenuTimer -= Time.fixedDeltaTime;
            string GameOverInfo;
            if (string.Equals(currentWinnerName, playerPhotonView.Owner.NickName))
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
        PhotonNetwork.Destroy(controller);
        PhotonNetwork.Destroy(this.gameObject);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0); //Level 0 is the start menu, Level 1 is the Gaming Scene
        Debug.Log("Leaved Room");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer) // when other player leaves current room successfully.this happen locacally
    {
        if (!photonView.IsMine) { return; }
        GameInfoManager.Refresh(otherPlayer.NickName + " Left the game...");
        GameInfoManager.Invoke("RefreshScoreBoard", 1f);
        //scoreBoardManager.Refresh();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //when other players join this room, this happen locacally
    {
        // scoreBoardManager.Refresh();
        if (!photonView.IsMine) { return; }
        playerPhotonView.RPC("SentData", RpcTarget.All, currentHealth, killCount, deathCount, isDead);//let new player get exist players' information
        GameInfoManager.Refresh(newPlayer.NickName + " Try to Join the game...");
        // GameInfoManager.Refresh(newPlayer.NickName + " Joined the game...");

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
        if (currentHealth >= playerHealth) { Debug.LogWarning("?!?!?!?!?!"); return; }//avoid health exceeed max health
        currentHealth += addHealth;
    }

    [PunRPC]
    void OnDeath()
    {
        deathCount++;
    }
    [PunRPC]
    void OnKill(string victimName)
    {
        killCount++;
        GameInfoManager.GlobalRefresh(player_Name + " X -> " + victimName);
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
    //=======================================================================================

    void PoisoningEffect()
    {
        damageareaPosition = damagearea.transform.position;
        damagearea_playerDistance = controller.transform.position.x - damageareaPosition.x;
        if (damagearea_playerDistance < 0)
        {
            takeDamageMask.SetActive(true);
            Damage(PoisoningEffectMultiplier(), true, string.Empty);
            // Debug.Log(PoisoningEffectMultiplier());
        }
        else { takeDamageMask.SetActive(false); }
    }
    public float PoisoningEffectMultiplier()
    {
        for (int i = startPoints.Count - 1; i >= 0; i--)
        {
            if (damageareaPosition.x >= startPoints[i].position.x)
            {
                return 0.06f * (i + 1);
            }
        }
        return 0.06f;

    }
    public void HealthEffect()
    {
        if (collisionDetect.inHealthArea)
        {
            getHealthMask.SetActive(true);
            Health(0.1f);
        }
        else
        {
            getHealthMask.SetActive(false);
        }
    }
    public void BelowDeathAltitude()
    {
        var playerPosition = controller.transform.position;
        if (playerPosition.y < deathAltitude)
        {
            deadPosition = playerPosition;//send death position to it's player Manager                                             
            Die(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
/*
* Copyright (C) 2022 Author: Lizhenghe.Chen.
* For personal study or educational use.
* Email: Lizhenghe.Chen@qq.com
*/
public class PlayerManager : MonoBehaviour
{
    // public static PlayerManager instance;

    string PlayerSelection;

    GameObject controller;
    PhotonView photonView;
    [Header("Below are each players's start points, must in order")]
    [SerializeField] int startPointsSize = 3;
    [SerializeField] List<Transform> startPoints;//must in order!

    public Vector3 deadPosition;

    [SerializeField] RoomManager RoomManager;
    //[Header("Below are each players data, should be sycn to all players in a room \n")]

    [Tooltip("Player's kill and death data, will be sync to all players in a room.")]
    public int killCount = 0, deathCount = 0;
    const float playerHealth = 100f; //this cannot be changed
    public float damageTimer = 1f;// this could change hit damage

    //================================================================
    [Header("Below parameter will be referenced by other scripts\n")]
    public float currentHealth;
    public bool isDead = false;
    public float billboardvalue;
    public int maxLife = 5;
    public string leftLifeTextContent;

    //================================================================
    //[Tooltip("How frequently (second) send player's information to server")]

    [SerializeField] GameObject[] playerList;
    [SerializeField] Canvas GameOverCanvas;
    [SerializeField] TMP_Text GameOverText;
    [SerializeField] string currentWinnerName;
    public float gameOverBackMenuTimer = 5;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        leftLifeTextContent = "Rest Life: " + (maxLife - deathCount);
        GameOverCanvas = GameObject.Find("GameOverCanvas").GetComponent<Canvas>();
        GameOverText = GameOverCanvas.transform.Find("GameOverText").GetComponent<TMP_Text>();
        GameOverCanvas.enabled = false;
        currentHealth = playerHealth;
        RoomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        if (photonView.IsMine) //is the photon View is hadle on the local player?
        {
            //make sure all players have this playerManager data
            
            PlayerSelection = RoomManager.playerType;
            // Debug.Log(startPoints.Length);
            for (int i = 0; i <= startPointsSize; i++)
            {
                string path = "StartPoints/startPoint" + i;
                startPoints.Add(GameObject.Find(path).transform);
            }
            deadPosition = startPoints[0].position;

            CreateController(true);

        }
    }

    void Update()
    {
        if (!photonView.IsMine) { return; }
        billboardvalue = currentHealth / playerHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (currentHealth >= playerHealth) { currentHealth = playerHealth; }

        photonView.RPC("SentData", RpcTarget.All, currentHealth, billboardvalue);//death and kill count no need to use here to decrease some server pressure

        // time += Time.deltaTime;
        // if (time >= frequency)
        // {
        //     // Debug.Log("Sent Message");
        //     photonView.RPC("SentData", RpcTarget.All, currentHealth, billboardvalue, killCount, deathCount, isDead);
        //     time = 0;
        // }
    }
    void FixedUpdate()
    {
        if (RoomManager.isTrainingGround) { return; }
        Invoke("CheckPlayerList", 2f);
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
    void CreateController(bool isStart)
    {
        var position = Vector3.zero;
        // Instantiate player controller
        if (isStart)
        {
            position = new Vector3(startPoints[0].position.x + Random.Range(-10.0f, 10.0f), 20f, startPoints[0].position.z + Random.Range(-100.0f, 100.0f));
        }
        else
        {
            var temp = RebirthPosition(deadPosition);
            position = new Vector3(temp.x + Random.Range(-10.0f, 10.0f), 20f, temp.z + Random.Range(-10.0f, 10f));
        }

        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", PlayerSelection), position, Quaternion.identity, 0,
        new object[] { photonView.ViewID });//bring the current plaerManagerID to the game, it could let controllers & ClollisionDetect read and find their playerManager later
        //  Debug.Log("Instantiated player controller");
        //  Debug.Log(photonView.ViewID);
    }
    void CreateSpectator()
    {

        var position = new Vector3(0, 10f, 0);
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "spectator"), position, Quaternion.identity, 0,
        new object[] { photonView.ViewID });//bring the current plaerManagerID to the game, it could let controllers & ClollisionDetect read and find their playerManager later
        Debug.Log("Instantiated player Spectator");
        //  Debug.Log(photonView.ViewID);
    }

    public void TakeDamage(float finalDamage, string message)
    {
        Damage(finalDamage);
        //photonView.RPC("Damage", RpcTarget.All, finalDamage);
        Debug.Log("**************** " + message + " ****************");
    }

    public void Die()
    {
        Debug.Log(this.photonView.Controller.NickName + " Dead!");

        Death();
        Invoke("LateDeadOption", 0);
        //photonView.RPC("Death", RpcTarget.All);

    }
    void LateDeadOption()
    {
        PhotonNetwork.Destroy(controller);
        currentHealth = playerHealth;// refresh the health;
        leftLifeTextContent = "Rest Life: " + (maxLife - deathCount);
        if (deathCount >= maxLife)
        {
            //isDead = true;
            photonView.RPC("IsDead", RpcTarget.All);
            CreateSpectator();
        }
        else CreateController(false);
    }
    public void Kill(string killerName)
    {
        Debug.Log(" !!!!!!!!!!!!!!!!!!Killed by: " + killerName + " !!!!!!!!!!!!!!!!!!");

        //OnKill();
        photonView.RPC("OnKill", RpcTarget.All);
    }

    public void Damage(float finalDamage)
    {
        // currentHealth -= finalDamage;
        photonView.RPC("DecreaseHealth", RpcTarget.All, finalDamage);
    }
    public void Health(float addHealth)
    {
        //  currentHealth += addHealth;
        photonView.RPC("AddHealth", RpcTarget.All, addHealth);
    }
    void Death()
    {
        // deathCount++;
        photonView.RPC("OnDeath", RpcTarget.All);
    }
    [PunRPC]
    void DecreaseHealth(float finalDamage)
    {
        currentHealth -= finalDamage;

    }
    [PunRPC]
    void AddHealth(float addHealth)
    {
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
    void SentData(float _currentHealth, float _billboardvalue)
    {
        currentHealth = _currentHealth;
        billboardvalue = _billboardvalue;
        //isDead = _isDead;
        // deathCount = _deathCount;
        // killCount = _killCount;

    }

    public void isMyPhotonView(PhotonView photonView)
    {
        if (!photonView.IsMine && photonView)
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
        if (!photonView.IsMine) { return; }
        if (gameOverBackMenuTimer > 0)
        {
            gameOverBackMenuTimer -= Time.fixedDeltaTime;
            string GameOverInfo;
            if (currentWinnerName == photonView.Owner.NickName)
            {
                Debug.LogWarning("SameName");
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

        // PhotonNetwork.LeaveRoom();

        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.Disconnect();
        //if (PhotonNetwork.CurrentRoom != null) { PhotonNetwork.Disconnect(); }
        SceneManager.LoadScene(0); //Level 0 is the start menu, Level 1 is the Gaming Scene
                                   // PhotonNetwork.LoadLevel(0);
        Debug.Log("Leaved Room");
    }
    // public void SetParticle(bool particleSystemJudge)
    // {
    //     PS = particleSystemJudge;
    // }

}

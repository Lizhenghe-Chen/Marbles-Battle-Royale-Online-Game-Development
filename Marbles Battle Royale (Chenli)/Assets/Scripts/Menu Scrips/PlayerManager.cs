using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerManager : MonoBehaviour
{
    // public static PlayerManager instance;

    string PlayerSelection;

    GameObject controller;
    PhotonView photonView;

    // RoomManager RoomManager;
    [Header("Below are each players data, should be sycn to all players in a room \n")]

    [Tooltip("Player's kill and death data, will be sync to all players in a room.")]
    public int killCount = 0, deathCount = 0;
    const float playerHealth = 100f; //this cannot be changed

    //================================================================
    [Header("Below parameter will be referenced by other scripts\n")]
    public float currentHealth;
    public bool isDead = false;
    public float billboardvalue;

    //================================================================
    [Tooltip("How frequently (second) send player's information to server")]
    [SerializeField] float frequency = 5f, time;
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        currentHealth = playerHealth;
        if (photonView.IsMine) //is the photon View is hadle on the local player?
        {
            //make sure all players have this playerManager data
            PlayerSelection = GameObject.Find("RoomManager").GetComponent<RoomManager>().playerType;
            CreateController();
        }
    }

    void Update()
    {
         if (!photonView.IsMine) { return; }
        billboardvalue = currentHealth / playerHealth;
        photonView.RPC("SentData", RpcTarget.All, currentHealth, billboardvalue, killCount, deathCount, isDead);
        // time += Time.deltaTime;
        // if (time >= frequency)
        // {
        //     // Debug.Log("Sent Message");
        //     photonView.RPC("SentData", RpcTarget.All, currentHealth, billboardvalue, killCount, deathCount, isDead);
        //     time = 0;
        // }
    }


    void CreateController()
    {

        // Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        // Instantiate player controller
        var position = new Vector3(Random.Range(-10.0f, 10.0f), 20f, Random.Range(-10.0f, 10.0f));
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", PlayerSelection), position, Quaternion.identity, 0,
        new object[] { photonView.ViewID });//bring the current plaerManagerID to the game, it could let controllers & ClollisionDetect can read and find their playerManager later
        //  Debug.Log("Instantiated player controller");
        //  Debug.Log(photonView.ViewID);
    }
    void CreateSpectator()
    {

        // Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        // Instantiate player controller
        var position = new Vector3(0, 10f, 0);
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "spectator"), position, Quaternion.identity, 0,
        new object[] { photonView.ViewID });//bring the current plaerManagerID to the game, it could let controllers & ClollisionDetect can read and find their playerManager later
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
        isDead = true;
        Death();
        //photonView.RPC("Death", RpcTarget.All);
        PhotonNetwork.Destroy(controller);
        currentHealth = playerHealth;// refresh the health;
        //CreateSpectator();
        CreateController();

    }
    public void Kill(string killerName)
    {
        Debug.Log(" !!!!!!!!!!!!!!!!!!Killed by: " + killerName + " !!!!!!!!!!!!!!!!!!");
        //isDead = true;
        OnKill();
        //photonView.RPC("OnKill", RpcTarget.All);
    }

    void Damage(float finalDamage)
    {
        currentHealth -= finalDamage;
    }

    void Death()
    {
        deathCount++;
    }

    void OnKill()
    {
        killCount++;
    }
    [PunRPC]
    void SentData(float _currentHealth, float _billboardvalue, int _killCount, int _deathCount, bool _isDead)
    {
        currentHealth = _currentHealth;
        billboardvalue = _billboardvalue;
        isDead = _isDead;
        deathCount = _deathCount;
        killCount = _killCount;
    }

    public void isMyPhotonView(PhotonView photonView)
    {
        if (!photonView.IsMine && photonView)
        {
            return;
        }
    }


}

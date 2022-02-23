using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    public string playerType;

    void Awake()
    {
        //check if there is another RoomManager exists
        if (Instance)
        {
            Destroy(gameObject); //make sure there is only one RoomManager Instance
            return;
        }
        if (SceneManager.GetActiveScene().buildIndex == 1) { }
        else
        {
            DontDestroyOnLoad(gameObject); //if there is only one,
            Instance = this;
        }

    }

    void Update()
    {

    }

    //https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnEnable.html
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (
            scene.buildIndex == 1 // We're in the game scene
        )
        {
            PhotonNetwork
                .Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"),
                Vector3.zero,
                Quaternion.identity);
        }
    }
    public void LeaveRoom()
    {

        // PhotonNetwork.Disconnect();

        // SceneManager.LoadScene(0); //Level 0 is the start menu, Level 1 is the Gaming Scene
        PhotonNetwork.LoadLevel(0);
        Debug.Log("Leaved Room");
    }
}

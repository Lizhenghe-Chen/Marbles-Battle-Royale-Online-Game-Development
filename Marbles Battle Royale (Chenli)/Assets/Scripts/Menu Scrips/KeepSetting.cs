using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
/*this script just like an temporary files that will loss after game restart  */
public class KeepSetting : MonoBehaviourPunCallbacks
{
    public static KeepSetting Instance;
    public bool playBackGroundMusic;
    public bool showTutorial = true;
    public bool returnFromGame = false;
    [Range(0f, 1f)] public float backGroundMusicVolume = 0.5f;
    // public Toggle playBackGroundMusictoggle;
    // public Slider backGroundMusicVolumeSlider;
    // [Tooltip("Find when start")] public SoundController soundController;
    // called first
    // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html

    public int start = 0; //this will help system to know the game just start or just return to menu 
    public string playerName, playerType, roomName;
    private void Awake()
    {
        //https://learn.unity.com/tutorial/implement-data-persistence-between-scenes#60b74233edbc2a54f13d5f4a
        if (Instance != null)
        {
            Destroy(gameObject); //make sure there is only one RoomManager Instance
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); //if there is only one,
    }
    // void OnEnable()
    // {
    //     Debug.Log("OnEnable called");
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }
    // void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     Debug.Log("OnSceneLoaded: " + scene.name);

    // }
    void Start()
    {
        if (KeepSetting.Instance != null)
        {
            playBackGroundMusic = KeepSetting.Instance.playBackGroundMusic;
        }
    }
    public void LeaveRoom()
    {
        Destroy(GameObject.Find("RoomManager").gameObject);
        // PhotonNetwork.Destroy(player);
        // PhotonNetwork.Destroy(this.gameObject);
        returnFromGame = true;
        PhotonNetwork.LeaveRoom();

        Debug.Log("Leaved Room");
    }
    public override void OnLeftRoom()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) { return; }
        
        SceneManager.LoadScene(0); //Level 0 is the start menu, Level 1 is the Gaming Scene

        base.OnLeftRoom();
    }
}

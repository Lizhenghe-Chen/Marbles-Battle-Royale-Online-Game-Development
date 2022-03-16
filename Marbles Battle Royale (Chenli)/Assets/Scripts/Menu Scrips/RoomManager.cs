using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    //  public AudioSource backGroundMusic;

    //public string playerType;
    public bool isTrainingGround = false, showTutorial = true;
    public Toggle showTutorialToggle;

    public bool playBackGroundMusic = true;
    public Toggle playBackGroundMusictoggle;
    public Slider backGroundMusicVolumeSlider;
    public SoundController soundController;
    public KeepSetting keepSetting;

    private void Awake()
    {
        // https://learn.unity.com/tutorial/implement-data-persistence-between-scenes#60b74233edbc2a54f13d5f4a
        //check if there is another RoomManager exists
        if (Instance != null)
        {
            Destroy(gameObject); //make sure there is only one RoomManager Instance
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); //if there is only one,

        keepSetting = GameObject.Find("KeepSetting").GetComponent<KeepSetting>();
        showTutorialToggle.isOn = keepSetting.showTutorial;
    }


    //https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnEnable.html
    public override void OnEnable()
    {
        // MenuManager.OpenMenu(LoadingMenu);
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
    public void SetTutorial()
    {
        showTutorial = showTutorialToggle.isOn;
        keepSetting.showTutorial = showTutorial;
    }
}

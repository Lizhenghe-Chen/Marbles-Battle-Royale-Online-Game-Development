using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*this script just like an temporary save files that will loss after game restart  */
public class KeepSetting : MonoBehaviour
{
    public static KeepSetting Instance;
    public bool playBackGroundMusic;

    [Range(0f, 1f)] public float backGroundMusicVolume = 0.5f;
    // public Toggle playBackGroundMusictoggle;
    // public Slider backGroundMusicVolumeSlider;
    // [Tooltip("Find when start")] public SoundController soundController;
    // called first
    // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html

    public int start = 0; //this will help system to know the game just start or just return to menu 
    public string playerName,playerType;
    private void Awake()
    {
        //https://learn.unity.com/tutorial/implement-data-persistence-between-scenes#60b7425dedbc2a54f13d5f52

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
        if (RoomManager.Instance != null)
        {
            start++;
            playBackGroundMusic = KeepSetting.Instance.playBackGroundMusic;
        }
    }

}

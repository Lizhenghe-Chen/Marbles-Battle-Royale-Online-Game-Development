using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    //public AudioSource buttonSound;
    public GameObject Canvas;
    public AudioSource backGroundMusic;
    public Toggle playBackGroundMusictoggle;
    public Slider backGroundMusicVolumeSlider;
    [SerializeField] bool isLeaveRoom = false;
    // [SerializeField] bool playBackGroundMusic = false;
    [SerializeField] NetworkManager networkManager;
    [Range(0f, 1f)]
    public float backGroundMusicVolume = 0.5f;
    // [SerializeField] RoomManager roomManager;
    [SerializeField] KeepSetting keepSetting;
    public bool playBackGroundMusic;
    // Start is called before the first frame update
    private void Awake()
    {

        //  backGroundMusicVolumeSlider
        //  backGroundMusic.volume = 0.5f;
    }
    void Start()
    {
        keepSetting = GameObject.Find("KeepSetting").GetComponent<KeepSetting>();
        backGroundMusic = transform.Find("BackGroundMusic").GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Canvas = GameObject.Find("Canvas");
            networkManager = Canvas.GetComponent<NetworkManager>();
            playBackGroundMusictoggle = Canvas.transform.Find("TitleMenu/Buttons/playBackGroundMusic").GetComponent<Toggle>();
            backGroundMusicVolumeSlider = Canvas.transform.Find("TitleMenu/Buttons/BackGroundMusicVolume").GetComponent<Slider>();

            playBackGroundMusictoggle.isOn = keepSetting.playBackGroundMusic;
            backGroundMusicVolumeSlider.value = keepSetting.backGroundMusicVolume;

        }
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {

            // playBackGroundMusictoggle = Canvas.transform.Find("TitleMenu/Buttons/playBackGroundMusic").GetComponent<Toggle>();
            // backGroundMusicVolumeSlider = Canvas.transform.Find("TitleMenu/Buttons/BackGroundMusicVolume").GetComponent<Slider>();

            //   playBackGroundMusictoggle.isOn = keepSetting.playBackGroundMusic;
            // backGroundMusicVolumeSlider.value = keepSetting.backGroundMusicVolume;


        }

        playBackGroundMusic = keepSetting.playBackGroundMusic;
        backGroundMusicVolume = keepSetting.backGroundMusicVolume;
        /*Do Not Forget set the backGroundMusic's PlayOnAwake to off!!!*/
        if (playBackGroundMusic) backGroundMusic.Play(); else backGroundMusic.Pause();

        SetVolume(backGroundMusicVolume);

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            isLeaveRoom = networkManager.isLeaveRoom;
        }
        // if (SceneManager.GetActiveScene().buildIndex == 1) { isLeaveRoom = networkManager.isLeaveRoom; }
        if (playBackGroundMusic == false) { return; }
        TurnOffMusic();
    }


    void TurnOffMusic()
    {
        if (!isLeaveRoom) { backGroundMusic.volume = backGroundMusicVolume; return; }
        backGroundMusic.volume = Mathf.Lerp(backGroundMusic.volume, 0, Time.deltaTime * 10f);
    }
    public void PauseMusic() { backGroundMusic.Pause(); }
    public void TurnOnMusic() { backGroundMusic.Play(); }
    public void SetVolume(float sliderValue)
    {
        backGroundMusic.volume = sliderValue;
        backGroundMusicVolume = sliderValue;
    }


    public void SetBackGroundMusic()
    {
        playBackGroundMusic = playBackGroundMusictoggle.isOn;
        keepSetting.playBackGroundMusic = playBackGroundMusic;
        if (playBackGroundMusic == false) { PauseMusic(); }
        else { TurnOnMusic(); }
    }
    public void SetBackGroundMusicVolume()
    {
        //Debug.Log(backGroundMusicVolumeSlider.value);
        SetVolume(backGroundMusicVolumeSlider.value);
        keepSetting.backGroundMusicVolume = backGroundMusicVolumeSlider.value;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SoundController : MonoBehaviour
{
    //public AudioSource buttonSound;
    public AudioSource backGroundMusic;
    [SerializeField] bool isLeaveRoom = false;
    NetworkManager networkManager;
    // Start is called before the first frame update
    void Start()
    {

        // DontDestroyOnLoad(buttonSound); //if there is only one,
        backGroundMusic = transform.Find("BackGroundMusic").GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene().buildIndex == 0) { networkManager = GameObject.Find("Canvas").GetComponent<NetworkManager>(); }
        // if (SceneManager.GetActiveScene().buildIndex == 1) { }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) { isLeaveRoom = networkManager.isLeaveRoom; }
        // if (SceneManager.GetActiveScene().buildIndex == 1) { isLeaveRoom = networkManager.isLeaveRoom; }

        TurnOffMusic();
    }


    void TurnOffMusic()
    {
        if (!isLeaveRoom) { backGroundMusic.volume = 0.5f; return; }
        backGroundMusic.volume = Mathf.Lerp(backGroundMusic.volume, 0, Time.deltaTime * 10f);
    }
}

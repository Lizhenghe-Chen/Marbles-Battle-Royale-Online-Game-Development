using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundCtrl : MonoBehaviour
{
    public AudioSource buttonSound;
    // Start is called before the first frame update
    void Start()
    {
        buttonSound = GameObject.Find("ButtonSound").GetComponent<AudioSource>();
        DontDestroyOnLoad(buttonSound);
    }
    public void PlayButtonSound() { buttonSound.Play(); }
}

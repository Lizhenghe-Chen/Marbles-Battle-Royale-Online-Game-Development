using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateLoading : MonoBehaviour
{
    public Animator aimator;

    private void Start()
    {
        //aimator.SetTrigger("FadeIn");
        // transform.enabled = true;
        //  this.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {

    }

    //https://docs.unity3d.com/ScriptReference/Animator.Play.html
    public void LeavingLevel()
    {
        aimator.Play("FadeOut", 0, 0);
        //aimator.SetTrigger("FadeOut");
        //  Debug.Log("Palyersssssssssssssssss");
    }
    public void LoadingLevel()
    {
        aimator.Play("FadeIn", 0, 0);
        // aimator.SetTrigger("FadeIn");
    }
}

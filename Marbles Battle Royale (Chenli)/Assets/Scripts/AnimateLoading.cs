using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateLoading : MonoBehaviour
{
    public Animator aimator;

    private void Start()
    {
        aimator.SetTrigger("FadeIn");
        // transform.enabled = true;
        //  this.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void LeavingLevel()
    {
        aimator.SetTrigger("FadeOut");
        //  Debug.Log("Palyersssssssssssssssss");
    }
    public void LoadingLevel()
    {
        aimator.SetTrigger("FadeIn");
    }
}

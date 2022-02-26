using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateLoading : MonoBehaviour
{
    public Animator aimator;
    private void Start()
    {
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
    }
    public void LoadingLevel()
    {
        aimator.SetTrigger("FadeIn");
    }
}

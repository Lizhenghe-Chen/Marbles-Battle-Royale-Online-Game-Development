using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateLoading : MonoBehaviour
{
    public Animator aimator;
    // Start is called before the first frame update
    void Start()
    {

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

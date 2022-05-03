using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
public class GameSetting : MonoBehaviour
{
    // Start is called before the first frame update
    PhotonView playerPhotonView;
    public TMP_Dropdown qualitySetting;
    [SerializeField] private int value;
    [SerializeField] private PostProcessVolume postProcessVolume;
    [SerializeField] private AmbientOcclusion ambientOcclusion;
    [SerializeField] private MotionBlur motionBlur;
    [SerializeField] private Bloom bloom;

    void Start()
    {
        //qualitySetting = transform.Find("TitleMenu/qualitySetting").GetComponent<TMP_Dropdown>();
        value = qualitySetting.value = QualitySettings.GetQualityLevel();
        // value = qualitySetting.value;
        ChangeQualityLevel();
    }

    public void ChangeQualityLevel()
    {
        postProcessVolume = GameObject.Find("Post_Process Volum").GetComponent<PostProcessVolume>();
        postProcessVolume.sharedProfile.TryGetSettings<AmbientOcclusion>(out ambientOcclusion);
        postProcessVolume.sharedProfile.TryGetSettings<Bloom>(out bloom);
        postProcessVolume.sharedProfile.TryGetSettings<MotionBlur>(out motionBlur);
        value = qualitySetting.value;

        QualitySettings.SetQualityLevel(value, true);

        if (value == 0 || value == 1 || value == 2)//very low, low, medium
        {
            bloom.enabled.Override(false);
            motionBlur.enabled.Override(false);
            ambientOcclusion.enabled.Override(false);
        }
        else
        {
            bloom.enabled.Override(true);
            motionBlur.enabled.Override(true);
            ambientOcclusion.enabled.Override(true);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameSetting : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Dropdown qualitySetting;
    [SerializeField] int value;
    void Start()
    {
        //qualitySetting = transform.Find("TitleMenu/qualitySetting").GetComponent<TMP_Dropdown>();
        value = qualitySetting.value = QualitySettings.GetQualityLevel();
        value=qualitySetting.value;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ChangeQualityLevel()
    {
        value=qualitySetting.value;
        QualitySettings.SetQualityLevel(value, true);
    }
}

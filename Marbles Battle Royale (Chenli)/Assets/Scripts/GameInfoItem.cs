using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameInfoItem : MonoBehaviour
{
    public TMP_Text gameInfoMessage;
    public string message;
    [SerializeField] float coolingTime = 5, time = 0;//time to remove the item
    void Update()
    {
        time += Time.deltaTime;
        if (time >= coolingTime)
        {
            time = coolingTime;
            Destroy(this.gameObject);
            //time = 0;
        }
    }
    public GameInfoItem(string message)
    {
        this.message = message;
    }
    public override string ToString()
    {
        return "[" + message + "]";
    }
    public void Initialize(string message)
    {
        Debug.LogWarning("gameInfoMessage Initialize :" + message);
        gameInfoMessage.text = message;


    }
}

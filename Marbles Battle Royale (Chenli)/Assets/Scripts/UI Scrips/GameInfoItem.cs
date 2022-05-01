using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameInfoItem : MonoBehaviour
{
    public TMP_Text gameInfoMessage;
    public string message;
    [SerializeField]
    float coolingTime = 5;//time to remove the item
    void Start()
    {
        Invoke("Destory", coolingTime);
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
    void Destory() { Destroy(this.gameObject); }
}

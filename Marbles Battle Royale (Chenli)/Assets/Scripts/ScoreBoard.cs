using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class ScoreBoard : MonoBehaviour
{
    public TMP_Text userNameText, killsText, deathText;
    [SerializeField] Color localPlayerScoreColor;

    public void Initialize(string nickName, int killCount, int deathCount, bool isLocalPlayer)
    {
        if (isLocalPlayer) { transform.Find("Image").GetComponent<Image>().color = localPlayerScoreColor; }

        // ViewID=transform.parent.parent.GetComponent<PhotonView>().Owner.NickName;
        userNameText.text = nickName;
        deathText.text = deathCount.ToString();
        killsText.text = killCount.ToString();

    }
}
public class ScoreBoardItem : IComparable<ScoreBoardItem>
{
    public string playerName;
    public int killCount, deathCount;

    public ScoreBoardItem(string playerName, int killCount, int deathCount)
    {
        this.playerName = playerName;
        this.killCount = killCount;
        this.deathCount = deathCount;
    }
    public override string ToString()
    {
        return "[" + playerName + "\tkill:\t" + killCount + "\tdeath:\t" + deathCount + "]";
    }
    //IComparable 
    public int CompareTo(ScoreBoardItem other)
    {
        return NormalCompareMethod(other);
        // return AverangeCompareMethod(other);
    }
    int NormalCompareMethod(ScoreBoardItem other)
    {
        if (other.killCount > this.killCount)
        {
            return 1;
        }
       
        else if (other.killCount == this.killCount)
        {
            if (other.deathCount > this.deathCount) { return -1; } else return 1;

        }
        else return -1;
    }
    int AverangeCompareMethod(ScoreBoardItem other)
    {
        if ((other.killCount - this.deathCount) > 0)
        {
            return 1;
        }
        else if ((other.killCount - this.deathCount)== 0) { return 0; }

        else return -1;
    }
}

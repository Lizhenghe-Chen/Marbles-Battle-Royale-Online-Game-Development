using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ScoreBoardManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject ScoreBoardCanvas, scoreBoardPrefabs;
    [SerializeField] Transform container;
    public PhotonView pV;
    Dictionary<string, ScoreBoard> scoreBoardItems = new Dictionary<string, ScoreBoard>();// playerName as key, ScoreBoard(item) as value
    List<ScoreBoardItem> list = new List<ScoreBoardItem>();
    [SerializeField] TMP_Text ScoreboardText;

    void Awake()
    {
        if (this.name == "ScoreBoardCanvas (1)") { pV = transform.parent.parent.parent.parent.GetComponent<PhotonView>(); }
        else if (this.name == "ScoreBoardCanvas (3)")
        {
            //Debug.Log("!!@#$!@$!$!@$#@!$!$!$!@$!$!@$!" + transform.parent.name);
             pV = this.transform.parent.GetComponent<PhotonView>();
        }
        else
            pV = transform.parent.parent.GetComponent<PhotonView>();

    }
    void Start()
    {
        if (!pV.IsMine) { Destroy(ScoreBoardCanvas); return; }
        // foreach (GameObject obj in GameObject.FindGameObjectsWithTag("PlayerManager"))
        // {
        //     PlayerManager playermanManager = obj.GetComponent<PlayerManager>();
        //     PhotonView photonView = obj.GetComponent<PhotonView>();
        //     var nickName = photonView.Owner.NickName;
        //     // Debug.Log(nickName + "death" + playermanManager.deathCount);
        //     list.Add(new ScoreBoardItem(nickName, playermanManager.deathCount, playermanManager.killCount));
        // }
        // foreach (ScoreBoardItem item in list) { Debug.Log(item.ToString()); }
    }
    void Update()
    {

        Refresh();

        // if (GameObject.FindGameObjectsWithTag("PlayerManager").Length != scoreBoardItems.Count)//some player enter or leave the game
        // {

        //     Refresh();
        // }
    }
    void LateUpdate()
    {


    }

    void Refresh()
    {
        // Debug.Log( GameObject.FindGameObjectsWithTag("PlayerManager").Length);
        list.Clear();
        // scoreBoardItems.Clear();
        foreach (Transform child in container)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("PlayerManager"))
        {
            PlayerManager playermanManager = obj.GetComponent<PlayerManager>();
            var nickName = obj.GetComponent<PhotonView>().Owner.NickName;
            // Debug.Log(nickName + "death" + playermanManager.deathCount);

            list.Add(new ScoreBoardItem(nickName, playermanManager.killCount, playermanManager.deathCount, playermanManager.isDead));
            // AddScoreboardItem(nickName, playermanManager.deathCount, playermanManager.killCount);
        }
        list.Sort();
        // string message = string.Empty;
        // Debug.Log(list.Count);
        foreach (ScoreBoardItem item in list)
        {
            // message += item.ToString() + "\n";

            AddScoreboardItem(item.playerName, item.killCount, item.deathCount, item.isDead);
        }
        // ScoreboardText.text = message;
        // message = string.Empty;
    }
    void AddScoreboardItem(string playerName, int killCount, int deathCount, bool isDead)
    {
        bool isLocalPlayer = false;
        if (playerName == pV.Owner.NickName) { isLocalPlayer = true; }
        ScoreBoard item = Instantiate(scoreBoardPrefabs, container).GetComponent<ScoreBoard>();
        item.Initialize(playerName, killCount, deathCount, isLocalPlayer, isDead);
        // scoreBoardItems[playerName] = item;

    }
    // void RemoveScoreboardItem(string nickName)
    // {
    //      Destroy(scoreBoardItems[nickName].gameObject);
    //     scoreBoardItems.Remove(nickName);
    // }
    // public override void OnPlayerEnteredRoom(Player otherPlayer)//when new player entered the room, add to board
    // {
    //     AddScoreboardItem(otherPlayer);
    // }
    // public override void OnPlayerLeftRoom(Player otherPlayer)
    // {
    //     RemoveScoreboardItem(otherPlayer);
    // }
}

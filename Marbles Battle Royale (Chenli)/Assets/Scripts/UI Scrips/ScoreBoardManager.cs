using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
/*
* Copyright (C) 2022 Author: Lizhenghe.Chen.
* For personal study or educational use.
* Email: Lizhenghe.Chen@qq.com
*/
public class ScoreBoardManager : MonoBehaviourPunCallbacks
{
    //public static ScoreBoardManager Instance ;
    [SerializeField] GameObject ScoreBoardCanvas, scoreBoardPrefabs;
    [SerializeField] Transform container;
    public PhotonView playerPhotonView;
    Dictionary<string, ScoreBoard> scoreBoardItems = new Dictionary<string, ScoreBoard>();// playerName as key, ScoreBoard(item) as value
    List<ScoreBoardItem> list = new List<ScoreBoardItem>();
    [SerializeField] TMP_Text ScoreboardText;
    [SerializeField] GameInfoManager gameInfoManager;
    public string type;
    void Awake()
    {
        //Instance=this;
        if (type == "InGameMenu")//player InGameMenu score board
        {
            playerPhotonView = transform.parent.parent.parent.GetComponent<PhotonView>();
        }
        else if (type == "spectator")//player spectector upright
        {
            //Debug.Log("!!@#$!@$!$!@$#@!$!$!$!@$!$!@$!" + transform.parent.name);
            playerPhotonView = this.transform.parent.parent.GetComponent<PhotonView>();
        }
        else
            playerPhotonView = transform.parent.parent.GetComponent<PhotonView>();//player OnGame upright

    }
    void Start()
    {
        gameInfoManager = GameObject.Find("GameInfoCanvas/GameInfo").GetComponent<GameInfoManager>();
        gameInfoManager.scoreBoardManager = this;
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
        //if (!playerPhotonView.IsMine) { return; }
        // Refresh();
        // if (GameObject.FindGameObjectsWithTag("PlayerManager").Length != scoreBoardItems.Count)//some player enter or leave the game
        // {

        //     Refresh();//insted loop, AllRefreshScoreBoard() in other scripts can reduce cpu pressure
        // }
    }
    void LateUpdate()
    {

    }
    // [PunRPC]
    // public void AllRefresh()//this will send messages to all players in a room
    // {
    //     Debug.Log("Refresh ScoreBoard");
    //     Refresh();

    // }

    public void Refresh()
    {
        foreach (Transform child in container)
        {
            GameObject.Destroy(child.gameObject);
        }
        list.Clear();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("PlayerManager"))
        {
            PlayerManager playermanManager = obj.GetComponent<PlayerManager>();
            var nickName = obj.GetComponent<PhotonView>().Owner.NickName;
            Debug.Log(nickName + "death" + playermanManager.deathCount);

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
        Debug.Log("!!!!!Refreshed ScoreBoard");
    }
    void AddScoreboardItem(string playerName, int killCount, int deathCount, bool isDead)
    {
        bool isLocalPlayer = false;
        if (playerName == playerPhotonView.Owner.NickName) { isLocalPlayer = true; }
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

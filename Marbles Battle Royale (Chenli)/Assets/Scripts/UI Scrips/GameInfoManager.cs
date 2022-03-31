using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GameInfoManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject GameInfoCanvas, gameInfoPrefabs;
    [SerializeField] Transform container;
    [SerializeField] PhotonView photonView;
    public ScoreBoardManager scoreBoardManager;//will be set in CollisionDetect and SpectatorMovement scrips 
    //[SerializeField] float coolingTime = 5, time;//time to remove the first item
    void Start()
    {
        photonView = transform.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (container.childCount == 0)
        // {
        //     // Debug.Log("nope");
        //     return;
        // }
        // time += Time.deltaTime;
        // if (time >= coolingTime)
        // {
        //     time = coolingTime;
        //     Remove();
        //     time = 0;
        // }
    }

    void Remove() { Destroy(container.GetChild(0).gameObject); }
    public void Refresh(string message)//this will send messages to all players in a room
    {
        Debug.Log("RPC send " + message);
        photonView.RPC("GlobalRefresh", RpcTarget.All, message);
//photonView.RPC("AllRefreshScoreBoard", RpcTarget.All);
    }
    // public void LocalRefresh(string message)//this will send messages to all players in a room
    // {
    //     //   GlobalRefresh(message);
    //     photonView.RPC("LRefresh", RpcTarget.All, message);
    // }
    [PunRPC]
    public void GlobalRefresh(string message) { AddGameInfoItem(message);  }

    public void LRefresh(string message) { if (photonView.IsMine) { AddGameInfoItem(message); } }
    void AddGameInfoItem(string message)
    {
        //  if (photonView.Owner.IsMasterClient) { return; }
        GameInfoItem item = Instantiate(gameInfoPrefabs, container).GetComponent<GameInfoItem>();
        item.Initialize(message);
        // scoreBoardItems[playerName] = item;

    }

    [PunRPC]
    public void AllRefreshScoreBoard()//tell all players to refresh their score board
    {
        Debug.Log("!!!!!Refresh ScoreBoard");
        ScoreBoardManager.Instance.Refresh();

    }
}

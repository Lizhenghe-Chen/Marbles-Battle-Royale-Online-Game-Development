using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameInfoManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject GameInfoCanvas, gameInfoPrefabs;
    [SerializeField] private Transform container;
    public PhotonView pV;
    public ScoreBoardManager scoreBoardManager;//will be set in CollisionDetect and SpectatorMovement scrips 
    [SerializeField] private GameObject[] playerList;                                          //[SerializeField] float coolingTime = 5, time;//time to remove the first item
    public string currentWinnerName;
    public bool gameOver = false;

    public Canvas GameOverCanvas;
    public TMP_Text GameOverText;
    private void Start()
    {
        pV = GetComponent<PhotonView>();
    }

    // void Remove() { Destroy(container.GetChild(0).gameObject); }
    public void GlobalRefresh(string message)//this will send messages to all players in a room
    {
        Debug.Log("RPC send " + message);
        pV.RPC("Refresh", RpcTarget.All, message);
        pV.RPC("RefreshScoreBoard", RpcTarget.All);
    }

    [PunRPC]
    public void Refresh(string message)//local message
    {
        AddGameInfoItem(message);

    }

    public void LRefresh(string message) { if (pV.IsMine) { AddGameInfoItem(message); } }
    void AddGameInfoItem(string message)
    {
        //  if (pV.Owner.IsMasterClient) { return; }
        GameInfoItem item = Instantiate(gameInfoPrefabs, container).GetComponent<GameInfoItem>();
        item.Initialize(message);
    }

    [PunRPC]//tell all players to refresh their score board
    public void RefreshScoreBoard()
    {
        scoreBoardManager.Refresh();
        CheckPlayerList();
        // Invoke("InvokeScoreBoard", 0.5f);
    }

    public void CheckPlayerList()
    {
        Debug.Log("CheckPlayerList");
        playerList = GameObject.FindGameObjectsWithTag("Player");
        if (playerList.Length <= 0)
        {
            gameOver = true;
            // Debug.LogWarning("No Player in the room currently.");
        }
        else if (playerList.Length == 1)
        {
            currentWinnerName = playerList[0].GetComponent<PhotonView>().Owner.NickName;
            gameOver = true;
        }
        else
        {
            gameOver = false;
            GameOverCanvas.enabled = false;
        }
    }
}

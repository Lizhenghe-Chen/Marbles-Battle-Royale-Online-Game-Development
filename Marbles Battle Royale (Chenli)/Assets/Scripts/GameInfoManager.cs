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
    [SerializeField] float coolingTime = 5, time;//time to remove the first item
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
    public void Refresh(string message) { photonView.RPC("GlobalRefresh", RpcTarget.All, message); }
    [PunRPC]
    public void GlobalRefresh(string message) { AddGameInfoItem(message); }
    void AddGameInfoItem(string message)
    {
        GameInfoItem item = Instantiate(gameInfoPrefabs, container).GetComponent<GameInfoItem>();
        item.Initialize(message);
        // scoreBoardItems[playerName] = item;

    }
}

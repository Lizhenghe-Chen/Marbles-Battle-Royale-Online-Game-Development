using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickTransfer : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform transitionTarget;
    //bool reachEnd = false;
    public Transform Player;


    [SerializeField] GameInfoManager GameInfoManager;
    void Start()
    {
        GameInfoManager = GameObject.Find("GameInfoCanvas/GameInfoTitle").GetComponent<GameInfoManager>();
    }
    private void Update()
    {
        // if (Player == null) { return; }
        // if (Player.position == transitionTarget.position)
        // {
        //     reachEnd = true;
        //     Player = null;
        //     return;
        // }
        // else { Player.position = Vector3.MoveTowards(Player.position, transitionTarget.position, 1); }

    }
  
    void OnTriggerEnter(Collider collision)
    {

        AnimateLoading playerAnimateLoading = collision.transform.Find("UI/Canvas/Image").GetComponent<AnimateLoading>();
        playerAnimateLoading.LeavingLevel();

        Player = collision.transform;
        Invoke("QuickTransfering", 1f);
    }



    void QuickTransfering()
    {
        GameInfoManager.Refresh(Player.GetComponent<CollisionDetect>().player_Name + " Transfered to fnnel");
        Player.position = transitionTarget.position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ToolBox : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform transitionTarget;
    //bool reachEnd = false;
    public Transform Player;
    [SerializeField] PhotonView photonView, GameInfoPhotonView;

    [SerializeField] GameInfoManager GameInfoManager;
    void Start()
    {
        GameInfoManager = GameObject.Find("GameInfoCanvas/GameInfo").GetComponent<GameInfoManager>();
        photonView = GetComponent<PhotonView>();
        GameInfoPhotonView = GameObject.Find("GameInfoCanvas/GameInfo").GetComponent<PhotonView>();
        if (!GameInfoPhotonView.IsMine) { this.enabled = false; }
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

    private void OnTriggerEnter(Collider collision)//for transfer
    {
        // if (!PhotonNetwork.IsMasterClient) { return; }
        if (collision.GetComponent<Rigidbody>() == null) { return; }
        Player = collision.transform;

        Invoke("QuickTransfering", 0);

    }
    private void OnCollisionEnter(Collision other)
    {
        //  if (!PhotonNetwork.IsMasterClient) { return; }
        if (this.name == "ExtraLife" && other.transform.tag == "Player")
        {
            Player = other.transform;
            var otherPlayerManager = Player.GetComponent<MovementController>().playerManager;
            otherPlayerManager.maxLife++;
            otherPlayerManager.leftLifeTextContent = "Rest Life: " + (otherPlayerManager.maxLife - otherPlayerManager.deathCount - 1);
            photonView.RPC("DestroyForAll", RpcTarget.All);
            //DestroyForAll();//destroy this game object for all players
            //PhotonNetwork.Destroy(this.gameObject);
        }
    }
    private void OnCollisionStay(Collision other)
    {
        //if (!PhotonNetwork.IsMasterClient) { return; }
        if (this.name == "ExtraLife") { return; }

        if (other.transform.tag == "Player")
        {
            Player = other.transform;
            var otherCollisionDetect = Player.GetComponent<CollisionDetect>();
            if (this.name == "Enlarge" && Player.localScale.x <= otherCollisionDetect.initialScale.x * otherCollisionDetect.maxScale) { other.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f); }
            if (this.name == "Shrink" && Player.localScale.x >= otherCollisionDetect.initialScale.x * otherCollisionDetect.minScale) { other.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f); }
        }
        if (other.transform.tag == "Robot")
        {
            Player = other.transform;
            var otherRobotController = Player.GetComponent<RobotController>();
            if (this.name == "Enlarge" && Player.localScale.x <= otherRobotController.initialScale.x * otherRobotController.maxScale) { other.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f); }
            if (this.name == "Shrink" && Player.localScale.x >= otherRobotController.initialScale.x * otherRobotController.minScale) { other.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f); }
        }
    }



    public void QuickTransfering()
    {
        if (Player.tag == "Player")
        {
            if (Player.GetComponent<MovementController>().photonView.IsMine)
            {
                AnimateLoading playerAnimateLoading = Player.Find("UI/Canvas/Image").GetComponent<AnimateLoading>();
                playerAnimateLoading.LoadingLevel();
                GameInfoManager.GlobalRefresh(Player.GetComponent<CollisionDetect>().player_Name + " Transfered to " + transitionTarget.name);
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient) { GameInfoManager.Refresh(Player.name + " Transfered to " + transitionTarget.name); }
        }

        Player.position = transitionTarget.position;
    }
    [PunRPC]
    public void DestroyForAll()
    {

        Destroy(this.gameObject);
        // Destroy(PhotonView.Find(viewID).gameObject);
    }
}

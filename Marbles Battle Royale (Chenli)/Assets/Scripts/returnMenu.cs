using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
public class returnMenu : MonoBehaviourPunCallbacks
{

    public void LeaveRoom()
    {
        Destroy(GameObject.Find("RoomManager").gameObject);
        //PhotonNetwork.Disconnect();
        // PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LoadLevel(0);
        PhotonNetwork.LeaveRoom();
        //if (PhotonNetwork.CurrentRoom != null) { PhotonNetwork.Disconnect(); }
         SceneManager.LoadScene(0); //Level 0 is the start menu, Level 1 is the Gaming Scene
        Debug.Log("Leaved Room");
    }
}

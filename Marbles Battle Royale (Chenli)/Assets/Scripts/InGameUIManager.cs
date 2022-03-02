using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class InGameUIManager : MonoBehaviour
{
    [Header("InGameUIList & InGameUICanvas need Manually attach: ")]
    public GameObject[] InGameUIList;

    [SerializeField] PhotonView photonView;
    PlayerManager playerManager;
    [SerializeField] bool menueOpen = false;
    [SerializeField] GameObject InGameUI;
    private void Awake()
    {
        Debug.Log("Paraent " + transform.parent);
        photonView = transform.parent.parent.GetComponent<PhotonView>();

    }
    void Start()
    {
        InGameUI = transform.Find("InGameMenu").gameObject;
        playerManager = transform.parent.parent.GetComponent<MovementController>().playerManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) { menueOpen = !menueOpen; }
        //  Debug.Log(counter);

        if (!menueOpen)
        {
            InGameUI.SetActive(false);
            Cursor.visible = false;
        }
        else
        {
            InGameUI.SetActive(true);
            Cursor.visible = true;

        }
        //if (playerManager.isDead == true) { OpenMenu("DeadInfo"); } 

    }
    public void OpenMenu(string menuName)
    {
        //Debug.Log("Go to " + menuName);
        for (int i = 0; i < InGameUIList.Length; i++)
        {
            if (InGameUIList[i].name == menuName)
            {
                // Debug.Log("set " + menuList[i] + "Active");
                InGameUIList[i].SetActive(true);
            }
            else
            {
                // Debug.Log("set " + menuList[i] + "DeActive");
                InGameUIList[i].SetActive(false);
            }
        }
    }
    public void LeaveRoom()
    {
        playerManager.LeaveRoom();
        // Destroy(GameObject.Find("RoomManager").gameObject);
        // //PhotonNetwork.Disconnect();
        // // PhotonNetwork.LeaveRoom();
        // //PhotonNetwork.LoadLevel(0);
        // PhotonNetwork.LeaveRoom();
        // //if (PhotonNetwork.CurrentRoom != null) { PhotonNetwork.Disconnect(); }
        //  SceneManager.LoadScene(0); //Level 0 is the start menu, Level 1 is the Gaming Scene
        // Debug.Log("Leaved Room");
    }
}

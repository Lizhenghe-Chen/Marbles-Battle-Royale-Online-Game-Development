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
    RoomManager roomManager;
    Rigidbody rb;
    [SerializeField] GameObject InGameUI, ControlTipsUI, ControlTipsNotice;
    private void Awake()
    {
        Debug.Log("Paraent " + transform.parent);
        photonView = transform.parent.parent.GetComponent<PhotonView>();
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }
    void Start()
    {
        InGameUI = transform.Find("InGameMenu").gameObject;
        ControlTipsUI = transform.Find("ControlTips").gameObject;
        ControlTipsNotice = transform.Find("ControlTipsNotice").gameObject;
        rb = transform.parent.parent.GetComponent<MovementController>().rb;
        playerManager = transform.parent.parent.GetComponent<MovementController>().playerManager;

        ControlTipsNotice.SetActive(false);
        if (!roomManager.isTrainingGround) { ContrilTips(); }//otherwise no showup when game start
        InGameMenu();//set menu off when start
        // if (!photonView.IsMine)
        // {
        //     Destroy(transform.parent);
        // } 
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InGameMenu();
        }
        if (InGameUI.activeSelf) { rb.angularVelocity = Vector3.zero; }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ContrilTips();
        }

        //if (playerManager.isDead == true) { OpenMenu("DeadInfo"); } 

    }
    public void InGameMenu()
    {

        if (InGameUI.activeSelf)//if is flase
        {
            InGameUI.SetActive(false);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            InGameUI.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void ContrilTips()
    {

        if (ControlTipsUI.activeSelf)
        {
            ControlTipsNotice.SetActive(true);
            ControlTipsUI.SetActive(false);
        }
        else
        {
            ControlTipsNotice.SetActive(false);
            ControlTipsUI.SetActive(true);
        }
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

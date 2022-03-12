using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TranningGroundObjects : MonoBehaviour
{
    [SerializeField] RoomManager roomManager;
   // public GameObject robot;
    void Awake() { roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>(); }
    void Start()
    {
        if (!roomManager.isTrainingGround) { Destroy(this.gameObject); return; }
       // Instantiate(robot, new Vector3(0,0,0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

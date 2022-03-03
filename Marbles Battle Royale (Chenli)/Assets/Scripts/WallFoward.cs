using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFoward : MonoBehaviour
{
    [SerializeField] float moveSpeed = 0.05f;
    [SerializeField] Transform endPoint;
    [SerializeField] RoomManager roomManager;
    void Start()
    {
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (roomManager.isTrainingGround) { transform.position = new Vector3(-110, 0, 0); return; }

        if (transform.position.x <= endPoint.position.x) { transform.position += new Vector3(moveSpeed, 0, 0); }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBillbord : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform Player;
    void Start()
    {
        Player = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (cam == null) { cam = FindObjectOfType<Camera>(); return; }
        transform.LookAt(cam.transform);
        BillBoardFollow();
    }
    public void BillBoardFollow()
    {
        //float lerpValue = Mathf.Lerp(transform.position, Player.position, 0.0005f);
        Vector3 target = new Vector3(Player.position.x, Player.position.y + 3, Player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, 100);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBillbord : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform target;
    void Start()
    {
        target = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (cam == null) { cam = FindObjectOfType<Camera>(); return; }
        transform.LookAt(cam.transform);
        Billboard.BillBoardFollow(target, this.transform, 2);
    }

}

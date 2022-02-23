using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keep : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform Player;


    //public Transform Camera;

    public float closest_dist = 3f;

    public float moveSpeed = 2f;

    public float maxDistance = 2f;

    public static float currentDistance;

    void Start()
    {
        Player = transform.parent;
        // Debug.Log(Player);
    }

    // Update is called once per frame
    void Update()
    {
        BillBoardFollow();
    }

    public void Move()
    {
        transform.LookAt(Player.position);
        float currentDistance =
            Vector3.Distance(transform.position, Player.position);

        Debug
            .Log("Camera:" +
            currentDistance +
            "Follower:" +
            (currentDistance / maxDistance));
        if (currentDistance > maxDistance)
        {
            transform
                .Translate(Vector3.forward *
                (currentDistance / maxDistance) *
                moveSpeed);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            // transform.Translate(Vector3.forward * 0.5f); //速度可调  自行调整
            maxDistance += 0.5f; //速度可调  自行调整w
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && maxDistance > 5)
        {
            // transform.Translate(Vector3.forward * -0.5f); //速度可调  自行调整
            maxDistance -= 0.5f; //速度可调  自行调整
        }
    }
    void BillBoardFollow()
    {
        //float lerpValue = Mathf.Lerp(transform.position, Player.position, 0.0005f);
        Vector3 target = new Vector3(Player.position.x, Player.position.y + 2f, Player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, 100);
    }
}

//  pos = (transform.position - Player.position).normalized;

//         if (currentDistance > maxDistance)
//         {
//             //获得两个物体之间的单位向量
//             //单位向量乘以最远的距离系数
//             pos *= maxDistance * (currentDistance - maxDistance);

//             //物体A的坐标加上距离向量
//             transform.position = pos + Player.position;
//         }

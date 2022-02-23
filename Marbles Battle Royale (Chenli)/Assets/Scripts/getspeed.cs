using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is aim to solve the problem that the player cannot have same inertance with moving object,
// this script will be used, actived and deactivated by player's CollisionDetect.cs.
public class getspeed : MonoBehaviour
{
    Vector3 PrevPos;

    Vector3 NewPos;

    public Vector3 ObjVelocity;

    void Awake()
    {
      //  this.enabled = false;
    }

    void Start()
    {
        PrevPos = transform.position;
        NewPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        NewPos = transform.position; // each frame track the new position
        ObjVelocity = (NewPos - PrevPos) / Time.fixedDeltaTime; // velocity = dist/time
        PrevPos = NewPos; // update position for next frame calculation
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickTransfer : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform transitionTarget;
    bool reachEnd = false;
    public Transform Player;
    void Start()
    {

    }
    private void Update()
    {
        if (Player == null) { return; }
        if (Player.position == transitionTarget.position)
        {
            reachEnd = true;
            Player = null;
            return;
        }
        else { Player.position = Vector3.MoveTowards(Player.position, transitionTarget.position, 1); }

    }

    void OnCollisionEnter(Collision collision)
    {
        reachEnd = false;
        Player = collision.transform;
        // collision.transform.position = transitionTarget.position;
        Debug.LogWarning("OHHHHHHHh");
    }
}

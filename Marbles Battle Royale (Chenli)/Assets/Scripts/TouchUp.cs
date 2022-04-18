using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TouchUp : MonoBehaviour
{
    PhotonView photonView;

    public float Speed = 3f;

    public float returnSpeed = 2f;

    public float upRange = 10f;

    public float downRange = 5f;

    public float waitTime = 0.3f;

    // Start is called before the first frame update
    [SerializeField] Vector3 initial_location;
    public bool invert = false;
    bool isColliding = false;

    // private void Awake()
    // {
    //     if (this.tag == "elevator")
    //     {
    //         //   this.enabled = false;
    //     }
    // }

    void Start()
    {
        initial_location = this.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (isColliding == false)
        {
            // Debug.LogWarning("Return"); 
            transform.position = Vector3.MoveTowards(transform.position, initial_location, returnSpeed * Time.deltaTime);
            if (transform.position == initial_location) { this.enabled = false; }
        }
        // if (transform.position.y > initial_locationY && isColliding == false)
        // {
        //     //Debug.Log("Down");
        //     transform.position -= Vector3.up * downSpeed * Time.deltaTime;
        // }
    }

    void OnTriggerStay(Collider collision)
    {
        if (collision.GetComponent<Rigidbody>())
        {
            isColliding = true;
            Invoke("lift", 0);
            // if (transform.position.y <= initial_locationY + upRange)
            // {
            //     Invoke("lift", waitTime);
            // }
        }
    }

    void OnTriggerExit(Collider collision)
    {
        isColliding = false;
    }

    void lift()
    {
        if (!invert && transform.position.y <= initial_location.y + upRange) { transform.position += Vector3.up * Speed * Time.deltaTime; }
        if (invert && transform.position.y >= initial_location.y - downRange) { transform.position += Vector3.down * Speed * Time.deltaTime; }
    }
}

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TouchUp : MonoBehaviour
{
    PhotonView photonView;

    public float upSpeed = 5f;

    public float downSpeed = 2f;

    public float upRange = 10f;

    public float downRange = 5f;

    public float waitTime = 0.3f;

    // Start is called before the first frame update
    float initial_locationY;

    bool isColliding = false;

    private void Awake()
    {
        if (this.tag == "elevator")
        {
         //   this.enabled = false;
        }
    }

    void Start()
    {
        if (photonView)
        {
            if (!photonView.IsMine)
            {
                return;
            }
        }

        if (this.enabled == true)
        {
            initial_locationY = this.transform.position.y;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > initial_locationY && isColliding == false)
        {
            //Debug.Log("Down");
            transform.position -= Vector3.up * downSpeed * Time.deltaTime;
        }
    }

    void OnTriggerStay(Collider collision)
    {
        if (collision.GetComponent<Rigidbody>())
        {
            isColliding = true;

            if (transform.position.y <= initial_locationY + upRange)
            {
                Invoke("lift", waitTime);
            }
        }
    }

    void OnTriggerExit(Collider collision)
    {
        isColliding = false;
    }

    void lift()
    {
        transform.position += Vector3.up * upSpeed * Time.deltaTime;
    }
}

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpController : MonoBehaviour
{
    Vector3 target;

    PhotonView photonView;

    float jumpThreshold; //the Raycast distance, which will determine the onTheGround()

    Rigidbody rg;

   [SerializeField] const float distanceBetweenGround = 0.5f;

    public float sliteForce = 2f;
    public float extra_gravity = 10f;

    private MovementController movementController;

    private CollisionTrigger CollisionTrigger;

    //AudioSource audio;
    // Start is called before the first frame update
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        movementController = GetComponent<MovementController>();
        CollisionTrigger = GetComponentInChildren<CollisionTrigger>();
        rg = GetComponent<Rigidbody>(); //find the Rigidbody object
        jumpThreshold = GetComponent<SphereCollider>().radius + distanceBetweenGround;
        
        // Debug.Log(GetComponent<SphereCollider>().radius);
    }

    // int jumpCount = 0;
    public float jumpforce;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        GiveLittileForce();
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        JumpCommand();

        // rg.AddForce(Vector3.down * extra_gravity);

        target = transform.localPosition;

        // Debug.Log(target.x + "," + target.y + "," + target.z);
    }

    // void OnCollisionStay(Collision col)
    // {
    //     jumpCount = 1;
    // }
    // void OnCollisionExit(Collision col)
    // {
    //     jumpCount = 0;
    //     //  Debug.Log( " In the Air" );
    // }
    void GiveLittileForce()
    {
        if (onTheGround() == false)
        {
            //  Debug.Log("sliteForce");
            var force =
                (Input.GetKey(KeyCode.LeftShift) ? sliteForce * 2 : sliteForce);
            rg
                .AddForce(movementController.Camera.transform.forward *
                force *
                movementController.verticalInput);
            rg
                .AddForce(movementController.Camera.transform.right *
                force *
                movementController.horizontalInput);
        }
    }

    void JumpCommand()
    {
        //Debug.Log(CollisionTrigger.onTheGround);
        if (Input.GetKeyDown(KeyCode.Space) && onTheGround())
        {
            rg.AddForce(Vector3.up * jumpforce);

            //   Debug.Log(a + "Space pressed and jump" + jumpCount);
        }
    }

    bool onTheGround()
    {
        Ray checkGround = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        Color rayColor;
        if (Physics.Raycast(checkGround, out hit, jumpThreshold))
        {
            rayColor = Color.green;
            //Debug.Log("i'm grounded");
        }
        else
        {
            rayColor = Color.red;
            //Debug.Log("not grounded");
        }
        Debug
            .DrawRay(transform.position, Vector3.down, rayColor, jumpThreshold);

        return hit.collider;
    }
}

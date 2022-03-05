using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
public class JumpController : MonoBehaviour
{
    Vector3 target;

    PhotonView photonView;

    float jumpThreshold; //the Raycast distance, which will determine the onTheGround()

    Rigidbody rg;

    [SerializeField] const float distanceBetweenGround = 0.5f;
    [SerializeField] float angularVelocity;
    //[SerializeField] Transform Camera;
    public float sliteForce = 2f;
    public float extra_gravity = 10f;

    private MovementController movementController;

    private CollisionTrigger CollisionTrigger;
    public bool Grounded, OnCollisionGrounded;
    PlayerManager playerManager;

    //===============================
    [Header("**Below for players Jump function,jumpPane need attach menually **\n")]
    [SerializeField] Image jumpPanel;
    [SerializeField] float coolingTime = 2, time, rushValue;//player jump cooling time
    //AudioSource audio;
    // Start is called before the first frame update
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        time = coolingTime;
        if (!photonView.IsMine)
        { return; }

        playerManager = GetComponent<MovementController>().playerManager;
        movementController = GetComponent<MovementController>();
        CollisionTrigger = GetComponentInChildren<CollisionTrigger>();
        rg = GetComponent<Rigidbody>(); //find the Rigidbody object
        jumpThreshold = GetComponent<SphereCollider>().radius + distanceBetweenGround;
        jumpPanel = transform.Find("UI/Canvas/JumpLoading/jumpPanel").GetComponent<Image>();
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

        JumpMethod();
    }

    void OnCollisionStay(Collision col)
    {
        //   if (col.collider.tag == "Terrain")
        OnCollisionGrounded = true;
    }
    void OnCollisionExit(Collision col)
    {
        //  if (col.collider.tag == "Terrain")
        OnCollisionGrounded = false;
        //  Debug.Log( " In the Air" );
    }
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
        if (Input.GetKeyDown(KeyCode.Space) && onTheGround())
        {
            rg.AddForce(Vector3.up * jumpforce);
            time = 0;
        }
    }

    bool onTheGround()
    {
        Ray checkGround = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        Color rayColor;
        if (Physics.Raycast(checkGround, out hit, jumpThreshold) || (!Physics.Raycast(checkGround, out hit, jumpThreshold) && OnCollisionGrounded))
        {
            Grounded = true;
            rayColor = Color.green;
            //Debug.Log("i'm grounded");
        }
        else
        {
            Grounded = false;
            rayColor = Color.red;
            //Debug.Log("not grounded");
        }
        Debug
            .DrawRay(transform.position, Vector3.down, rayColor, jumpThreshold);

        return Grounded;
    }
    void JumpMethod()
    {
        time += Time.deltaTime;
        rushValue = time / coolingTime;
        jumpPanel.fillAmount = rushValue;
        if (time >= coolingTime)
        {
            time = coolingTime;
            JumpCommand();

        }
    }
    // public void ParticleSystemJudge()
    // {
    //     angularVelocity = rg.angularVelocity.magnitude;
    //     if (angularVelocity >= 15f && onTheGround())
    //     {
    //         playerManager.SetParticle(true);

    //     }
    //     else
    //     {
    //         playerManager.SetParticle(false);

    //     }
    // }
}

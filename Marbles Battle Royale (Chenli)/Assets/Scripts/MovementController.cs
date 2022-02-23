using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    [Tooltip("when In Locoal debug state, mark this")]
    [SerializeField] bool isInDebugMode = false;
    [Header("**Below Parameters should find by themsleves at the Start()**\n")]
    [SerializeField] Rigidbody rb; // player
    [Header("Below compoments will manage player's movement\n")]
    public PlayerManager playerManager;
    public Transform Camera;
    PhotonView photonView;
    [SerializeField] float angularVelocity;
    [Tooltip("control payer's moving speed, speedUp speed(timer) and rush force")] public float initial_torque, torque_timer, rushForce;

    // public float torque_tim = 10f; //the timer for the torque
    // public float rushForce = 2500;
    private float torque;

    [HideInInspector] public float horizontalInput, verticalInput;



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        if (isInDebugMode)
        {

            Debug.Log(name + "in DebugMode");
        }
        else
        {
            //find current player's playerManager.cs
            playerManager = PhotonView.Find((int)photonView.InstantiationData[0]).GetComponent<PlayerManager>();
            Debug.Log("controller playerManagerID: " + (int)photonView.InstantiationData[0]);
        }


    }

    /*you should check Update() and FixedUpdate() functions for input and surround this with:
     if (PhotonView.isMine) condition or add if (!PhotonView.isMine) { return; } 
     before the input handling.*/
    void Start()
    {
        //https://doc-api.photonengine.com/en/pun/v2/class_photon_1_1_pun_1_1_photon_view.html#a67184424cffe2daae9001e06a6192d21
        //is the photon View is hadle on the local player?
        if (!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<cameraDist>().gameObject); //this make sure that the camera compoments will not mess up
        }
        Camera = transform.Find("ThirdPersonCamera/MainCamera");
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        TurningTorque();
        rush();
    }

    void FixedUpdate()
    {

    }

    void TurningTorque()
    {
        angularVelocity = rb.angularVelocity.magnitude;
        var speedUpTorque = initial_torque * torque_timer;

        //Debug.Log (angularVelocity);
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.LeftShift) )
        {
            torque = speedUpTorque;
          //  Debug.Log("speed Up");
        }
        else if (angularVelocity <= initial_torque)
        {
            torque = initial_torque;
        }
        else { torque = 0; }
        // else
        // {
        //     torque = initial_torque;
        // }

        // torque=(Input.GetKey(KeyCode.LeftShift) ? speedUpTorque:initial_torque);
        rb.AddTorque(Camera.transform.right * torque * verticalInput);
        rb.AddTorque(-Camera.transform.forward * torque * horizontalInput);
    }

    void rush()
    {
        if (Input.GetKey(KeyCode.Tab) || Input.GetMouseButton(1))
        {
            rb.angularVelocity = new Vector3(0, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
        {
            rb.AddForce(Camera.transform.forward * rushForce);

            //Debug.Log("Rush!");
        }
    }


}

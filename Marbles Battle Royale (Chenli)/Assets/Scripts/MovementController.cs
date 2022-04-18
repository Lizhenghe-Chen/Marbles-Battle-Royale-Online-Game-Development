using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
/*
* Copyright (C) 2022 Author: Lizhenghe.Chen.
* For personal study or educational use.
* Email: Lizhenghe.Chen@qq.com
*/
public class MovementController : MonoBehaviour
{

    [Tooltip("when In Locoal debug state, mark this")]
    [SerializeField] bool isInDebugMode = false;
    [Header("**Below Parameters should find by themsleves at the Start()**\n")]
    public Rigidbody rb; // player
    [Header("Below compoments will manage player's movement\n")]
    public PlayerManager playerManager;
    public Transform Camera;
    public PhotonView photonView;
    [HideInInspector] public float horizontalInput, verticalInput;
    //public float angularVelocity;

    //===============================
    [Header("**Below for players rush function,rushPane need attach menually **\n")]

    [SerializeField] TMP_Text LeftLifeText;
    [Tooltip("control payer's moving speed, speedUp speed(timer) and rush force")]
    public float initial_torque, speedUp_torque;
    private const float torque_multiplier = 3;

    //[SerializeField] float coolingTime = 2, time;//player rush cooling time
    //===============================

    private CollisionDetect collisionDetect;


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
            //    Debug.Log("controller playerManagerID: " + (int)photonView.InstantiationData[0]);
        }
    }
    /*you should check Update() and FixedUpdate() functions for input and surround this with:
     if (PhotonView.isMine) condition or add if (!PhotonView.isMine) { return; } 
     before the input handling.*/
    void Start()
    {
        //https://doc-api.photonengine.com/en/pun/v2/class_photon_1_1_pun_1_1_photon_view.html#a67184424cffe2daae9001e06a6192d21

        if (!photonView.IsMine)    //is the photon View is hadle on the local player? Very important!
        {
            Destroy(GetComponentInChildren<cameraDist>().gameObject); // destory camera this make sure that the camera compoments will not mess up
            this.enabled = false;
            return;
        }

        collisionDetect = GetComponent<CollisionDetect>();
        Camera = transform.Find("ThirdPersonCamera/MainCamera");
        LeftLifeText = transform.Find("UI/Canvas/LeftLifeText").GetComponent<TMP_Text>();
        speedUp_torque = initial_torque * torque_multiplier;
    }

    void FixedUpdate()
    {
        TurningTorque();
        Break();
    }

    void TurningTorque()
    {
        // angularVelocity = rb.angularVelocity.magnitude;
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        rb.maxAngularVelocity = (Input.GetKey(KeyCode.LeftShift) ? speedUp_torque : initial_torque);
        rb.AddTorque(Camera.transform.right * rb.maxAngularVelocity * verticalInput);
        rb.AddTorque(-Camera.transform.forward * rb.maxAngularVelocity * horizontalInput);

    }
    void Break()
    {
        if (Input.GetKey(KeyCode.Tab) || Input.GetMouseButton(1))
        {
            rb.angularVelocity = Vector3.zero;
        }
        // if (Input.GetKey(KeyCode.Space))
        // {
        //     transform.localScale = new Vector3(collisionDetect.initialScale.x, collisionDetect.initialScale.y / 2, collisionDetect.initialScale.z);
        // }
        // else { transform.localScale = collisionDetect.initialScale; }
    }

}

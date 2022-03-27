using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MovementController : MonoBehaviour
{

    [Tooltip("when In Locoal debug state, mark this")]
    [SerializeField] bool isInDebugMode = false;
    [Header("**Below Parameters should find by themsleves at the Start()**\n")]
    public Rigidbody rb; // player
    [Header("Below compoments will manage player's movement\n")]
    public PlayerManager playerManager;
    public Transform Camera;
    PhotonView photonView;
    [HideInInspector] public float horizontalInput, verticalInput;
    public float angularVelocity;

    //===============================
    [Header("**Below for players rush function,rushPane need attach menually **\n")]


    [SerializeField] TMP_Text LeftLifeText;
    [Tooltip("control payer's moving speed, speedUp speed(timer) and rush force")]
    public float initial_torque, torque_timer;

    // public float torque_tim = 10f; //the timer for the torque
    // public float rushForce = 2500;
    private float torque;

    //[SerializeField] float coolingTime = 2, time;//player rush cooling time
    //===============================
    [SerializeField] bool inHealthArea = false;
    [SerializeField] GameObject damagearea;
    [SerializeField] Vector3 damageareaPosition, playerPosition;
    [SerializeField] float damagearea_playerDistance;
    public GameObject takeDamageMask, getHealthMask;
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
        //  time = coolingTime;
        if (!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<cameraDist>().gameObject); //this make sure that the camera compoments will not mess up
        }

        collisionDetect = GetComponent<CollisionDetect>();
        Camera = transform.Find("ThirdPersonCamera/MainCamera");
        LeftLifeText = transform.Find("UI/Canvas/LeftLifeText").GetComponent<TMP_Text>();
        damagearea = GameObject.Find("DamageArea");
        takeDamageMask = transform.Find("UI/Canvas/TakeDamageMask").gameObject;
        getHealthMask = transform.Find("UI/Canvas/GetHealthMask").gameObject;
        takeDamageMask.SetActive(false);
        getHealthMask.SetActive(false);
    }

    void Update()
    {
        if (!isInDebugMode)
        {
            LeftLifeText.text = playerManager.leftLifeTextContent;
        }
        if (!photonView.IsMine)
        {
            return;
        }
        Break();

        TurningTorque();


        // playerManager.Damage(0.01f);
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        HealthEffect();
        PoisoningEffect();// low FPS will affect damgae and health speed, so move it to FixedUpdate
    }
    void OnTriggerStay(Collider collision)
    {
        if (collision.name == "HealthArea")
        {
            inHealthArea = true;
            // Debug.LogWarning("Inside Area");
        }

    }
    void OnTriggerExit(Collider collision)
    {
        if (collision.name == "HealthArea") { inHealthArea = false; Debug.LogWarning("Outside HealthArea"); }

    }
    void TurningTorque()
    {
        angularVelocity = rb.angularVelocity.magnitude;
        var speedUpTorque = initial_torque * torque_timer;

        //Debug.Log (angularVelocity);
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.LeftShift))
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
    void PoisoningEffect()
    {
        damageareaPosition = damagearea.transform.position;
        playerPosition = transform.position;
        damagearea_playerDistance = playerPosition.x - damageareaPosition.x;
        if (damagearea_playerDistance < 0)
        {
            takeDamageMask.SetActive(true);
            playerManager.Damage(PoisoningEffectMultiplier(), true, string.Empty);
            // Debug.Log(PoisoningEffectMultiplier());
        }
        else { takeDamageMask.SetActive(false); }
    }
    public float PoisoningEffectMultiplier()
    {
        for (int i = playerManager.startPoints.Count - 1; i >= 0; i--)
        {
            if (damageareaPosition.x >= playerManager.startPoints[i].position.x)
            {
                return 0.06f * (i + 1);
            }
        }
        return 0.06f;

    }
    void HealthEffect()
    {
        if (inHealthArea)
        {
            getHealthMask.SetActive(true);
            playerManager.Health(0.02f);
        }
        else
        {
            getHealthMask.SetActive(false);
        }
    }
}

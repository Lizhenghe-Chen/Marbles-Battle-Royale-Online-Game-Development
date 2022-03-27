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

    //[SerializeField] Transform Camera;
    public float sliteForce = 10f;
    public float extra_gravity = 10f;

    private MovementController movementController;

    private CollisionTrigger CollisionTrigger;
   
    public bool Grounded, OnCollisionGrounded;
    PlayerManager playerManager;

    //===============================
    public float jumpForce, rushForce;
    [Header("**Below for players Jump function,jumpPane need attach menually **\n")]
    [SerializeField] Image jumpPanel;

    [SerializeField] Image rushPanel;
    [SerializeField] float RushcoolingTime = 4, Rushtime, JumpcoolingTime = 2, JumpTime, fillValue = 0.0f;//player jump cooling time
    public Transform Camera;

    //===============================
    public AudioSource audioSource;
    public AudioClip footStep, hitSound, brakeSound, jumpSound, rushSound;
    // [SerializeField] bool isPlaying;
    public float Velocity, AngularVelocity;
    public bool playHitSound = false, playRushSound = false, playJumpSound = false;

    //============For tranning Ground Fetch===================
    [SerializeField] RoomManager roomManager;
    [SerializeField] KeepSetting keepSetting;
    public GuidanceText guidanceText;
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        rg = GetComponent<Rigidbody>(); //find the Rigidbody object
        Rushtime = RushcoolingTime;
        JumpTime = JumpcoolingTime;
        // if (!photonView.IsMine)
        // { return; }

        playerManager = GetComponent<MovementController>().playerManager;
        movementController = GetComponent<MovementController>();
        CollisionTrigger = GetComponentInChildren<CollisionTrigger>();

        jumpThreshold = GetComponent<SphereCollider>().radius + distanceBetweenGround;

        Camera = transform.Find("ThirdPersonCamera/MainCamera");
        rushPanel = transform.Find("UI/Canvas/RushLoading/rushPanel").GetComponent<Image>();
        jumpPanel = transform.Find("UI/Canvas/JumpLoading/jumpPanel").GetComponent<Image>();


        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        keepSetting = GameObject.Find("KeepSetting").GetComponent<KeepSetting>();
        if (roomManager.isTrainingGround && keepSetting.showTutorial) { guidanceText = GameObject.Find("GameInfoCanvas/Tutorial/GuidanceText").GetComponent<GuidanceText>(); }

        // Debug.Log(GetComponent<SphereCollider>().radius);
    }

    // int jumpCount = 0;


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
        PlaySounds();//all players in the game should do this, so above the photonView.IsMine

        //   isPlaying = audioSource.isPlaying;


        if (!photonView.IsMine)
        {
            return;
        }

        JumpMethod();
        RushMethod();
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
            var force = (Input.GetKey(KeyCode.LeftShift) ? sliteForce * 2 : sliteForce);
            rg.AddForce(movementController.Camera.transform.forward * force * movementController.verticalInput);
            rg.AddForce(movementController.Camera.transform.right * force * movementController.horizontalInput);
        }
    }



    public bool onTheGround()//a combination of tuch and raycast
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
        JumpTime += Time.deltaTime;
        fillValue = JumpTime / JumpcoolingTime;
        jumpPanel.fillAmount = fillValue;
        if (JumpTime >= JumpcoolingTime)
        {
            JumpTime = JumpcoolingTime;
            JumpCommand();
        }
    }
    void JumpCommand()
    {
        if (Input.GetKeyUp(KeyCode.Space) && onTheGround())
        {
            photonView.RPC("globalSoundTrigger", RpcTarget.All, "jump");

            rg.AddForce(Vector3.up * jumpForce);
            JumpTime = 0;

            if (roomManager.isTrainingGround && keepSetting.showTutorial)
            {
                if (guidanceText.Goal == 3) { guidanceText.jumpRushMission += 1; }
            }
        }
    }
    void RushMethod()
    {
        Rushtime += Time.deltaTime;
        fillValue = Rushtime / RushcoolingTime;
        rushPanel.fillAmount = fillValue;
        if (Rushtime >= RushcoolingTime)
        {
            Rushtime = RushcoolingTime;
            RushCommand();
        }
    }
    void RushCommand()
    {
        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
        {
            photonView.RPC("globalSoundTrigger", RpcTarget.All, "rush");

            rg.AddForce(Camera.transform.forward * rushForce);
            Rushtime = 0;

            if (roomManager.isTrainingGround && keepSetting.showTutorial)
            {
                if (guidanceText.Goal == 3) { guidanceText.jumpRushMission += 1; }
            }
        }
    }
    public float hitForce;
    public void PlaySounds()
    {

        Velocity = rg.velocity.magnitude;
        AngularVelocity = rg.angularVelocity.magnitude;
        // var tempV = (float)System.Math.Tanh(Velocity);//between 0 to 1:https://en.wikipedia.org/wiki/Activation_function
        //double.Parse(System.Math.Round(Velocity, 3).ToString());

        if (playHitSound)
        {
            //Debug.Log("HitSound");
            audioSource.clip = hitSound;
            audioSource.pitch = 1f;
            if (hitForce < 10) { audioSource.volume = 0.5f; } else { audioSource.volume = 1f; }
            audioSource.Play();

            playHitSound = false;
        }
        if (playJumpSound) { PlayClip(jumpSound, 2f, 1f); playJumpSound = false; }
        if (playRushSound) { PlayClip(rushSound, 2f, 1f); playRushSound = false; }
        if (!audioSource.isPlaying)
        {
            if (onTheGround() && AngularVelocity > 1)//below play footstep
            {
                // Debug.Log("Walking");
                audioSource.clip = footStep;

                if (1f < AngularVelocity && AngularVelocity < 5f)
                {
                    audioSource.pitch = 0.5f;
                    audioSource.volume = 0.3f;
                }
                else if (3f <= AngularVelocity && AngularVelocity < 10f)
                {
                    audioSource.pitch = 1.5f;
                    audioSource.volume = 0.6f;
                }
                else if (10f <= AngularVelocity && AngularVelocity < 15f)
                {
                    audioSource.pitch = 2f;
                    audioSource.volume = 0.8f;
                }
                else if (AngularVelocity > 15f)
                {
                    audioSource.pitch = 3f;
                    audioSource.volume = 1f;
                }

                audioSource.Play();
            }
            if (onTheGround() && Velocity > 5 && (Input.GetKey(KeyCode.Tab) || Input.GetMouseButton(1)))
            {
                PlayClip(brakeSound, 3f, 0.5f);
            }

            // else if (onTheGround() && Velocity > 3 && AngularVelocity < 0.1)
            // {
            //     audioSource.pitch = 3f;
            //     audioSource.volume = 1f;

            //     audioSource.Play();
            // }
        }


        // else
        // {
        //     if (audioSource.isPlaying)
        //     {
        //         audioSource.clip = null;
        //         //if player does not touch the ground
        //         audioSource.Pause();
        //     }
        // }
    }

    public void PlayClip(AudioClip audioClip, float pitch, float volume)
    {
        audioSource.clip = audioClip;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.Play();
    }
    [PunRPC]
    public void globalSoundTrigger(string type)
    {
        if (type == "jump") { playJumpSound = true; }
        if (type == "rush") { playRushSound = true; }
        // Debug.Log("globalSoundTrigger : " + trigger);
        // playJumpSound = true;
        // Debug.Log("globalSoundTrigger set " + trigger);
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

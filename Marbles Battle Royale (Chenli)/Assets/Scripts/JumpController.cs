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
    public bool combinedGrounded, OnCollisionGrounded;
    PlayerManager playerManager;
    //===============================
    public float jumpForce, rushForce;
    [Header("**Below for players Jump function,jumpPane need attach menually **\n")]
    [SerializeField] Image jumpPanel;
    [SerializeField] Image rushPanel;
    public float RushcoolingTime = 4, Rushtime, JumpcoolingTime = 2, JumpTime, fillValue = 0.0f;//player jump cooling time
    public Transform Camera;
    //===============================
    public AudioSource audioSource;
    public AudioClip footStep, grounding, hitSound, brakeSound, jumpSound, rushSound;
    public float Velocity, AngularVelocity;
    // public bool playHitSound = false, playRushSound = false, playJumpSound = false;
    //============For tranning Ground Fetch===================
    [SerializeField] RoomManager roomManager;
    [SerializeField] KeepSetting keepSetting;
    public GuidanceText guidanceText;
    public bool isMenuOpen;
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rg = GetComponent<Rigidbody>(); //find the Rigidbody object
        if (!photonView.IsMine) { return; }
        Rushtime = RushcoolingTime;
        JumpTime = JumpcoolingTime;

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
        if (!photonView.IsMine)
        {
            return;
        }
        JumpMethod();
        RushMethod();
    }

    void OnCollisionStay(Collision col)
    {
        OnCollisionGrounded = true;
    }
    void OnCollisionExit(Collision col)
    {
        OnCollisionGrounded = false;
    }
    void GiveLittileForce()
    {
        if (!OnCollisionGrounded)
        {
            //  Debug.Log("sliteForce");
            var force = (Input.GetKey(KeyCode.LeftShift) ? sliteForce * 2 : sliteForce);
            rg.AddForce(movementController.Camera.transform.forward * force * movementController.verticalInput);
            rg.AddForce(movementController.Camera.transform.right * force * movementController.horizontalInput);
        }
    }

    public bool onTheGround()//a combination of touch and raycast
    {
        bool isRayGrounded = Physics.Raycast(transform.position, Vector3.down, jumpThreshold);
        Color rayColor;
        if (isRayGrounded ||( !isRayGrounded && OnCollisionGrounded))
        {
            combinedGrounded = true;
            rayColor = Color.green;
            //Debug.Log("i'm grounded");
        }
        else
        {
            combinedGrounded = false;
            rayColor = Color.red;
            //Debug.Log("not grounded");
        }
        Debug.DrawRay(transform.position, Vector3.down, rayColor, jumpThreshold);
        return combinedGrounded;
    }
    void JumpMethod()//put this method in FixedUpdate to accumulate time
    {

        if (JumpTime >= JumpcoolingTime)
        {
            JumpCommand();//allow the user to jump
            return;
        }
        JumpTime += Time.deltaTime;
        //fill the jump loading bar for user to see:
        fillValue = JumpTime / JumpcoolingTime;
        jumpPanel.fillAmount = fillValue;
    }
    void JumpCommand()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!onTheGround()) { return; }
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
        if (Rushtime >= RushcoolingTime)
        {
            if (isMenuOpen) { return; }
            RushCommand();
            return;
        }
        Rushtime += Time.deltaTime;
        fillValue = Rushtime / RushcoolingTime;
        rushPanel.fillAmount = fillValue;
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
    public void PlayHitSound()
    {
        //Debug.Log("HitSound");
        if (hitForce < 10) { PlayClip(hitSound, 1f, 0.5f); } else { PlayClip(hitSound, 1f, 1f); }
    }
    public void PlayGroundedSound() { if (!audioSource.isPlaying) { PlayClip(grounding, 1f, 0.5f); } }
    public void PlayJumpSound() { PlayClip(jumpSound, 2f, 1f); }
    public void PlayRushSound() { PlayClip(rushSound, 2f, 1f); }
    public void PlaySounds()
    {
        Velocity = rg.velocity.magnitude;
        AngularVelocity = rg.angularVelocity.magnitude;
        // var tempV = (float)System.Math.Tanh(Velocity);//between 0 to 1:https://en.wikipedia.org/wiki/Activation_function
        //double.Parse(System.Math.Round(Velocity, 3).ToString());

        // if (playHitSound)
        // {
        //     //Debug.Log("HitSound");
        //     audioSource.clip = hitSound;
        //     audioSource.pitch = 1f;
        //     if (hitForce < 10) { audioSource.volume = 0.5f; } else { audioSource.volume = 1f; }
        //     audioSource.Play();

        //     playHitSound = false;
        // }
        // if (playJumpSound) { PlayClip(jumpSound, 2f, 1f); playJumpSound = false; }
        // if (playRushSound) { PlayClip(rushSound, 2f, 1f); playRushSound = false; }
        if (!audioSource.isPlaying)
        {
            if (OnCollisionGrounded && AngularVelocity > 1)//below play footstep
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
            if (OnCollisionGrounded && Velocity > 5 && (Input.GetKey(KeyCode.Tab) || Input.GetMouseButton(1)))
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
        if (type == "jump") { PlayJumpSound(); }
        if (type == "rush") { PlayRushSound(); }
        // Debug.Log("globalSoundTrigger : " + trigger);
        // playJumpSound = true;
        // Debug.Log("globalSoundTrigger set " + trigger);
    }

}

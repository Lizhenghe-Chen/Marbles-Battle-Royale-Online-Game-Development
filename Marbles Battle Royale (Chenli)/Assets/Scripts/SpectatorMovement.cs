using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;
public class SpectatorMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rb; // player
    [SerializeField] Camera cam;
    [SerializeField][Range(1f, 30f)] float initial_speed = 5f;
    private const float speedUpMultiplier = 5;

    // Start is called before the first frame update
    [HideInInspector] public float horizontalInput, verticalInput;

    [SerializeField] PhotonView photonView;
    [SerializeField] GameInfoManager GameInfoManager;
    [SerializeField] Image FadeIn_OutImage;
    [SerializeField] Canvas canvas;
    [SerializeField] PostProcessVolume postProcessVolume;
    [SerializeField] bool menueOpen = false;
    [SerializeField] string[] ignoreObject = { "Untagged", "Player", "Robot" };
    public DepthOfField df;

    public Slider focusDistanceSlider, aptureSlider, focalLengthSlider;
    [SerializeField] GameObject CinemachineCameraCtrl;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(gameObject); //this make sure that the camera compoments will not mess up
            return;
        }
        canvas = transform.Find("Canvas").GetComponent<Canvas>();
        rb = GetComponent<Rigidbody>();
        FadeIn_OutImage.GetComponent<AnimateLoading>().LoadingLevel();
        postProcessVolume = GameObject.Find("Post_Process Volum").GetComponent<PostProcessVolume>();
        GameInfoManager = GameObject.Find("GameInfoCanvas/GameInfo").GetComponent<GameInfoManager>();
        GameInfoManager.scoreBoardManager = transform.Find("Canvas/ScoreBoard").GetComponent<ScoreBoardManager>();

        postProcessVolume.sharedProfile.TryGetSettings<DepthOfField>(out df);

        df.focusDistance.value = focusDistanceSlider.value;
        df.aperture.value = aptureSlider.value;
        df.focalLength.value = focalLengthSlider.value;

        // aptureSlider = canvas.transform.Find("AptureSlider").GetComponent<Slider>();
        // focalLengthSlider = canvas.transform.Find("FocalLengthSlider").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        Menu_Cursor();
        MovingCommand();
        UpdatePS();

    }
    void UpDownCommand()
    {
        //Debug.Log(CollisionTrigger.onTheGround);
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E))
        {
            rb.AddForce(Vector3.up * initial_speed);

            //  Debug.Log("Space pressed and UP");
        }
        if (Input.GetKey(KeyCode.Q))
        {
            rb.AddForce(-Vector3.up * initial_speed);

            //   Debug.Log(a + "Space pressed and jump" + jumpCount);
        }
    }
    void MovingCommand()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.Period) && initial_speed < 30f) { initial_speed += 1f; }
        if (Input.GetKey(KeyCode.Comma) && initial_speed > 1f) { initial_speed -= 1f; }

        if (Input.GetKey(KeyCode.Tab) || Input.GetMouseButton(1))
        {
            rb.velocity = Vector3.zero;
            return;
        }
        var speed = (Input.GetKey(KeyCode.LeftShift)) ? speedUpMultiplier * initial_speed : initial_speed;

        rb.AddForce(cam.transform.forward * speed * verticalInput);
        rb.AddForce(cam.transform.right * speed * horizontalInput);
        UpDownCommand();
    }
    public void LeaveRoom()
    {
        Destroy(GameObject.Find("RoomManager").gameObject);
        //PhotonNetwork.Disconnect();
        // PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LoadLevel(0);
        PhotonNetwork.LeaveRoom();
        //if (PhotonNetwork.CurrentRoom != null) { PhotonNetwork.Disconnect(); }
        SceneManager.LoadScene(0); //Level 0 is the start menu, Level 1 is the Gaming Scene
        Debug.Log("Leaved Room");
    }
    void Menu_Cursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { menueOpen = !menueOpen; }
        CinemachineCameraCtrl.GetComponent<CinemachineVirtualCamera>().enabled = !menueOpen;
        canvas.enabled = menueOpen;
        Cursor.visible = menueOpen;
        Cursor.lockState = (menueOpen) ? CursorLockMode.None : CursorLockMode.Locked;
    }
    void UpdatePS()//post-processing
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //  Debug.Log(" df changed");
            df.focusDistance.value += 0.1f;
            focusDistanceSlider.value = df.focusDistance.value;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            df.focusDistance.value -= 0.1f;
            focusDistanceSlider.value = df.focusDistance.value;
        }

    }
    void OnCollisionEnter(Collision collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "CameraIgnore")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }
    public void SetFocusDistance()
    {
        df.focusDistance.value = focusDistanceSlider.value;
    }
    public void SetFocalLength()
    {
        df.focalLength.value = focalLengthSlider.value;
    }
    public void SetApture()
    {
        df.aperture.value = aptureSlider.value;
    }
}

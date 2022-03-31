using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SpectatorMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rb; // player
    [SerializeField] Camera cam;
    [SerializeField] float initial_speed = 5f;
    [SerializeField] float speed = 5f;
    // Start is called before the first frame update
    [HideInInspector] public float horizontalInput, verticalInput;

    [SerializeField] PhotonView photonView;
    [SerializeField] GameInfoManager GameInfoManager;
    [SerializeField] Image FadeIn_OutImage;

    [SerializeField] bool menueOpen = false;
     [SerializeField] string[] ignoreObject = {"Untagged","Player","Robot"};
    void Start()
    {

        FadeIn_OutImage.GetComponent<AnimateLoading>().LoadingLevel();
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            Destroy(gameObject); //this make sure that the camera compoments will not mess up
            return;
        }
        GameInfoManager = GameObject.Find("GameInfoCanvas/GameInfo").GetComponent<GameInfoManager>();
        GameInfoManager.scoreBoardManager = transform.Find("Canvas/ScoreBoard").GetComponent<ScoreBoardManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Menu_Cursor();
        MovingCommand();


    }
    void UpDownCommand()
    {
        //Debug.Log(CollisionTrigger.onTheGround);
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E))
        {
            rb.AddForce(Vector3.up * speed);

            //  Debug.Log("Space pressed and UP");
        }
        if (Input.GetKey(KeyCode.Q))
        {
            rb.AddForce(-Vector3.up * speed);

            //   Debug.Log(a + "Space pressed and jump" + jumpCount);
        }
    }
    void MovingCommand()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && initial_speed <= 30f) { initial_speed += 1f; }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && initial_speed >= 3f) { initial_speed -= 1f; }

        if (Input.GetKey(KeyCode.Tab) || Input.GetMouseButton(1))
        {
            rb.velocity = Vector3.zero;
            return;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = initial_speed * 5;
        }
        else speed = initial_speed;

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
        //  Debug.Log(counter);

        if (!menueOpen)
        {
            // InGameUI.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // InGameUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer)=="CameraIgnore" )
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }
}

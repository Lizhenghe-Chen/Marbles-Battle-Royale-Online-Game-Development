using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpectatorMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rb; // player
    [SerializeField] Camera cam;
    [SerializeField] float initial_speed = 5f;
    [SerializeField] float speed = 5f;
    // Start is called before the first frame update
    [HideInInspector] public float horizontalInput, verticalInput;

    [SerializeField] PhotonView photonView;
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            Destroy(gameObject); //this make sure that the camera compoments will not mess up
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpDownCommand();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = initial_speed * 5;
        }
        else speed = initial_speed;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && speed <= 30f) { initial_speed += 1f; }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && speed >= 3f) { initial_speed -= 1f; }
        rb.AddForce(cam.transform.forward * speed * verticalInput);
        rb.AddForce(cam.transform.right * speed * horizontalInput);
    }
    void UpDownCommand()
    {
        //Debug.Log(CollisionTrigger.onTheGround);
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E))
        {
            rb.AddForce(Vector3.up * speed);

            Debug.Log("Space pressed and UP");
        }
        if (Input.GetKey(KeyCode.Q))
        {
            rb.AddForce(-Vector3.up * speed);

            //   Debug.Log(a + "Space pressed and jump" + jumpCount);
        }
    }
}

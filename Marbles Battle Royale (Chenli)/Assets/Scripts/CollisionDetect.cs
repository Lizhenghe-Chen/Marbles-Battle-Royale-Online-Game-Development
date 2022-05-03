using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CollisionDetect : MonoBehaviour
{
    [Header("This compoment will manage player's collision and damages\n")]

    PhotonView photonView;
    //public TMP_Text testText;
    //================================================================
    //[Header("Below one Parameters need Manually attach\n")]



    [Header("UI need Manually attach, this is for player's UI display,\n this will make sure player only get their own UI panel")]

    //[SerializeField] GameInfoManager GameInfoManager;
    [Tooltip("This is for player's Health display, should be an image")]
    //[SerializeField] Image healthBarImage;

    [SerializeField] Rigidbody rb; // player
    public string player_Name, other_Player_Name;

    // double hitDirection;
    [SerializeField] float currentHealth;

    Vector3 other_Player_Velocity;
    public Vector3 Player_Velocity;
    public bool inHealthArea = false;
    public float hitForce;
    //================================================================
    //private CollisionTrigger CollisionTrigger;
    private static float X_velocity, Y_velocity, Z_velocity, TotalSpeed;
    PlayerManager playerManager;
    MovementController movementController;
    // Start is called before the first frame update
    JumpController jumpController;
    public Vector3 initialScale;
    public float maxScale = 4, minScale = 0.2f;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Debug.Log(UI.transform.Find("HealthbarBackground/Healthbar"));
        photonView = GetComponent<PhotonView>();
        player_Name = photonView.Owner.NickName;
        jumpController = GetComponent<JumpController>();
        //   GameInfoManager = GameObject.Find("GameInfoCanvas/GameInfo").GetComponent<GameInfoManager>();
        //  GameInfoManager.scoreBoardManager = transform.Find("UI/ScoreBoard").GetComponent<ScoreBoardManager>();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            initialScale = transform.localScale;
            movementController = GetComponent<MovementController>();
            playerManager = movementController.playerManager;
            playerManager.initialScale = initialScale;
            //CollisionTrigger = GetComponentInChildren<CollisionTrigger>();
        }
        else
        {
            //Destroy(healthBarImage);
        }
    }


    void FixedUpdate()
    {
 Player_Velocity = rb.velocity;//since the collision will get the late velocity that give wrong damage, so we need to get the velocity in fixed update (earlier)
        if (!photonView.IsMine)
        {
            return;
        }
        currentHealth = playerManager.currentHealth;
        // billboardvalue = playerManager.currentHealth / playerHealth;// this will display user's current health for all players, check Billboard.cs
        //  Billboard.DisplayHealthBar(healthBarImage, playerManager.billboardvalue);

        if (transform.localScale.x > initialScale.x)
        {
            transform.localScale -= new Vector3(0.001f, 0.001f, 0.001f);
        }
        if (transform.localScale.x < initialScale.x)
        {
            transform.localScale += new Vector3(0.0005f, 0.0005f, 0.0005f);
        }
        // healthBarImage.fillAmount = billboardvalue;
    }
  
    // void OnCollisionStay(Collision collision)
    // {
    //     if (collision.collider.name == "funnel")
    //     {
    //         rb.drag = 0;
    //         //Debug.Log(collision.collider.name);
    //     }
    //     else { rb.drag = 0.1f; }

    // }
    //https://docs.unity3d.com/Manual/ExecutionOrder.html
    void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        jumpController.PlayGroundedSound();//let sound effect happen once locally
        // Debug.Log(target.photonView.Owner.NickName);

        if (collision.rigidbody && (collision.collider.tag == "Player" || collision.collider.tag == "Robot"))
        {

            jumpController.PlayHitSound();//let sound effect happen once locally
            if (collision.collider.tag == "Player")
            {
                other_Player_Name = collision.collider.GetComponent<PhotonView>().Owner.NickName;
                other_Player_Velocity = collision.gameObject.GetComponent<CollisionDetect>().Player_Velocity;
            }
            else if (collision.collider.tag == "Robot")
            {
                other_Player_Name = collision.collider.tag;
                other_Player_Velocity = collision.gameObject.GetComponent<RobotController>().robotVelocity;
               // Debug.Log("relativeVelocity: " + collision.relativeVelocity.magnitude + "other_Player_Velocity" + other_Player_Velocity.magnitude + "rb.velocity" + rb.velocity.magnitude);
                // Debug.Log("@@@" + collision.collider.GetComponent<Rigidbody>().velocity.magnitude + "@@@" + collision.body.GetComponent<Rigidbody>().velocity.magnitude);
            }

            // hitDirection = System.Math.Round(Vector3.Dot(Player_Velocity, other_Player_Velocity), 3);
            // judgeDamage(collision, Player_Velocity, other_Player_Velocity, hitDirection);
            if (Player_Velocity.magnitude < other_Player_Velocity.magnitude)//if player get damage
            {
                hitForce = collision.relativeVelocity.magnitude * collision.collider.GetComponent<Rigidbody>().mass;//let different type of ball have different damage

                jumpController.hitForce = hitForce;//for sound effect use
                var finalDamage = hitForce * playerManager.damageTimer;//

                // movementController.takeDamageMask.enabled = true;
                var hitessage = player_Name + " got damage from " + other_Player_Name + " with value:" + finalDamage;
                Debug.Log(hitessage);
                if (currentHealth - finalDamage <= 0)//after take damage, judge dead immediantly
                {
                    if (collision.collider.tag == "Player")//if killer is a player
                    {
                        PlayerManager otherPlayerManager = collision.collider.GetComponent<MovementController>().playerManager;
                        otherPlayerManager.Kill(player_Name);
                        // GameInfoManager.Refresh(other_Player_Name + " Killed " + player_Name);
                    }
                    // else { GameInfoManager.Refresh(other_Player_Name + " X -> " + player_Name); }
                }

                //photonView.RPC("Damage", RpcTarget.All, finalDamage, hitessage);//send damage to all players， this function will change  'currentHealth' value
                playerManager.TakeDamage(finalDamage, hitessage, other_Player_Name);
                //  billboardvalue = currentHealth / playerHealth;// this will display user's current health
            }

            // Vector3 PlayerPosition = this.transform.position;
            // Vector3 TesterPosition = collision.transform.position;
            // Vector3 direction = (TesterPosition - PlayerPosition);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Transfer platform")
        {
            //This will make the player a child of the Obstacle
            transform.parent = other.gameObject.transform.parent; //Change "myPlayer" to your player
            Debug.Log("changed Parent to " + other.gameObject.name);
        }
        if (other.gameObject.name == "elevator")
        {
            other.gameObject.GetComponent<TouchUp>().enabled = true;
        }
        if (other.name == "HealthArea")
        {
            inHealthArea = true;
            // Debug.LogWarning("Inside Area");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Transfer platform")
        {
            this.transform.parent = null;
            rb.velocity += other.gameObject.GetComponent<getspeed>().ObjVelocity;
            //other.gameObject.GetComponent<getspeed>().enabled = false;
        }
        
        if (other.name == "HealthArea") { inHealthArea = false; Debug.LogWarning("Outside HealthArea"); }
    }

    public float[] getSpeedData(Rigidbody rb)
    {
        X_velocity = rb.velocity.x;
        Y_velocity = rb.velocity.y;
        Z_velocity = rb.velocity.z;
        Player_Velocity = rb.velocity;
        TotalSpeed = Mathf.Sqrt(Mathf.Pow(X_velocity, 2) +
                Mathf.Pow(Y_velocity, 2) + Mathf.Pow(Z_velocity, 2));
        float[] data = { X_velocity, Y_velocity, Z_velocity, TotalSpeed };
        return data;
    }

    //================================================================

    public static bool judgeDamage(Vector3 Player_Velocity, Vector3 other_Player_Velocity)
    {
        return (Player_Velocity.magnitude > other_Player_Velocity.magnitude);
    }
}

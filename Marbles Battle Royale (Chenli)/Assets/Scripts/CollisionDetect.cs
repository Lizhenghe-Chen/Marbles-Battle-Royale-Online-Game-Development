using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CollisionDetect : MonoBehaviour
{
    [Header("This compoment will manage player's collision and damages\n")]


    public static bool ISufferDamage = false;

    PhotonView photonView;
    //public TMP_Text testText;
    //================================================================
    //[Header("Below one Parameters need Manually attach\n")]



    [Header("UI need Manually attach, this is for player's UI display,\n this will make sure player only get their own UI panel")]
 
    [SerializeField] GameInfoManager GameInfoManager;
    [Tooltip("This is for player's Health display, should be an image")]
    //[SerializeField] Image healthBarImage;
    [SerializeField] double deathAltitude;

    [SerializeField] Rigidbody rb; // player
    public string player_Name, other_Player_Name;

    double hitDirection;
    [SerializeField] float currentHealth;
    float damageTimer;
    Vector3 other_Player_Velocity;
    public Vector3 Player_Velocity;
    public float hitForce;
    //================================================================
    //private CollisionTrigger CollisionTrigger;
    private static float

            X_velocity,
            Y_velocity,
            Z_velocity,
            TotalSpeed;
    PlayerManager playerManager;
    MovementController movementController;
    // Start is called before the first frame update


    JumpController jumpController;
    public Vector3 initialScale;
    public float maxScale = 4, minScale = 0.2f;
    void Awake()
    {
       
        GameInfoManager = GameObject.Find("GameInfoCanvas/GameInfoTitle").GetComponent<GameInfoManager>();
        rb = GetComponent<Rigidbody>();
      
        // Debug.Log(UI.transform.Find("HealthbarBackground/Healthbar"));
        photonView = GetComponent<PhotonView>();
        player_Name = photonView.Owner.NickName;

        jumpController = GetComponent<JumpController>();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            initialScale = transform.localScale;
            movementController = GetComponent<MovementController>();
            playerManager = movementController.playerManager;
            playerManager.initialScale = initialScale;
            damageTimer = playerManager.damageTimer;
            deathAltitude = playerManager.deathAltitude;
            // healthBarImage = transform.Find("Canvas/HealthbarBackground/Healthbar").GetComponent<Image>();
            // Debug.Log(healthBarImage);
            // UI = GameObject.Find("Canvas");

            //Debug.Log("collition playerManager: " + playerManager);

            //CollisionTrigger = GetComponentInChildren<CollisionTrigger>();
        }
        else
        {
            
            //Destroy(healthBarImage);
        }


    }

    // Update is called once per frame
    void LateUpdate()
    {

    }
    void FixedUpdate()
    {
        Player_Velocity = rb.velocity;


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
            transform.localScale += new Vector3(0.001f, 0.001f, 0.001f);
        }



        // healthBarImage.fillAmount = billboardvalue;
    }
    void Update()
    {

        if (!photonView.IsMine)
        {
            return;
        }
        // if (playerManager.currentHealth <= 0) { FadeIn_OutImage.GetComponent<AnimateLoading>().LeavingLevel(); }
        BelowDeathAltitude();
    }

    void OnCollisionStay(Collision collision)
    {
        //  if (collision.collider.name == "Enlarge" && transform.localScale.x <= initialScale.x * maxScale) { transform.localScale += new Vector3(0.1f, 0.1f, 0.1f); }
        //  if (collision.collider.name == "Shrink" && transform.localScale.x >= initialScale.x * minScale) { transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f); }

        if (collision.collider.name == "funnel")
        {
            rb.drag = 0;
            //Debug.Log(collision.collider.name);
        }
        else { rb.drag = 0.1f; }

    }
    //https://docs.unity3d.com/Manual/ExecutionOrder.html
    void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        // Debug.Log(target.photonView.Owner.NickName);

        if (collision.rigidbody && (collision.collider.tag == "Player" || collision.collider.tag == "Robot"))
        {

            if (collision.collider.tag == "Player")
            {
                other_Player_Name = collision.collider.GetComponent<PhotonView>().Owner.NickName;
                other_Player_Velocity = collision.gameObject.GetComponent<CollisionDetect>().Player_Velocity;
            }
            else if (collision.collider.tag == "Robot")
            {
                other_Player_Name = collision.collider.tag;
                other_Player_Velocity = collision.gameObject.GetComponent<RobotController>().robotVelocity;
            }

            hitDirection = System.Math.Round(Vector3.Dot(Player_Velocity, other_Player_Velocity), 3);

            jumpController.playHitSound = true;//let sound effect happen once locally
            ISufferDamage = judgeDamage(Player_Velocity, other_Player_Velocity);//for debug use
            // judgeDamage(collision, Player_Velocity, other_Player_Velocity, hitDirection);
            if (ISufferDamage)//if player get damage
            {
                hitForce = collision.relativeVelocity.magnitude * 1.5f * collision.collider.GetComponent<Rigidbody>().mass;//let different type of ball have different damage


                jumpController.hitForce = hitForce;//for sound effect use
                var finalDamage = hitForce * damageTimer;//

                // movementController.takeDamageMask.enabled = true;
                var hitessage = player_Name + " got damage from " + other_Player_Name + " with value:" + finalDamage;
                Debug.Log(hitessage);
                if (currentHealth - finalDamage <= 0)//after take damage, judge dead immediantly
                {
                    if (collision.collider.GetComponent<MovementController>() != null)//if killer is a player
                    {
                        PlayerManager otherPlayerManager = collision.collider.GetComponent<MovementController>().playerManager;
                        otherPlayerManager.Kill(player_Name);
                        // GameInfoManager.Refresh(other_Player_Name + " Killed " + player_Name);
                    }
                    else { GameInfoManager.Refresh(other_Player_Name + " X -> " + player_Name); }

                }

                //photonView.RPC("Damage", RpcTarget.All, finalDamage, hitessage);//send damage to all players， this function will change  'currentHealth' value
                playerManager.TakeDamage(finalDamage, hitessage, other_Player_Name);
                //  billboardvalue = currentHealth / playerHealth;// this will display user's current health


            }
            // else
            // {//This is a not proper method because the death is judged locally, Howeversome time the otherplayer actually don't get hitted due to the network issue
            //     if (finalDamage >= collision.collider.GetComponent<MovementController>().playerManager.currentHealth)
            //     {
            //         playerManager.killCount++;
            //     }
            // }

            // judgeDamage (Player_Velocity, other_Player_Velocity, hitDirection);
            // Vector3 otherV =
            //     GameObject.Find("Sphere").GetComponent<cam>().Player_Velocity;
            // Vector3 PlayerPosition = this.transform.position;
            // Vector3 TesterPosition = collision.transform.position;
            // Vector3 direction = (TesterPosition - PlayerPosition);
        }

        // movementController.takeDamageMask.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Transfer platform")
        {
            //This will make the player a child of the Obstacle
            transform.parent = other.gameObject.transform.parent; //Change "myPlayer" to your player

            //collision.gameObject.GetComponent<getspeed>().enabled = true;
            Debug.Log("changed Parent to " + other.gameObject.name);
        }
    }
    // [PunRPC]
    // void Damage(float finalDamage, string message)
    // {
    //     currentHealth -= finalDamage;

    //     Debug.Log("**************** " + message + " ****************");

    // }
    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.name == "Transfer platform")
        {
            this.transform.parent = null;
            rb.velocity += collision.gameObject.GetComponent<getspeed>().ObjVelocity;
            //collision.gameObject.GetComponent<getspeed>().enabled = false;
        }
    }

    public float[] getSpeedData(Rigidbody rb)
    {
        X_velocity = rb.velocity.x;
        Y_velocity = rb.velocity.y;
        Z_velocity = rb.velocity.z;
        Player_Velocity = rb.velocity;
        TotalSpeed =
            Mathf
                .Sqrt(Mathf.Pow(X_velocity, 2) +
                Mathf.Pow(Y_velocity, 2) +
                Mathf.Pow(Z_velocity, 2));
        float[] data = { X_velocity, Y_velocity, Z_velocity, TotalSpeed };
        return data;
    }


    //================================================================

    // public void TakeDamage(float damage)//Send
    // {
    //     photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    // }
    // [PunRPC]
    // void RPC_TakeDamage(float damage) //Recieve this method will run on everyone's computer, but !photonView.IsMine will make it only run on the victum's computer
    // {
    //     if (!photonView.IsMine)
    //     {
    //         return;
    //     }
    //     Debug.Log("ohhh took damage: " + damage);
    // }
    public static bool judgeDamage(Vector3 Player_Velocity, Vector3 other_Player_Velocity)
    {
        if (Player_Velocity.magnitude > other_Player_Velocity.magnitude) { return false; } else { return true; }
    }
    // public void judgeDamage(Collision collision, Vector3 Player_Velocity, Vector3 other_Player_Velocity, double hitDirection)
    // {

    //     // var damage = Mathf.Abs(Player_Velocity.magnitude - other_Player_Velocity.magnitude);

    //     //++++++++++++++++
    //     if (
    //         hitDirection >= 0.05 //same direction
    //     )
    //     {
    //         if (
    //             Player_Velocity.magnitude > other_Player_Velocity.magnitude //player speed is higher
    //         )
    //         {
    //             // damage = Mathf.Abs(Player_Velocity.magnitude - other_Player_Velocity.magnitude);
    //             // Debug.Log(name + " same direction hit " + other_Player_Name + " with differ Force:" + damage);
    //             ISufferDamage = false;

    //         }
    //         else if (
    //             Player_Velocity.magnitude < other_Player_Velocity.magnitude //player speed is lower
    //         )
    //         {
    //             // Debug.Log(name + " hitted by " + other_Player_Name + " with differ Force:" + damage);
    //             ISufferDamage = true;
    //         }
    //         //return damage;

    //     }
    //     else if (
    //         hitDirection <= 0.05 && hitDirection >= -0.05 //between +- 0.05 means someone is motionless
    //     )
    //     {
    //         if (
    //             Player_Velocity.magnitude > other_Player_Velocity.magnitude //player speed is higher
    //         )
    //         {
    //             //Debug.Log(name + " hit " + other_Player_Name + " with Force:" + Player_Velocity.magnitude);
    //             ISufferDamage = false;
    //             //return Player_Velocity.magnitude;

    //         }
    //         else if (
    //             Player_Velocity.magnitude < other_Player_Velocity.magnitude //player speed is lower
    //         )
    //         {
    //             //Debug.Log(name + " hitted by " + other_Player_Name + " with Force:" + other_Player_Velocity.magnitude);
    //             ISufferDamage = true;
    //             // return other_Player_Velocity.magnitude;
    //         }
    //         //return 666666;

    //     }
    //     else
    //     {
    //         //<0 means opposite direction
    //         if (
    //             Player_Velocity.magnitude > other_Player_Velocity.magnitude //player speed is higher
    //         )
    //         {
    //             // Debug.Log(name + " hit " + other_Player_Name + " with differ Force:" + damage);
    //             ISufferDamage = false;

    //         }
    //         else if (
    //             Player_Velocity.magnitude < other_Player_Velocity.magnitude //player speed is lower
    //         )
    //         {
    //             // Debug.Log(name + " hitted by " + other_Player_Name + " with differ Force:" + damage);
    //             ISufferDamage = true;
    //         }
    //         //return damage;

    //     }

    // }
    public void BelowDeathAltitude()
    {
        if (transform.localPosition.y < deathAltitude)
        {
            playerManager.deadPosition = transform.position;//send death position to it's player Manager
                                                            //   FadeIn_OutImage.GetComponent<AnimateLoading>().LeavingLevel();
            GameInfoManager.Refresh(player_Name + " Drop dead");
            playerManager.Die();
            // gameObject.transform.position =
            //     new Vector3(target.x + Random.Range(3, 10),
            //         100,
            //         target.z + Random.Range(3, 10));
        }
    }
    // void Die()
    // {
    //     if (!playerManager)
    //     {
    //         Debug.LogWarning("No playerManager");
    //         return;
    //     }
    //     else
    //         playerManager.Die();
    // }


}

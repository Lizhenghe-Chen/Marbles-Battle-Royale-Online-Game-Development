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
    [SerializeField] GameObject UI;
    [Tooltip("This is for player's Health display, should be an image")]
    [SerializeField] Image healthBarImage;
    [SerializeField] const double deathAltitude = -40f;

    [SerializeField] Rigidbody rb; // player
    public string other_Player_Name;

    double hitDirection;
    [SerializeField] float currentHealth;
    [SerializeField] float damageTimer = 1f;
    Vector3 other_Player_Velocity;
    [SerializeField] Vector3 Player_Velocity;
    float hitForce;
    //================================================================
    //private CollisionTrigger CollisionTrigger;
    private static float

            X_velocity,
            Y_velocity,
            Z_velocity,
            TotalSpeed;
    PlayerManager playerManager;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        healthBarImage = UI.transform.Find("HealthbarBackground/Healthbar").GetComponent<Image>();
        // Debug.Log(UI.transform.Find("HealthbarBackground/Healthbar"));
        photonView = GetComponent<PhotonView>();

    }

    void Start()
    {

        if (photonView.IsMine)
        {
            playerManager = GetComponent<MovementController>().playerManager;
            // healthBarImage = transform.Find("Canvas/HealthbarBackground/Healthbar").GetComponent<Image>();
            // Debug.Log(healthBarImage);
            // UI = GameObject.Find("Canvas");

            //Debug.Log("collition playerManager: " + playerManager);

            //CollisionTrigger = GetComponentInChildren<CollisionTrigger>();
        }
        else
        {
            Destroy(UI);//make sure the UI panel not messed up when play online
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
        DisplayHealthBar(healthBarImage, playerManager.billboardvalue);
        if (currentHealth <= 0)
        {
            Invoke("Die", 0.2f);

            return;
        }
        // healthBarImage.fillAmount = billboardvalue;
    }
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        BelowDeathAltitude();
    }


    //https://docs.unity3d.com/Manual/ExecutionOrder.html
    void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        // Debug.Log(target.photonView.Owner.NickName);

        if (collision.rigidbody)
        {//&& collision.GetType() == typeof(SphereCollider)
            other_Player_Name = collision.collider.GetComponent<PhotonView>().Owner.NickName;
            other_Player_Velocity = collision.gameObject.GetComponent<CollisionDetect>().Player_Velocity;
            hitDirection = System.Math.Round(Vector3.Dot(Player_Velocity, other_Player_Velocity), 3);

            var finalDamage = judgeDamage(collision, Player_Velocity, other_Player_Velocity, hitDirection) * damageTimer;
            if (ISufferDamage)
            {
                var hitessage = photonView.Owner.NickName + " got damage from " + other_Player_Name + " with value:" + finalDamage;
                Debug.Log(hitessage);
                //currentHealth -= finalDamage;
                //photonView.RPC("Damage", RpcTarget.All, finalDamage, hitessage);//send damage to all players， this function will change  'currentHealth' value
                playerManager.TakeDamage(finalDamage, hitessage);
                //  billboardvalue = currentHealth / playerHealth;// this will display user's current health
                currentHealth = playerManager.currentHealth;
                if (currentHealth <= 0)
                {
                    Invoke("Die", 0.2f);
                    PlayerManager otherPlayerManager = collision.collider.GetComponent<MovementController>().playerManager;
                    otherPlayerManager.Kill(other_Player_Name);
                    return;
                }
            }

            // judgeDamage (Player_Velocity, other_Player_Velocity, hitDirection);
            // Vector3 otherV =
            //     GameObject.Find("Sphere").GetComponent<cam>().Player_Velocity;
            // Vector3 PlayerPosition = this.transform.position;
            // Vector3 TesterPosition = collision.transform.position;
            // Vector3 direction = (TesterPosition - PlayerPosition);
        }
        if (collision.gameObject.name == "Transfer platform")
        {
            //This will make the player a child of the Obstacle
            transform.parent = collision.gameObject.transform.parent; //Change "myPlayer" to your player

            //collision.gameObject.GetComponent<getspeed>().enabled = true;
            Debug.Log("changed Parent to " + collision.gameObject.name);
        }
    }
    [PunRPC]
    void Damage(float finalDamage, string message)
    {
        currentHealth -= finalDamage;

        Debug.Log("**************** " + message + " ****************");

    }
    void OnCollisionExit(Collision collision)
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
    public static void DisplayHealthBar(Image healthBarImage, float billboardvalue)
    {
        Color goodHealth = Color.green;
        Color mediumHealth = Color.yellow;
        Color badHealth = Color.red;
        Color currentColor = healthBarImage.color;
        //Color.Lerp(currentColor, Color.blue, Mathf.PingPong(Time.time, 1));

        healthBarImage.fillAmount = billboardvalue;
        if (billboardvalue >= 0.6f)
        {
            healthBarImage.color = goodHealth;
        }
        else if (billboardvalue < 0.6f && billboardvalue >= 0.3f)
        {
            healthBarImage.color = mediumHealth;
        }
        else { healthBarImage.color = badHealth; }

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
    public float judgeDamage(Collision collision, Vector3 Player_Velocity, Vector3 other_Player_Velocity, double hitDirection)
    {
        hitForce = collision.relativeVelocity.magnitude;
        var damage =
            Mathf
                .Abs(Player_Velocity.magnitude -
                other_Player_Velocity.magnitude);

        //++++++++++++++++
        if (
            hitDirection >= 0.05 //same direction
        )
        {
            if (
                Player_Velocity.magnitude > other_Player_Velocity.magnitude //player speed is higher
            )
            {
                damage = Mathf.Abs(Player_Velocity.magnitude - other_Player_Velocity.magnitude);
                // Debug.Log(name + " same direction hit " + other_Player_Name + " with differ Force:" + damage);
                ISufferDamage = false;

            }
            else if (
                Player_Velocity.magnitude < other_Player_Velocity.magnitude //player speed is lower
            )
            {
                // Debug.Log(name + " hitted by " + other_Player_Name + " with differ Force:" + damage);
                ISufferDamage = true;
            }
            //return damage;

        }
        else if (
            hitDirection <= 0.05 && hitDirection >= -0.05 //between +- 0.05 means someone is motionless
        )
        {
            if (
                Player_Velocity.magnitude > other_Player_Velocity.magnitude //player speed is higher
            )
            {
                //Debug.Log(name + " hit " + other_Player_Name + " with Force:" + Player_Velocity.magnitude);
                ISufferDamage = false;
                //return Player_Velocity.magnitude;

            }
            else if (
                Player_Velocity.magnitude < other_Player_Velocity.magnitude //player speed is lower
            )
            {
                //Debug.Log(name + " hitted by " + other_Player_Name + " with Force:" + other_Player_Velocity.magnitude);
                ISufferDamage = true;
                // return other_Player_Velocity.magnitude;
            }
            //return 666666;

        }
        else
        {
            //<0 means opposite direction
            if (
                Player_Velocity.magnitude > other_Player_Velocity.magnitude //player speed is higher
            )
            {
                // Debug.Log(name + " hit " + other_Player_Name + " with differ Force:" + damage);
                ISufferDamage = false;

            }
            else if (
                Player_Velocity.magnitude < other_Player_Velocity.magnitude //player speed is lower
            )
            {
                // Debug.Log(name + " hitted by " + other_Player_Name + " with differ Force:" + damage);
                ISufferDamage = true;
            }
            //return damage;

        }
        return hitForce;
    }
    void BelowDeathAltitude()
    {
        if (transform.localPosition.y < deathAltitude)
        {
            Die();
            // gameObject.transform.position =
            //     new Vector3(target.x + Random.Range(3, 10),
            //         100,
            //         target.z + Random.Range(3, 10));
        }
    }
    void Die()
    {
        if (!playerManager)
        {
            Debug.Log("No playerManager");
            return;
        }
        else
            playerManager.Die();
    }
}

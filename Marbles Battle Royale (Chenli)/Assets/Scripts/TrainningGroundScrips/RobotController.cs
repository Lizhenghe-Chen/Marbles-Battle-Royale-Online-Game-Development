using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;//will use Player Type
using System.Collections;

public class RobotController : MonoBehaviourPunCallbacks
{
    public GameObject Target;
    public bool hasTarget;
    public float findTargetTimer = 3f;
    PhotonView robotPhotonView;
    private Rigidbody rb; // player
    [SerializeField] private Transform Eye;
    private Image healthBarImage;
    [Tooltip("loopMovement and loopRush should enable in same time to get best direction")]
    [SerializeField] private bool loopMovement, loopRush; //
    [SerializeField]
    private coolingTimeRange JumpCoolingTimeRange, RushCoolingTimeRange;
    [SerializeField]
    float RushCoolingTime = 3, JumpCoolingTime = 3,
    jumpforce = 200, rushForce = 400, attackRange = 50;//player jump cooling time

    //Create a custom struct and apply [Serializable] attribute to it
    [Serializable]
    public struct coolingTimeRange
    {
        public float min;
        public float max;
    }

    public Vector3 robotVelocity;
    public const float robotHealth = 100f;
    public float robotCurrentHealth;
    public float billboardvalue;
    public Vector3 startPointPosition;
    public Vector3 initialScale;
    public float maxScale = 4, minScale = 0.2f;
    public Vector3 scaleSpeed = new Vector3(0.001f, 0.001f, 0.001f);
    [SerializeField] GameInfoManager GameInfoManager;
    void Awake() { robotPhotonView = GetComponent<PhotonView>(); }
    void Start()
    {
        initialScale = transform.localScale;
        rb = GetComponent<Rigidbody>();
        healthBarImage = this.transform.Find("BillBoard/showHealthbarBackground/Healthbar").GetComponent<Image>();

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CheckEnemy());
            StartCoroutine(WaitRush());
            StartCoroutine(WaitJump());
            startPointPosition = this.transform.position;
            GameInfoManager = GameObject.Find("GameInfoCanvas/GameInfo").GetComponent<GameInfoManager>();
            robotCurrentHealth = robotHealth;
            //billboardvalue = robotCurrentHealth / robotHealth;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // robotPhotonView.RPC("SendHealthData", RpcTarget.All, robotCurrentHealth, billboardvalue);
            belowDeathAltitudeOrDead();

            if (hasTarget) { Eye.transform.LookAt(Target.transform); }
            //LoopJump();
            //LoopRush();

        }

        billboardvalue = robotCurrentHealth / robotHealth;

        InGameUIManager.SetHealthBar(healthBarImage, billboardvalue);
    }
    void FixedUpdate()
    {
        robotVelocity = rb.velocity;//let all player can get this locally
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        Movement();
        if (transform.localScale.x > initialScale.x)
        {
            transform.localScale -= scaleSpeed;
        }
        if (transform.localScale.x < initialScale.x)
        {
            transform.localScale += scaleSpeed;
        }

    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.rigidbody && other.transform.tag == "Player")
        {
            CollisionDetect collisionDetect = other.gameObject.GetComponent<CollisionDetect>();
            Damage(other, collisionDetect.Player_Velocity);
        }
        if (other.transform.tag == "Robot")
        {
            RobotController robotController = other.gameObject.GetComponent<RobotController>();
            Damage(other, robotController.robotVelocity);
        }
    }
    void Damage(Collision other, Vector3 other_Player_Velocity)
    {
        if (!PhotonNetwork.IsMasterClient)
        { return; }

        // var hitDirection = System.Math.Round(Vector3.Dot(Player_Velocity, other_Player_Velocity), 3);
        if (robotVelocity.magnitude < other_Player_Velocity.magnitude)
        {
            string other_Player_Name;
            var finalDamage = other.relativeVelocity.magnitude * 1.5f * other.collider.GetComponent<Rigidbody>().mass;//let different type of ball have different damage
            robotCurrentHealth -= finalDamage;
            if (robotCurrentHealth <= 0)
            {
                if (other.collider.GetComponent<MovementController>() != null)//if killer is a player
                {
                    other_Player_Name = other.collider.GetComponent<PhotonView>().Owner.NickName;

                }
                else { other_Player_Name = other.transform.name; }
                GameInfoManager.GlobalRefresh(other_Player_Name + " X -> " + this.name);
            }
            robotPhotonView.RPC("SendHealthData", RpcTarget.All, robotCurrentHealth, billboardvalue);
        }
    }
    void belowDeathAltitudeOrDead()
    {
        if (transform.position.y < -20 || robotCurrentHealth <= 0)
        {
            if (transform.position.y < -20)
            {
                GameInfoManager.GlobalRefresh(this.name + " Fall dead");
            }
            // transform.position = startPointPosition;
            // rb.velocity = Vector3.zero;
            // rb.angularVelocity = Vector3.zero;
            // robotCurrentHealth = robotHealth;
            robotPhotonView.RPC("DestroyForAll", RpcTarget.All);

            // robotPhotonView.RPC("SendHealthData", RpcTarget.All, robotCurrentHealth, billboardvalue);
        }
    }

    IEnumerator WaitJump()
    {
        while (true)
        {
            if (hasTarget)
            {
                rb.AddForce(Vector3.up * jumpforce);
                JumpCoolingTime = UnityEngine.Random.Range(JumpCoolingTimeRange.min, JumpCoolingTimeRange.max);
            }
            yield return new WaitForSeconds(JumpCoolingTime);//every 5 seconds find the closest enemy
        }
    }
    IEnumerator WaitRush()
    {
        while (true)
        {
            if (hasTarget)
            {
                rb.AddForce(Eye.transform.forward * rushForce);
                RushCoolingTime = UnityEngine.Random.Range(RushCoolingTimeRange.min, RushCoolingTimeRange.max);
            }
            yield return new WaitForSeconds(RushCoolingTime);//every 5 seconds find the closest enemy
        }
    }

    IEnumerator CheckEnemy()
    {
        while (loopRush || loopMovement)//only do below if robot need rush and move
        {
            Target = FindClosestEnemy();
            hasTarget = (Target == null) ? false : true;
            yield return new WaitForSeconds(5f);      //every 5 seconds find the closest enemy
        }
    }
    // public void LoopRush()
    // {
    //     if (loopRush)
    //     {
    //         RushTimer += Time.deltaTime;
    //         if (RushTimer >= RushCoolingTime)
    //         {
    //             RushTimer = RushCoolingTime;
    //             rb.AddForce(Eye.transform.forward * rushForce);
    //             RushCoolingTime = UnityEngine.Random.Range(RushCoolingTimeRange.min, RushCoolingTimeRange.max);
    //             RushTimer = 0;
    //         }
    //     }
    // }
    // public void LoopJump()
    // {
    //     if (loopJump)
    //     {
    //         JumpTimer += Time.deltaTime;
    //         if (JumpTimer >= JumpCoolingTime)
    //         {
    //             JumpTimer = JumpCoolingTime;
    //             rb.AddForce(Vector3.up * jumpforce);
    //             JumpCoolingTime = UnityEngine.Random.Range(JumpCoolingTimeRange.min, JumpCoolingTimeRange.max);
    //             JumpTimer = 0;
    //         }
    //     }
    // }
    public void Movement()
    {
        if (loopMovement && hasTarget)
        {
            rb.AddTorque(Eye.transform.right * RushCoolingTime);
        }
    }


    public GameObject FindClosestEnemy()
    {
        //https://docs.unity3d.com/ScriptReference/GameObject.FindGameObjectsWithTag.html
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closestTarget = null;
        var distance = attackRange;
        Vector3 position = transform.position;
        foreach (GameObject player in players)
        {
            float pointsDistance = Vector3.Distance(player.transform.position, position);
            if (pointsDistance < distance)
            {
                closestTarget = player;
                distance = pointsDistance;
            }
        }
        // if (Vector3.Distance(transform.position, closestTarget.transform.position) < distance) { }
        Array.Clear(players, 0, players.Length);
        return closestTarget;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //when other players join this room
    {
        if (PhotonNetwork.IsMasterClient)
        { robotPhotonView.RPC("SendHealthData", RpcTarget.All, robotCurrentHealth, billboardvalue); }

    }
    public override void OnMasterClientSwitched(Player newMasterClient) //after master client leaved
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CheckEnemy());
            StartCoroutine(WaitRush());
            StartCoroutine(WaitJump());
            GameInfoManager = GameObject.Find("GameInfoCanvas/GameInfo").GetComponent<GameInfoManager>();
            robotCurrentHealth = robotHealth;
            //billboardvalue = robotCurrentHealth / robotHealth;
        }
    }
    [PunRPC]
    void SendHealthData(float _currentHealth, float _billboardvalue)
    {
        robotCurrentHealth = _currentHealth;
        billboardvalue = _billboardvalue;
    }
    [PunRPC]
    public void DestroyForAll()
    {
        Debug.Log("DestroyForAll");
        Destroy(this.gameObject);
        // Destroy(PhotonView.Find(viewID).gameObject);
    }

}

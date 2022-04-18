using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;//will use Player Type
/*
* Copyright (C) 2022 Author: Lizhenghe.Chen.
* For personal study or educational use.
* Email: Lizhenghe.Chen@qq.com
*/
public class RobotController : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform Player;
    PhotonView robotPhotonView;
    [SerializeField] Rigidbody rb; // player
    [SerializeField] Transform Eye;
    [SerializeField] Image healthBarImage;
    [Tooltip("loopMovement and loopRush should enable in same time to get best direction")]
    [SerializeField] private bool loopMovement, loopJump, loopRush; //
    [SerializeField]
    private coolingTimeRange JumpCoolingTimeRange, RushCoolingTimeRange;
    [SerializeField]
    float RushCoolingTime = 3, RushTimer, JumpCoolingTime = 3, JumpTimer,
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
    public GameObject Enemy;
    public bool hasTarget;

    public Vector3 initialScale;
    public float maxScale = 4, minScale = 0.2f;
    [SerializeField] GameInfoManager GameInfoManager;
    void Awake() { robotPhotonView = GetComponent<PhotonView>(); }
    void Start()
    {
        initialScale = transform.localScale;
        rb = GetComponent<Rigidbody>();
        healthBarImage = this.transform.Find("BillBoard/showHealthbarBackground/Healthbar").GetComponent<Image>();

        if (PhotonNetwork.IsMasterClient)
        {
            startPointPosition = this.transform.position;
            Player = this.transform;
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
            if (loopRush || loopMovement)
            {
                CheckEnemy();
                if (hasTarget) { Eye.transform.LookAt(Enemy.transform); } else { return; }
            }
            LoopJump();
            LoopRush();
            LoopMovement();
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
        if (transform.localScale.x > initialScale.x)
        {
            transform.localScale -= new Vector3(0.001f, 0.001f, 0.001f);
        }
        if (transform.localScale.x < initialScale.x)
        {
            transform.localScale += new Vector3(0.001f, 0.001f, 0.001f);
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
        if (CollisionDetect.judgeDamage(robotVelocity, other_Player_Velocity))
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

    //https://docs.unity3d.com/ScriptReference/GameObject.FindGameObjectsWithTag.html
    public void LoopJump()
    {
        if (loopJump)
        {
            JumpTimer += Time.deltaTime;
            if (JumpTimer >= JumpCoolingTime)
            {
                JumpTimer = JumpCoolingTime;
                rb.AddForce(Vector3.up * jumpforce);
                JumpCoolingTime = UnityEngine.Random.Range(JumpCoolingTimeRange.min, JumpCoolingTimeRange.max);
                JumpTimer = 0;
            }
        }
    }
    public void LoopRush()
    {
        if (loopRush)
        {
            RushTimer += Time.deltaTime;
            if (RushTimer >= RushCoolingTime)
            {
                RushTimer = RushCoolingTime;
                rb.AddForce(Eye.transform.forward * rushForce);
                RushCoolingTime = UnityEngine.Random.Range(RushCoolingTimeRange.min, RushCoolingTimeRange.max);
                RushTimer = 0;
            }
        }
    }
    public void LoopMovement()
    {
        if (loopMovement)
        {
            rb.AddTorque(Eye.transform.right * RushCoolingTime);
        }
    }
    public void CheckEnemy()
    {
        if (loopJump || loopRush)
        {
            Enemy = FindClosestEnemy();
            if (Enemy == null) { hasTarget = false; } else { hasTarget = true; }
        }
    }
    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        var distance = attackRange;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        // if (Vector3.Distance(transform.position, closest.transform.position) < distance) { }
        return closest;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //when other players join this room
    {
        if (PhotonNetwork.IsMasterClient)
        { robotPhotonView.RPC("SendHealthData", RpcTarget.All, robotCurrentHealth, billboardvalue); }

    }
    public override void OnMasterClientSwitched(Player newMasterClient) //after master client leaved
    {
        GameInfoManager = GameObject.Find("GameInfoCanvas/GameInfo").GetComponent<GameInfoManager>();
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

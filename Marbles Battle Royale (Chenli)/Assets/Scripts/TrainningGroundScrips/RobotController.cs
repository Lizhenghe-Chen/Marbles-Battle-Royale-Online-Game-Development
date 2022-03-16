using System;
using UnityEngine;
using UnityEngine.UI;
public class RobotController : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] Rigidbody rb; // player
    [SerializeField] Transform Eye;
    [SerializeField] Image healthBarImage;
    [Tooltip("loopMovement and loopRush should enable in same time to get best direction")]
    [SerializeField] private bool loopMovement, loopJump, loopRush; //
    [SerializeField]
    private coolingTimeRange JumpCoolingTimeRange, RushCoolingTimeRange;
    [SerializeField]
    float RushCoolingTime = 3, RushTimer, JumpCoolingTime = 3, JumpTimer,
    jumpforce = 200, rushForce = 400, attackRange=50;//player jump cooling time

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
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        healthBarImage = this.transform.Find("BillBoard/showHealthbarBackground/Healthbar").GetComponent<Image>();
        startPointPosition = this.transform.position;
        Player = this.transform;
        robotCurrentHealth = robotHealth;
        billboardvalue = robotCurrentHealth / robotHealth;
    }

    // Update is called once per frame
    void Update()
    {
        billboardvalue = robotCurrentHealth / robotHealth;
        healthBarImage.fillAmount = billboardvalue;

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
    void FixedUpdate() { robotVelocity = rb.velocity; }
    private void OnCollisionEnter(Collision other)
    {
        if (other.rigidbody && other.transform.tag == "Player" || other.transform.tag == "Robot")
        {
            CollisionDetect collisionDetect = other.gameObject.GetComponent<CollisionDetect>();
            var Player_Velocity = rb.velocity;
            var other_Player_Velocity = collisionDetect.Player_Velocity;
            var hitDirection = System.Math.Round(Vector3.Dot(Player_Velocity, other_Player_Velocity), 3);
            var finalDamage = collisionDetect.hitForce;
            robotCurrentHealth -= finalDamage;
        }
    }
    void belowDeathAltitudeOrDead()
    {
        if (transform.localPosition.y < -20 || robotCurrentHealth <= 0)
        {
            transform.position = startPointPosition;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            robotCurrentHealth = robotHealth;
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
                rb.AddForce(Eye.transform.forward * jumpforce);
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
        float distance = attackRange;
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
}

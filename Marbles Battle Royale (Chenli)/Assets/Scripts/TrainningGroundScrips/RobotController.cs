
using UnityEngine;
using UnityEngine.UI;
public class RobotController : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] Rigidbody rb; // player
    [SerializeField] Image healthBarImage;
    public const float robotHealth = 100f;
    public float robotCurrentHealth;
    public float billboardvalue;
    public Vector3 startPointPosition;
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
        if (transform.localPosition.y < -20)
        {

            transform.position = startPointPosition;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            robotCurrentHealth = robotHealth;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.rigidbody)
        {
            CollisionDetect collisionDetect = other.gameObject.GetComponent<CollisionDetect>();
            var Player_Velocity = rb.velocity;
            var other_Player_Velocity = collisionDetect.Player_Velocity;
            var hitDirection = System.Math.Round(Vector3.Dot(Player_Velocity, other_Player_Velocity), 3);
            var finalDamage = collisionDetect.hitForce;
            robotCurrentHealth -= finalDamage;



        }
    }
}

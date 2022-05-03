using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
*This CollisionTrigger can make sure get the latest collision data, 
*also prevent player cannot jump due to the jolt
**/
public class CollisionTrigger : MonoBehaviour
{
    public Rigidbody rb; // player
   // [SerializeField] Collider m_Collider;
    public double hitDirection; // the Dot of two objects' velocity, >0 means around same direction,<0 means hit in opposite direction

    public Vector3 other_Player_Velocity; //

    public string other_Player_Name;

    public bool onTheGround = false;

    void Start()
    {
        //   rb = GetComponent<Rigidbody>();


        // m_Collider = GetComponent<CapsuleCollider>();
        // m_Collider.enabled = false;
    }

    public static Vector3 camVector;



    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
        {
        //    m_Collider.enabled = true;
            other_Player_Name = other.name;
            other_Player_Velocity = other.GetComponent<Rigidbody>().velocity;

            // Debug
            //     .Log(other_Player_Name+" speed: "+
            //     other.GetComponent<Rigidbody>().velocity);
            //https://docs.unity3d.com/ScriptReference/Vector3.Dot.html
            hitDirection =
                System
                    .Math
                    .Round(Vector3
                        .Dot(rb.velocity,
                        other_Player_Velocity),
                    3);
            // Debug
            //     .Log(name +
            //     " V: " +
            //     System.Math.Round(other_Player_Velocity.magnitude, 3) +
            //     "Player V: " +
            //     System.Math.Round(rb.velocity.magnitude, 3) +
            //     "Velocity Dot: " +
            //     hitDirection);
        }
        // if (other.tag == "elevator")
        // {
        //     other.gameObject.GetComponent<TouchUp>().enabled = true;
        //     Debug.Log(other.tag + "!!!");
        // }
    }

    void OnTriggerExit(Collider collision)
    {
      //  if (collision.GetComponent<Rigidbody>()) m_Collider.enabled = false;


        //  if (collision.tag == "elevator")
        // {
        //     collision.gameObject.GetComponent<TouchUp>().enabled = false;

        // }
        //onTheGround = false;
        // if (collision.name == "ground")
        // {

        // }
    }
}

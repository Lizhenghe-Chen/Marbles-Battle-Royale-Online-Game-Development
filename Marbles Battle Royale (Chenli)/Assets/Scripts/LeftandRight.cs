using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftandRight : MonoBehaviour
{
    public bool updown = true;

    public float speed = 1.2f;

    public float left = -5f;

    public float right = 5f;

    public float upRange = 10f;

    public float downRange = 5f;

    static float

            locationX,
            locationY,
            locationZ;

    // Start is called before the first frame update
    void Start()
    {
        locationX = this.transform.position.x;
        locationY = this.transform.position.y;
        locationZ = this.transform.position.z;
    }

    // Update is called once per frame
    Vector3 newPosition = new Vector3(locationX + 10, locationY, locationZ);

    void Update()
    {
        // float lerpValue = Mathf.Lerp(locationX - 10, locationX + 10, 0.0005f);
        //transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        method1();
    }

    void method1()
    {
        if (updown == true)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;

            if (
                transform.position.y - locationY >= upRange ||
                transform.position.y - locationY <= downRange
            )
            {
                speed = -speed;
            }
        }
        else
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
            float judge = transform.position.x - locationX;

            // Debug
            //     .Log(locationX +
            //     " : " +
            //     transform.position.x +
            //     " difference:" +
            //     (transform.position.x - locationX) +
            //     "speed" +
            //     speed);
            if (judge < 0 && judge <= left)
            {
                speed = Mathf.Abs(speed);
                //   Debug.Log("go right");
            }
            else if (judge > 0 && judge > right)
            {
                speed = -Mathf.Abs(speed);
                //  Debug.Log("go left");
            }
        }
    }
}

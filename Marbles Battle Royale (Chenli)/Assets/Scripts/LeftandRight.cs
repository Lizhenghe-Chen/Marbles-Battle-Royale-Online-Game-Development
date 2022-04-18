using UnityEngine;
using Photon.Pun;
public class LeftandRight : MonoBehaviour
{
    public bool updown = true;
    public float speed = 1.2f;
    public float left = -5f;
    public float right = 5f;
    public float upRange = 10f;
    public float downRange = 5f;

    [SerializeField] float locationX, locationY, locationZ;

    // Start is called before the first frame update
    void Start()
    {
        locationX = this.transform.position.x;
        locationY = this.transform.position.y;
        locationZ = this.transform.position.z;
    }

    // void Update()
    // {
    //     // float lerpValue = Mathf.Lerp(locationX - 10, locationX + 10, 0.0005f);
    //     //transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);
    // }

    void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient)
        { return; }
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
            if ((judge < 0 && judge <= left) || (judge > 0 && judge > right))
            {
                speed = -speed;
                //   Debug.Log("go right");
            }
        }
    }
}
